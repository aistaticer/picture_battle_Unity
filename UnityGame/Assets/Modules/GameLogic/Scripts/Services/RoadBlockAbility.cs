using UnityEngine;
using UnityGame.Assets.Modules.GameLogic.Scripts;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
	/// <summary>
	/// RoadBlockアビリティ: プレイヤーの前方3マスを10秒間ブロック（移動不可・赤色）にする
	/// </summary>
	public class RoadBlockAbility : IPlayerAbility
	{
		private readonly PlayerManager _playerManager;
		private readonly TileManager _tileManager;
		private readonly string _ownerId;

		private const float BLOCK_DURATION = 10.0f;
		private const float COOLDOWN_DURATION = 20.0f;
		private const int BLOCK_RANGE = 3;

		public string AbilityName => "RoadBlock";
		public string OwnerId => _ownerId;

		public RoadBlockAbility(PlayerManager playerManager, TileManager tileManager, string ownerId)
		{
			_playerManager = playerManager;
			_tileManager = tileManager;
			_ownerId = ownerId;
		}

		public void OnPlayerMoved(PlayerMovedSignal signal)
		{
			// クールダウンチェック
			if (_playerManager.IsAbilityOnCooldown(signal.UserId, AbilityName))
				return;

			// プレイヤー状態取得
			var playerState = _playerManager.GetPlayerStateByUserId(signal.UserId);
			if (playerState == null) return;

			// 移動方向取得（LastMovementDirection）
			Vector3 direction = playerState.LastMovementDirection;

			// グリッド方向に変換（X/Z軸のみ、四捨五入）
			Vector3Int gridDirection = new Vector3Int(
				Mathf.RoundToInt(direction.x),
				0,
				Mathf.RoundToInt(direction.z)
			);

			// 現在位置から前方3タイルを計算
			var currentTileData = _tileManager.GetTileData(signal.TileKey);
			if (currentTileData == null) return;

			float unblockTime = Time.time + BLOCK_DURATION;
			int blockedCount = 0;

			for (int i = 1; i <= BLOCK_RANGE; i++)
			{
				int x = (int)currentTileData.Position.x + gridDirection.x * i;
				int y = (int)currentTileData.Position.y;
				int z = (int)currentTileData.Position.z + gridDirection.z * i;
				string tileKey = $"{x}-{y}-{z}";

				// タイルが存在する場合のみブロック
				if (_tileManager.GetTileData(tileKey) != null)
				{
					_tileManager.BlockTile(tileKey, unblockTime);
					blockedCount++;
				}
			}

			// クールダウン開始
			_playerManager.SetAbilityCooldown(signal.UserId, AbilityName, Time.time + COOLDOWN_DURATION);

			Debug.Log($"[RoadBlock] {signal.UserId} blocked {blockedCount} tiles for {BLOCK_DURATION}s");
		}
	}
}
