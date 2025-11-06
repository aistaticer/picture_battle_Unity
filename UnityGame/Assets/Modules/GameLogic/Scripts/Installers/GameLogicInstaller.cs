using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using picture_game_view.Assets.Modules.GameLogic.Scripts;
using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;

public class GameLogicInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallerHelper.BindClass<StartUp>(Container);
        InstallerHelper.BindClass<TileSpawner>(Container);
        InstallerHelper.BindMono<MapController>(Container);
    }
}
