using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Services;
using UnityGame.Assets.Modules.UserSystem;
using CameraSystem.Services;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
	/// <summary>
	/// タイル選択の入力処理とロジックを担当するコントローラー
	/// </summary>
	public class SelectionController : ITickable, IInitializable
	{
		private readonly PlayerManager _playerManager;
		private readonly DisplayTileState _displayTileState;
		private readonly TileManager _tileManager;
		private readonly GameStateController _gameStateController;
		private readonly CameraShaker _cameraShaker;
		private readonly PlayerMover _playerMover;
		private readonly SignalBus _signalBus;

		// 現在選択中のタイルキー
		private string _currentSelectedTileKey = null;

		// 残りの移動可能距離
		private int _remainingMoveDistance = 0;

		// 移動距離の範囲
		private const int MIN_MOVE_DISTANCE = 1;
		private const int MAX_MOVE_DISTANCE = 5;

		// 訪問したタイルのリスト（通った経路を記録）
		private List<string> _visitedTiles = new List<string>();

		// タイルの元の所有者を保存する辞書（所有者から色を逆算できる）
		private Dictionary<string, string> _originalTileOwners = new Dictionary<string, string>();

		public SelectionController(
			PlayerManager playerManager,
			DisplayTileState displayTileState,
			TileManager tileManager,
			GameStateController gameStateController,
			CameraShaker cameraShaker,
			PlayerMover playerMover,
			SignalBus signalBus)
		{
			_playerManager = playerManager;
			_displayTileState = displayTileState;
			_tileManager = tileManager;
			_gameStateController = gameStateController;
			_cameraShaker = cameraShaker;
			_playerMover = playerMover;
			_signalBus = signalBus;
		}

		public void Initialize()
		{
			// 初期化処理（必要に応じて）
		}

		public void Tick()
		{
			// Alice専用：選択モードがアクティブでない場合、スペースキーで選択モードを開始
			// ターン制ではないため、いつでも移動可能
			if (!IsSelectionModeActive && Input.GetKeyDown(KeyCode.Space))
			{
				// Alice（player001）の選択モードを開始（ターン制ではないためいつでも可能）
				StartSelectionMode("player001");
			}

			// 選択モード中の処理（Alice専用）
			if (IsSelectionModeActive)
			{
				HandleArrowKeySelection();
			}
		}

		/// <summary>
		/// 選択モードを開始する（指定されたプレイヤーの位置を基準に）
		/// </summary>
		/// <param name="userId">プレイヤーのUserId</param>
		public void StartSelectionMode(string userId)
		{
			// 指定されたプレイヤーの現在位置を取得
			string playerTileKey = _playerManager.GetPlayerTileKeyByUserId(userId);

			if (playerTileKey == null)
			{
				Debug.LogWarning($"{userId}の位置が見つかりません");
				return;
			}

			// 移動可能距離をランダムに設定（1～5）
			_remainingMoveDistance = Random.Range(MIN_MOVE_DISTANCE, MAX_MOVE_DISTANCE + 1);
			_visitedTiles.Clear();
			_visitedTiles.Add(playerTileKey); // 開始位置を訪問済みに追加

			Debug.Log($"【選択開始】移動可能距離: {_remainingMoveDistance}");

			// プレイヤーの位置から移動可能な範囲を表示
			_displayTileState.DisplayClicableTile(playerTileKey, _remainingMoveDistance);

			// 初期選択位置をプレイヤーの位置に設定
			_currentSelectedTileKey = playerTileKey;

			// 初期選択位置をハイライト
			_displayTileState.HighlightMarker(playerTileKey);

			// GameStateControllerで選択モードを開始
			_gameStateController.StartSelection();
		}

		/// <summary>
		/// 十字キーで行き先を選択し、Enterキーで決定する
		/// </summary>
		private void HandleArrowKeySelection()
		{
			// Zキーが押されているかチェック（押されている場合は列/行無視モード）
			bool ignoreAlignment = Input.GetKey(KeyCode.Z);

			// 十字キー入力で選択を移動
			if (Input.GetKeyDown(KeyCode.UpArrow))
			{
				MoveSelection(0, 0, 1, ignoreAlignment);  // +Z方向
			}
			else if (Input.GetKeyDown(KeyCode.DownArrow))
			{
				MoveSelection(0, 0, -1, ignoreAlignment); // -Z方向
			}
			else if (Input.GetKeyDown(KeyCode.RightArrow))
			{
				MoveSelection(1, 0, 0, ignoreAlignment);  // +X方向
			}
			else if (Input.GetKeyDown(KeyCode.LeftArrow))
			{
				MoveSelection(-1, 0, 0, ignoreAlignment); // -X方向
			}

			// Enterキーで選択を決定
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				ConfirmSelection();
			}

			// Escapeキーで選択モードをキャンセル
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				CancelSelectionMode();
			}
		}

		/// <summary>
		/// 選択を指定方向に移動する
		/// クリック可能なタイルの中から、指定方向にある最も近いタイルに移動
		/// 優先順位：同じ列/行のタイル → 他の列/行のタイル（ignoreAlignmentがtrueの場合は列/行無視）
		/// </summary>
		/// <param name="ignoreAlignment">trueの場合、列/行の優先を無視して最も近いタイルを選択</param>
		private void MoveSelection(int offsetX, int offsetY, int offsetZ, bool ignoreAlignment = false)
		{
			if (_currentSelectedTileKey == null)
				return;

			// Alice（player001）のGameObjectを取得
			var aliceGameObject = _playerManager.GetPlayerGameObjectByUserId("player001");

			// Alice移動中の場合は新しい移動を受け付けない
			if (aliceGameObject != null && _playerMover != null && _playerMover.IsMoving(aliceGameObject.transform))
			{
				Debug.Log("プレイヤー移動中のため、次の移動は待機してください");
				return;
			}

			// ========== ステップ1: 現在位置の取得 ==========
			var currentTileData = _tileManager.GetTileData(_currentSelectedTileKey);
			if (currentTileData == null)
				return;

			int currentX = (int)currentTileData.Position.x;
			int currentZ = (int)currentTileData.Position.z;

			// ========== 戻る方向のチェック（clickableTiles に関係なく移動可能） ==========
			// 来た道を戻る場合は、移動可能距離が0でも戻れるようにする
			if (_visitedTiles.Count >= 2)
			{
				string previousTileKey = _visitedTiles[_visitedTiles.Count - 2];
				var previousTileData = _tileManager.GetTileData(previousTileKey);

				if (previousTileData != null)
				{
					int prevX = (int)previousTileData.Position.x;
					int prevZ = (int)previousTileData.Position.z;

					// 移動方向が前のタイルの方向と一致するかチェック
					int deltaX = prevX - currentX;
					int deltaZ = prevZ - currentZ;

					// 矢印キーの方向と、前のタイルへの方向が一致する場合は戻る処理
					bool isGoingBackDirection = false;
					if (offsetX > 0 && deltaX > 0 && deltaZ == 0) isGoingBackDirection = true; // 右方向
					if (offsetX < 0 && deltaX < 0 && deltaZ == 0) isGoingBackDirection = true; // 左方向
					if (offsetZ > 0 && deltaZ > 0 && deltaX == 0) isGoingBackDirection = true; // 上方向
					if (offsetZ < 0 && deltaZ < 0 && deltaX == 0) isGoingBackDirection = true; // 下方向

					if (isGoingBackDirection)
					{
						// 戻る処理を実行して終了
						GoBackToPreviousTile(previousTileKey);
						return;
					}
				}
			}

			// ========== ステップ2: 候補タイルの準備 ==========
			// クリック可能なタイル全てを取得
			var clickableTiles = _tileManager.GetClickableTiles();

			// 最適な候補を保存する変数
			string bestTileKey = null;           // 優先候補（同じ列/行 or ignoreAlignment=trueの場合は最短距離）
			float bestDistance = float.MaxValue;
			string fallbackTileKey = null;       // フォールバック候補（違う列/行）
			float fallbackDistance = float.MaxValue;

			// ========== ステップ3: 全ての候補タイルをチェック ==========
			foreach (var tileKey in clickableTiles)
			{
				// 現在のタイルはスキップ（自分自身には移動しない）
				if (tileKey == _currentSelectedTileKey)
					continue;

				var tileData = _tileManager.GetTileData(tileKey);
				if (tileData == null)
					continue;

				int tileX = (int)tileData.Position.x;
				int tileZ = (int)tileData.Position.z;

				// --- 方向と列/行の判定 ---
				// このタイルが「押した矢印の方向にあるか」と「同じ列/行にあるか」をチェック
				bool isInDirection = false;  // 押した方向にあるか
				bool isSameLane = false;     // 同じ列/行にあるか

				if (offsetX > 0) // 右矢印（+X方向）
				{
					isInDirection = tileX > currentX;  // 右にあるか
					isSameLane = tileZ == currentZ;    // 同じ列（Z座標が同じ）か
				}
				else if (offsetX < 0) // 左矢印（-X方向）
				{
					isInDirection = tileX < currentX;  // 左にあるか
					isSameLane = tileZ == currentZ;    // 同じ列（Z座標が同じ）か
				}
				else if (offsetZ > 0) // 上矢印（+Z方向）
				{
					isInDirection = tileZ > currentZ;  // 上にあるか
					isSameLane = tileX == currentX;    // 同じ行（X座標が同じ）か
				}
				else if (offsetZ < 0) // 下矢印（-Z方向）
				{
					isInDirection = tileZ < currentZ;  // 下にあるか
					isSameLane = tileX == currentX;    // 同じ行（X座標が同じ）か
				}

				// 押した方向にないタイルはスキップ
				if (!isInDirection)
					continue;

				// --- 距離の計算 ---
				// 現在位置からこのタイルまでの距離（マンハッタン距離）
				float distance = Mathf.Abs(tileX - currentX) + Mathf.Abs(tileZ - currentZ);

				// --- モードによって候補の選び方を変える ---
				if (ignoreAlignment)
				{
					// ========== Zキー押している場合：押した方向に連なっている最も遠いタイルを選択 ==========
					if (isSameLane)
					{
						// 同じ列/行にあるタイル → より遠いタイルを優先（連なりの端まで移動）
						if (bestTileKey == null || distance > bestDistance)
						{
							bestDistance = distance;
							bestTileKey = tileKey;
						}
					}
				}
				else
				{
					// ========== 通常モード：同じ列/行を優先 ==========
					if (isSameLane)
					{
						// 同じ列/行にあるタイル → 優先候補に保存
						if (distance < bestDistance)
						{
							bestDistance = distance;
							bestTileKey = tileKey;
						}
					}
					else
					{
						// 違う列/行のタイル → フォールバック候補に保存
						// （同じ列/行に候補がない場合のみ使用される）
						if (distance < fallbackDistance)
						{
							fallbackDistance = distance;
							fallbackTileKey = tileKey;
						}
					}
				}
			}

			// ========== ステップ4: 最終的な移動先を決定 ==========
			// 優先候補があればそれを、なければフォールバック候補を使用
			// （通常モード: bestTileKey=同じ列/行の最短 → fallbackTileKey=違う列/行の最短）
			// （Zキー押下: bestTileKey=列/行無視の最短 → fallbackTileKeyは使わない）
			string targetTileKey = bestTileKey ?? fallbackTileKey;

			// ========== ステップ5: 移動可能距離をチェック ==========
			// targetTileKeyの有無に関わらず、移動距離が0なら進めない
			if (_remainingMoveDistance <= 0)
			{
				// 移動不可 - カメラシェイク
				_cameraShaker.Shake(0.2f, 0.1f);
				Debug.Log("移動可能距離が0です。これ以上進めません。");
				return;
			}

			// ========== ステップ6: 選択を実行 ==========
			if (targetTileKey != null)
			{
				// 進行方向を計算（現在位置から目標位置へのベクトル）
				var currentTilePos = _tileManager.GetTileData(_currentSelectedTileKey)?.Position;
				var targetTilePos = _tileManager.GetTileData(targetTileKey)?.Position;
				Vector3 direction = Vector3.zero;

				if (currentTilePos != null && targetTilePos != null)
				{
					direction = (targetTilePos.ToVector3() - currentTilePos.ToVector3()).normalized;
				}

				// プレイヤーを新しいタイルに移動させる（進行方向を向く）
				string currentPlayerId = _gameStateController.GetCurrentPlayerId();
				_playerManager.MovePlayerToTileKeyWithDirection(currentPlayerId, targetTileKey, direction);

				// プレイヤー移動シグナルを発火（移動中のタイル選択時）
				var playerPosition = _playerManager.GetPlayerPositionByUserId(currentPlayerId);
				if (playerPosition.HasValue)
				{
					_signalBus.Fire(new PlayerMovedSignal(currentPlayerId, targetTileKey, playerPosition.Value));
				}

				// 新しいタイルを選択
				_currentSelectedTileKey = targetTileKey;

				// 訪問前の元の所有者を保存（まだ保存していない場合のみ）
				if (!_originalTileOwners.ContainsKey(targetTileKey))
				{
					string originalOwner = _tileManager.GetOwnerInfo(targetTileKey) ?? "";
					_originalTileOwners[targetTileKey] = originalOwner;
				}

				// 訪問済みリストに追加
				if (!_visitedTiles.Contains(targetTileKey))
				{
					_visitedTiles.Add(targetTileKey);
				}

				// タイルの所有権を取得（通った経路を所有）
				TileType tileType = GetClickedTileTypeForPlayer(currentPlayerId);
				_displayTileState.ChangeClickableTileColor(targetTileKey, tileType);

				// 残りの移動可能距離を減らす
				_remainingMoveDistance--;

				// 移動可能範囲を再計算して表示
				_displayTileState.DisplayClicableTile(targetTileKey, _remainingMoveDistance);
				_displayTileState.HighlightMarker(_currentSelectedTileKey);

				Debug.Log($"選択移動: {targetTileKey}, 残り移動可能距離: {_remainingMoveDistance}");
			}
			else
			{
				// 候補が見つからなかった場合
				Debug.Log($"指定方向にクリック可能なタイルが見つかりません");
			}
		}

		/// <summary>
		/// プレイヤーIDからチームに応じた選択済みタイルの色（TileType）を取得する
		/// </summary>
		private TileType GetClickedTileTypeForPlayer(string playerId)
		{
			var playerInfo = _playerManager.GetPlayerInfoByUserId(playerId);
			if (playerInfo == null)
			{
				// プレイヤー情報が取得できない場合はデフォルトでTeamA
				return TileType.clickedTeamA;
			}

			// チーム名に応じて適切な色を返す
			return playerInfo.TeamName == "TeamA" ? TileType.clickedTeamA : TileType.clickedTeamB;
		}

		/// <summary>
		/// 来た道を戻る処理
		/// </summary>
		/// <param name="targetTileKey">戻り先のタイルキー</param>
		private void GoBackToPreviousTile(string targetTileKey)
		{
			// 現在のタイル（_visitedTiles の最後）を取得
			string currentTileKey = _visitedTiles[_visitedTiles.Count - 1];

			// 現在のタイルを _visitedTiles から削除
			_visitedTiles.RemoveAt(_visitedTiles.Count - 1);

			// 元の所有者を復元
			if (_originalTileOwners.TryGetValue(currentTileKey, out var originalOwner))
			{
				// 元の所有者に戻す
				_tileManager.SetTileOwner(currentTileKey, originalOwner);

				// 所有者から色を逆算して元の色に戻す（IsClickableチェックをスキップ）
				TileType originalType = GetTileTypeFromOwner(originalOwner);
				_tileManager.ChangesetColor(currentTileKey, originalType);

				// 保存していた所有者を削除
				_originalTileOwners.Remove(currentTileKey);
			}
			else
			{
				// 元の所有者が保存されていない場合はデフォルトに戻す
				_tileManager.SetTileOwner(currentTileKey, "");
				_tileManager.ChangesetColor(currentTileKey, TileType.Empty);
			}

			// 移動可能距離を増やす
			_remainingMoveDistance++;

			// 進行方向を計算（現在位置から戻り先へのベクトル）
			var currentTilePos = _tileManager.GetTileData(currentTileKey)?.Position;
			var targetTilePos = _tileManager.GetTileData(targetTileKey)?.Position;
			Vector3 direction = Vector3.zero;

			if (currentTilePos != null && targetTilePos != null)
			{
				direction = (targetTilePos.ToVector3() - currentTilePos.ToVector3()).normalized;
			}

			// プレイヤーを戻り先のタイルに移動させる（進行方向を向く）
			string currentPlayerId = _gameStateController.GetCurrentPlayerId();
			_playerManager.MovePlayerToTileKeyWithDirection(currentPlayerId, targetTileKey, direction);

			// プレイヤー移動シグナルを発火（Backspace時のタイル選択）
			var playerPosition = _playerManager.GetPlayerPositionByUserId(currentPlayerId);
			if (playerPosition.HasValue)
			{
				_signalBus.Fire(new PlayerMovedSignal(currentPlayerId, targetTileKey, playerPosition.Value));
			}

			// 選択を更新
			_currentSelectedTileKey = targetTileKey;

			// 移動可能範囲を再計算して表示（マーカーも再表示される）
			_displayTileState.DisplayClicableTile(targetTileKey, _remainingMoveDistance);
			_displayTileState.HighlightMarker(_currentSelectedTileKey);

			Debug.Log($"戻る: {targetTileKey}, 残り移動可能距離: {_remainingMoveDistance}");
		}

		/// <summary>
		/// 選択を決定してプレイヤーを移動する
		/// </summary>
		private void ConfirmSelection()
		{
				if (_currentSelectedTileKey == null)
						return;

				// 現在のターンのプレイヤーIDを取得
				string currentPlayerId = _gameStateController.GetCurrentPlayerId();

				// プレイヤーのチームに応じた色を取得
				TileType tileType = GetClickedTileTypeForPlayer(currentPlayerId);

				// 訪問した全てのタイルの所有者と色を現在のプレイヤーに変更
				foreach (var tileKey in _visitedTiles)
				{
					_tileManager.SetTileOwner(tileKey, currentPlayerId);
					// IsClickableチェックをスキップして直接色を変更
					_tileManager.ChangesetColor(tileKey, tileType);
				}

				Debug.Log($"選択決定: {_currentSelectedTileKey} ({_gameStateController.GetCurrentPlayerName()}), 訪問したタイル数: {_visitedTiles.Count}");

				// 移動を確定（GameStateControllerで状態もIdleに戻る）
				_gameStateController.ConfirmMove();

				// 選択モードを終了
				EndSelectionMode();
		}


		/// <summary>
		/// 選択モードをキャンセルする
		/// </summary>
		public void CancelSelectionMode()
		{
			Debug.Log("選択モードをキャンセル");

			// GameStateControllerでキャンセル処理
			_gameStateController.CancelSelection();

			// 選択モードを終了
			EndSelectionMode();
		}

		/// <summary>
		/// 選択モードを終了する
		/// </summary>
		private void EndSelectionMode()
		{
			// ハイライトをクリア
			_displayTileState.ClearHighlight();

			// マーカーをクリア
			_displayTileState.ClearDisplayMarkers();
			_tileManager.ClearClickableTiles();

			// 状態をリセット
			_currentSelectedTileKey = null;
			_remainingMoveDistance = 0;
			_visitedTiles.Clear();
			_originalTileOwners.Clear();
			// 状態はGameStateControllerで管理されるため、ここでは設定しない
		}

		/// <summary>
		/// 所有者IDから対応するタイルタイプ（色）を取得する
		/// </summary>
		/// <param name="ownerId">所有者ID</param>
		/// <returns>所有者に対応するタイルタイプ</returns>
		private TileType GetTileTypeFromOwner(string ownerId)
		{
			// 所有者が空の場合はEmpty
			if (string.IsNullOrEmpty(ownerId))
			{
				return TileType.Empty;
			}

			// 所有者のプレイヤー情報を取得
			var playerInfo = _playerManager.GetPlayerInfoByUserId(ownerId);
			if (playerInfo == null)
			{
				return TileType.Empty;
			}

			// チームに応じた色を返す
			return playerInfo.TeamName == "TeamA" ? TileType.clickedTeamA : TileType.clickedTeamB;
		}

		/// <summary>
		/// 現在の選択状態を取得する
		/// </summary>
		public bool IsSelectionModeActive => _gameStateController.GetCurrentState() == GameState.Selecting;

		/// <summary>
		/// 現在選択中のタイルキーを取得する
		/// </summary>
		public string CurrentSelectedTileKey => _currentSelectedTileKey;
	}
}
