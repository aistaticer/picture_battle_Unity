using UnityEngine;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// アビリティ名とオーナーUserIdからIPlayerAbilityインスタンスを生成するファクトリ
    /// </summary>
    public class AbilityFactory
    {
        private readonly PlayerManager _playerManager;
        private readonly TileManager _tileManager;

        [Inject]
        public AbilityFactory(PlayerManager playerManager, TileManager tileManager)
        {
            _playerManager = playerManager;
            _tileManager = tileManager;
        }

        /// <summary>
        /// アビリティ名とオーナーUserIdからインスタンスを生成する
        /// </summary>
        public IPlayerAbility Create(string abilityName, string ownerId)
        {
            switch (abilityName)
            {
                case "GhostTouch":
                    return new GhostTouchAbility(_playerManager, _tileManager, ownerId);
                case "RoadBlock":
                    return new RoadBlockAbility(_playerManager, _tileManager, ownerId);
                default:
                    Debug.LogWarning($"[AbilityFactory] 未知のアビリティ: {abilityName}");
                    return null;
            }
        }
    }
}
