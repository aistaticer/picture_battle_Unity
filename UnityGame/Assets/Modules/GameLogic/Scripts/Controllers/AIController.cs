using System.Collections.Generic;
using UnityEngine;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Services;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
	/// <summary>
	/// AI（敵）の自動移動を制御するコントローラー
	/// ターン制ではなく、独立したタイミングで移動する
	/// </summary>
	public class AIController : ITickable
	{
		private readonly PlayerManager _playerManager;
		private readonly TileManager _tileManager;
		private readonly PlayerMover _playerMover;
		private readonly TileActionService _tileActionService;

		private const string AI_PLAYER_ID = "player002"; // Bob
		private const int MAX_MOVE_DISTANCE = 4;
		private const float MOVE_INTERVAL = 3.0f; // 3秒ごとに移動を試みる

		private float _moveTimer = 0f;
		private bool _isExecutingMove = false;
		private List<TileData> _currentPath = null;
		private int _currentPathIndex = 0;

		// BFS用のオフセット（4方向）
		private static readonly Vector3Int[] offsets4 = new Vector3Int[]
		{
			new Vector3Int( 1, 0,  0), // +X
			new Vector3Int(-1, 0,  0), // -X
			new Vector3Int( 0, 0,  1), // +Z
			new Vector3Int( 0, 0, -1)  // -Z
		};

		public AIController(
			PlayerManager playerManager,
			TileManager tileManager,
			PlayerMover playerMover,
			TileActionService tileActionService)
		{
			_playerManager = playerManager;
			_tileManager = tileManager;
			_playerMover = playerMover;
			_tileActionService = tileActionService;
		}

		public void Tick()
		{
			// スタン中は移動しない（状態システムを使用）
			var playerState = _playerManager.GetPlayerActionState(AI_PLAYER_ID);
			if (playerState == PlayerActionState.Stunned)
			{
				return;
			}

			// 現在移動中の場合は、次のタイルへの移動を処理
			if (_isExecutingMove)
			{
				// プレイヤーが移動中でなければ、次のタイルに移動
				// AIプレイヤーのGameObjectを取得してチェック
				var aiGameObject = _playerManager.GetPlayerGameObjectByUserId(AI_PLAYER_ID);
				if (aiGameObject != null && _playerMover != null && !_playerMover.IsMoving(aiGameObject.transform))
				{
					MoveToNextTileInPath();
				}
				return;
			}

			// 新しい移動を開始するタイミングかチェック
			_moveTimer += Time.deltaTime;
			if (_moveTimer >= MOVE_INTERVAL)
			{
				_moveTimer = 0f;
				StartNewMove();
			}
		}

		/// <summary>
		/// 新しい移動を開始する
		/// </summary>
		private void StartNewMove()
		{
			// AIの現在位置を取得
			string currentTileKey = _playerManager.GetPlayerTileKeyByUserId(AI_PLAYER_ID);
			if (currentTileKey == null)
			{
				Debug.LogWarning($"AI ({AI_PLAYER_ID}) の位置が見つかりません");
				return;
			}

			// 移動可能な範囲内のタイルを取得
			var reachableTiles = GetReachableTiles(currentTileKey, MAX_MOVE_DISTANCE);
			if (reachableTiles.Count == 0)
			{
				Debug.Log("AIの移動可能なタイルがありません");
				return;
			}

			// ランダムに目標タイルを選択
			int randomIndex = Random.Range(0, reachableTiles.Count);
			string targetTileKey = reachableTiles[randomIndex];

			// 最短経路を計算
			_currentPath = _tileActionService.FindShortestPath(currentTileKey, targetTileKey);
			if (_currentPath == null || _currentPath.Count <= 1)
			{
				Debug.Log("AIの経路が見つかりませんでした");
				return;
			}

			// 移動を開始（インデックス1から開始 = 現在地をスキップ）
			_currentPathIndex = 1;
			_isExecutingMove = true;

			Debug.Log($"AI移動開始: {currentTileKey} → {targetTileKey} (経路長: {_currentPath.Count})");

			// 最初の移動を開始
			MoveToNextTileInPath();
		}

		/// <summary>
		/// 経路上の次のタイルに移動する
		/// </summary>
		private void MoveToNextTileInPath()
		{
			// 経路の終端に到達したかチェック
			if (_currentPath == null || _currentPathIndex >= _currentPath.Count)
			{
				// 移動完了
				_isExecutingMove = false;
				_currentPath = null;
				Debug.Log("AI移動完了");
				return;
			}

			var previousTile = _currentPath[_currentPathIndex - 1];
			var nextTile = _currentPath[_currentPathIndex];

			// 進行方向を計算
			Vector3 direction = (nextTile.Position.ToVector3() - previousTile.Position.ToVector3()).normalized;

			// プレイヤーのGameObjectを取得
			var playerGameObject = _playerManager.GetPlayerGameObjectByUserId(AI_PLAYER_ID);
			if (playerGameObject == null)
			{
				Debug.LogWarning($"AI ({AI_PLAYER_ID}) のGameObjectが見つかりません");
				_isExecutingMove = false;
				return;
			}

			// ジャンプアニメーションで移動
			_playerMover.JumpToPosition(
				playerGameObject.transform,
				nextTile.Position.ToVector3(),
				direction,
				0.3f
			);

			// PlayerManagerの内部状態を更新（PlayerInfoState.Positionを更新）
			// アニメーションは進行中だが、最終的にこの位置に到達するため先に更新
			_playerManager.MovePlayerToPosition(AI_PLAYER_ID, nextTile.Position.ToVector3());

			// タイルの所有権と色を変更
			_tileManager.SetTileOwner(nextTile.Key, AI_PLAYER_ID);
			_tileManager.ChangesetColor(nextTile.Key, TileType.clickedTeamB);

			Debug.Log($"AI移動: {nextTile.Key}");

			// 次のインデックスに進む
			_currentPathIndex++;
		}

		/// <summary>
		/// 指定されたタイルから指定距離以内の到達可能なタイルを取得（BFS）
		/// マーカーは作成せず、タイルキーのリストのみを返す
		/// </summary>
		/// <param name="startTileKey">開始タイルのキー</param>
		/// <param name="maxDistance">最大移動距離</param>
		/// <returns>到達可能なタイルキーのリスト</returns>
		private List<string> GetReachableTiles(string startTileKey, int maxDistance)
		{
			var reachableTiles = new List<string>();

			var startTile = _tileManager.GetTileData(startTileKey);
			if (startTile == null)
				return reachableTiles;

			// BFS用のデータ構造
			var visited = new HashSet<string>();
			var queue = new Queue<(TileData tile, int distance)>();

			// 開始地点をキューに追加
			queue.Enqueue((startTile, 0));
			visited.Add(startTile.Key);

			// BFSで探索
			while (queue.Count > 0)
			{
				var (current, distance) = queue.Dequeue();

				// 最大距離以内のタイルを探索
				if (distance < maxDistance)
				{
					// 4方向の隣接タイルを探索
					foreach (var offset in offsets4)
					{
						int newX = (int)current.Position.x + offset.x;
						int newY = (int)current.Position.y + offset.y;
						int newZ = (int)current.Position.z + offset.z;

						var adjacentKey = $"{newX}-{newY}-{newZ}";

						// タイルが存在するか確認
						var nextTileData = _tileManager.GetTileData(adjacentKey);
						if (nextTileData == null)
							continue;

						// まだ訪問していないタイル
						if (!visited.Contains(adjacentKey))
						{
							visited.Add(adjacentKey);

							// 移動可能なタイルとして追加（距離1以上のタイル）
							if (distance + 1 <= maxDistance && distance + 1 > 0)
							{
								reachableTiles.Add(adjacentKey);
							}

							// 次の探索対象としてキューに追加
							queue.Enqueue((nextTileData, distance + 1));
						}
					}
				}
			}

			return reachableTiles;
		}
	}
}
