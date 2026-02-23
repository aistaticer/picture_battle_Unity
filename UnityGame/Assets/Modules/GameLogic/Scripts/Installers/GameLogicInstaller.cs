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

        // アビリティシステムのバインド
        // GhostTouchAbilityをバインド（具象型 + IPlayerAbilityインターフェース）
        Container.BindInterfacesAndSelfTo<GhostTouchAbility>().AsSingle();

        // AbilityManagerをバインド（全アビリティを統括管理）
        Container.Bind<AbilityManager>().AsSingle();

        // AIController を ITickable としてバインド（敵の自動移動）
        Container.BindInterfacesAndSelfTo<AIController>().AsSingle();

        // PlayerManagerはPlayerSystemStartUpで動的に生成してバインドされる
    }
}
