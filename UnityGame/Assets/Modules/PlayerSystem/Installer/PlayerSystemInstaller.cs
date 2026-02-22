using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;

public class PlayerSystemInstaller : MonoInstaller
{
    [SerializeField] private PlayerModelSet _playerModelSet;

    public override void InstallBindings()
    {
        Debug.Log("aaaasss");

        // PlayerModelSetをバインド
        Container.BindInstance(_playerModelSet).AsSingle();

        // Movementをバインド
        Container.Bind<Movement>().AsSingle();

        // PlayerMoverをバインド（汎用的なジャンプ移動コンポーネント）
        InstallerHelper.BindMono<PlayerMover>(Container);

        // PlayerManagerをバインド（StartUpより先に）
        InstallerHelper.BindClass<PlayerManager>(Container);

        // PlayerSystemStartUpをバインド
        InstallerHelper.BindClass<PlayerSystemStartUp>(Container);
    }
}
