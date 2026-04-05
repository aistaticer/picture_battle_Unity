using UnityEngine;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// プレイヤー生成後にアビリティインスタンスを各PlayerStateに紐づけるサービス
    /// IInitializableとして登録することでInjectフェーズ（PlayerSystemStartUp）完了後に実行される
    /// </summary>
    public class AbilitySetupService : IInitializable
    {
        private readonly PlayerManager _playerManager;
        private readonly AbilityFactory _abilityFactory;

        [Inject]
        public AbilitySetupService(PlayerManager playerManager, AbilityFactory abilityFactory)
        {
            _playerManager = playerManager;
            _abilityFactory = abilityFactory;
        }

        public void Initialize()
        {
            var players = _playerManager.GetAllPlayers();
            foreach (var playerState in players)
            {
                foreach (var abilityName in playerState.Info.Abilities)
                {
                    var ability = _abilityFactory.Create(abilityName, playerState.Info.UserId);
                    playerState.AddAbility(ability);
                }
                Debug.Log($"[AbilitySetupService] {playerState.Info.UserId}: {playerState.Abilities.Count} 個のアビリティを紐づけました");
            }
        }
    }
}
