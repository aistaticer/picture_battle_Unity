using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using picture_game_view.Assets.Modules.GameLogic.Scripts;
using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;
using picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers;
using UnityGame.Assets.Modules.GameLogic.Scripts.Controllers;
using UnityGame.Assets.Modules.GameLogic.Scripts.Services;

public class GameLogicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // SignalBus 自体のバインド（StartUpから移動）
        SignalBusInstaller.Install(Container);

        // シグナル型の登録
        Container.DeclareSignal<TileClickedSignal>();
        Container.DeclareSignal<PlayerMovedSignal>();

        InstallerHelper.BindClass<StartUp>(Container);
        InstallerHelper.BindClass<TileSpawner>(Container);
        InstallerHelper.BindClass<TileActionService>(Container);
        InstallerHelper.BindClass<DisplayTileState>(Container);
        InstallerHelper.BindMono<MapController>(Container);
        InstallerHelper.BindClass<TileLogicController>(Container);

        // GameStateController を IInitializable としてバインド
        Container.BindInterfacesAndSelfTo<GameStateController>().AsSingle();

        // SelectionController を ITickable, IInitializable としてバインド
        Container.BindInterfacesAndSelfTo<SelectionController>().AsSingle();

        // AbilityFactoryをバインド
        Container.Bind<AbilityFactory>().AsSingle();

        // AbilitySetupService: プレイヤー生成後にアビリティを紐づける（IInitializable）
        Container.BindInterfacesAndSelfTo<UnityGame.Assets.Modules.GameLogic.Scripts.Services.AbilitySetupService>().AsSingle();

        // AbilityManagerをバインド（プレイヤーのアビリティに処理を委譲）
        Container.Bind<AbilityManager>().AsSingle();

        // AIController を ITickable としてバインド（敵の自動移動）
        Container.BindInterfacesAndSelfTo<AIController>().AsSingle();

        // AbilityCooldownUI を ITickable としてバインド（クールダウンUI更新）
        // GhostTouch用UI
        Container.BindInterfacesTo<AbilityCooldownUI>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .OnInstantiated<AbilityCooldownUI>((ctx, ui) =>
                ui.Initialize("player001", "GhostTouch", 5.0f))
            .NonLazy();

        // RoadBlock用UI
        Container.BindInterfacesTo<AbilityCooldownUI>()
            .FromNewComponentOnNewGameObject()
            .AsCached()
            .OnInstantiated<AbilityCooldownUI>((ctx, ui) =>
                ui.Initialize("player001", "RoadBlock", 20.0f))
            .NonLazy();

        // PlayerManagerはPlayerSystemStartUpで動的に生成してバインドされる
    }
}
