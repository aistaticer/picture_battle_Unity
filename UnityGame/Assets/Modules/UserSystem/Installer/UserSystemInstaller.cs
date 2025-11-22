using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;

namespace UnityGame.Assets.Modules.UserSystem.Installer
{
    public class UserSystemInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            // InstallerHelper.BindSO<TIleObjSet>(Container,ResourcesPath.ObjSetFolderPath);
            // InstallerHelper.BindSO<TileMaterialSet>(Container,ResourcesPath.MatSetFolderPath);
            InstallerHelper.BindClass<StartUp>(Container);
            // InstallerHelper.BindClass<TileManager>(Container);
        }
    }
}