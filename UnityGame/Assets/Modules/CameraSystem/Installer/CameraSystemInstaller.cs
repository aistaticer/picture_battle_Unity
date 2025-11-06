using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;

public class CameraSystemInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        InstallerHelper.BindSO<CameraObjSet>(Container,ResourcesPath.ObjSetFolderPath);
        InstallerHelper.BindClass<CameraSystem.StartUp>(Container);
    }
}
