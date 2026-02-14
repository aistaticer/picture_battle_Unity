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

public class GameLogicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Debug.Log("aaaa");
        // SignalBus 自体のバインド（StartUpから移動）
        SignalBusInstaller.Install(Container);

        // シグナル型の登録
        Container.DeclareSignal<TileClickedSignal>();

        InstallerHelper.BindClass<StartUp>(Container);
        InstallerHelper.BindClass<TileSpawner>(Container);
        InstallerHelper.BindClass<TileActionService>(Container);
        InstallerHelper.BindMono<MapController>(Container);
        InstallerHelper.BindClass<TileLogicController>(Container);

        // PlayerManagerはPlayerSystemStartUpで動的に生成してバインドされる
    }
}
