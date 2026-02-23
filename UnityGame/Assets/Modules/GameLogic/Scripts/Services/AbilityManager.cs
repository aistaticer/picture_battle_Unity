using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// プレイヤーアビリティの統括管理クラス
    /// PlayerMovedSignalを受け取り、該当するアビリティに処理を委譲する
    /// </summary>
    public class AbilityManager
    {
        private readonly PlayerManager _playerManager;
        private readonly List<IPlayerAbility> _abilities;

        [Inject]
        public AbilityManager(
            PlayerManager playerManager,
            List<IPlayerAbility> abilities)
        {
            _playerManager = playerManager;
            _abilities = abilities;

            Debug.Log($"【AbilityManager】初期化完了。登録アビリティ数: {_abilities.Count}");
            foreach (var ability in _abilities)
            {
                Debug.Log($"  - {ability.AbilityName}");
            }
        }

        /// <summary>
        /// プレイヤー移動シグナルのハンドラ
        /// 移動したプレイヤーが持つアビリティを確認し、該当するアビリティを実行
        /// </summary>
        public void OnPlayerMoved(PlayerMovedSignal signal)
        {
            // 移動したプレイヤーの情報を取得
            var playerInfo = _playerManager.GetPlayerInfoByUserId(signal.UserId);
            if (playerInfo == null)
            {
                Debug.LogWarning($"【AbilityManager】プレイヤー情報が見つかりません: {signal.UserId}");
                return;
            }

            // プレイヤーが持つ各アビリティを実行
            foreach (var ability in _abilities)
            {
                if (playerInfo.HasAbility(ability.AbilityName))
                {
                    ability.OnPlayerMoved(signal);
                }
            }
        }
    }
}
