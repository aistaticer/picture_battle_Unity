using UnityEngine;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// PlayerMovedSignalを受け取り、移動したプレイヤーのアビリティに処理を委譲する
    /// </summary>
    public class AbilityManager
    {
        private readonly PlayerManager _playerManager;

        [Inject]
        public AbilityManager(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        /// <summary>
        /// プレイヤー移動シグナルのハンドラ
        /// 移動したプレイヤーのPlayerStateからアビリティを取得して実行
        /// </summary>
        public void OnPlayerMoved(PlayerMovedSignal signal)
        {
            var playerState = _playerManager.GetPlayerStateByUserId(signal.UserId);
            if (playerState == null)
            {
                Debug.LogWarning($"[AbilityManager] プレイヤーが見つかりません: {signal.UserId}");
                return;
            }

            foreach (var ability in playerState.Abilities)
            {
                ability.OnPlayerMoved(signal);
            }
        }
    }
}
