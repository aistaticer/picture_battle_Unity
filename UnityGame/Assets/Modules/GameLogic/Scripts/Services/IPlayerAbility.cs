namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// プレイヤーアビリティの共通インターフェース
    /// </summary>
    public interface IPlayerAbility
    {
        /// <summary>
        /// アビリティの名前（player.jsonのabilitiesと一致させる）
        /// </summary>
        string AbilityName { get; }

        /// <summary>
        /// プレイヤーが移動した時に呼ばれる
        /// </summary>
        void OnPlayerMoved(PlayerMovedSignal signal);
    }
}
