using UnityEngine;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Services;
using UnityGame.Assets.Modules.UserSystem;

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

		// 現在選択中のタイルキー
		private string _currentSelectedTileKey = null;

		public SelectionController(
			PlayerManager playerManager,
			DisplayTileState displayTileState,
			TileManager tileManager,
			GameStateController gameStateController)
		{
			_playerManager = playerManager;
			_displayTileState = displayTileState;
			_tileManager = tileManager;
			_gameStateController = gameStateController;
		}

		public void Initialize()
		{
			// 初期化処理（必要に応じて）
		}

		public void Tick()
		{
			// 選択モードがアクティブでない場合のみ、スペースキーで選択モードを開始
			if (_gameStateController.CanStartSelection() && Input.GetKeyDown(KeyCode.Space))
			{
				// 現在のターンのプレイヤーで選択モードを開始
				string currentPlayerId = _gameStateController.GetCurrentPlayerId();
				StartSelectionMode(currentPlayerId);
			}

			// 選択モード中の処理
			if (_gameStateController.GetCurrentState() == GameState.Selecting)
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

			// プレイヤーの位置から移動可能な範囲を表示
			_displayTileState.DisplayClicableTile(playerTileKey, 7);

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

			// ========== ステップ1: 現在位置の取得 ==========
			var currentTileData = _tileManager.GetTileData(_currentSelectedTileKey);
			if (currentTileData == null)
				return;

			int currentX = (int)currentTileData.Position.x;
			int currentZ = (int)currentTileData.Position.z;

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

			// ========== ステップ5: 選択を実行 ==========
			if (targetTileKey != null)
			{
				// 新しいタイルを選択してハイライト
				_currentSelectedTileKey = targetTileKey;
				_displayTileState.HighlightMarker(_currentSelectedTileKey);

				Debug.Log($"選択移動: {targetTileKey}");
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
		/// 選択を決定して色を変更する
		/// </summary>
		private void ConfirmSelection()
		{
				if (_currentSelectedTileKey == null)
						return;

				// 現在のターンのプレイヤーIDを取得
				string currentPlayerId = _gameStateController.GetCurrentPlayerId();

				// プレイヤーのチームに応じた色を取得
				TileType tileType = GetClickedTileTypeForPlayer(currentPlayerId);

				// 選択されたタイルの色を最終的な色に変更
				_displayTileState.ChangeClickableTileColor(_currentSelectedTileKey, tileType);

				// 現在のターンのプレイヤーを選択されたタイルに移動
				_playerManager.MovePlayerToTileKey(currentPlayerId, _currentSelectedTileKey);

				Debug.Log($"選択決定: {_currentSelectedTileKey} ({_gameStateController.GetCurrentPlayerName()})");

				// 移動を確定し、ターンを切り替え（GameStateControllerで状態もIdleに戻る）
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

			_currentSelectedTileKey = null;
			// 状態はGameStateControllerで管理されるため、ここでは設定しない
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
