using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.TileSystem.Configs;
using picture_game_view.Assets.Modules.Shared;


public class TileSystemInstraller : MonoInstaller
{
    public override void InstallBindings()
    {

        // // TileMaterialSetを取得してDI登録
        // var tileMaterialSet = Resources.Load<TileMaterialSet>("MatSet/TileMaterialSet");
        // Container.BindInstance(tileMaterialSet).AsSingle();

        InstallerHelper.BindSO<TIleObjSet>(Container,ResourcesPath.ObjSetFolderPath);
        InstallerHelper.BindSO<TileMaterialSet>(Container,ResourcesPath.MatSetFolderPath);
        InstallerHelper.BindClass<TileSystem.Scripts.StartUp>(Container);
        InstallerHelper.BindClass<TileManager>(Container);



        // Type[] orderedTypes = new Type[]
        // {
        //     // typeof(TIleObjSet),
        //     // typeof(TileMaterialSet),
        //     typeof(TileSystem.Scripts.StartUp)
        // };

        // foreach (var type in orderedTypes)
        // {
        //     Debug.Log(type);

        //     if (typeof(MonoBehaviour).IsAssignableFrom(type))
        //     {
        //         // 新しい空の GameObject を生成して型名を設定
        //         var go = new GameObject(type.Name);

        //         // Zenject を使って MonoBehaviour コンポーネントを GameObject に追加し、依存注入も行う
        //         var component = Container.InstantiateComponent(type, go);

        //         // 作成したコンポーネントを DI コンテナに登録
        //         // 他のクラスから [Inject] で取得できるようになる
        //         //Container.BindInstance(component).AsSingle();
        //         Container.Bind(type).FromInstance(component).AsSingle();

        //     }
        //     else if (typeof(ScriptableObject).IsAssignableFrom(type))
		// 	{
        //         var asset = Resources.Load(type.Name) as ScriptableObject;
        //         if (asset != null)
        //             Container.BindInstance(asset).AsSingle();
		// 	}

        //     else
        //     {
        //         // Type から純粋クラスのインスタンスを生成
        //         var instance = Activator.CreateInstance(type);

        //         // 作成したインスタンスを Zenject に登録
        //         // これにより他のクラスで [Inject] 可能になる
        //         Container.BindInstance(instance).AsSingle();

        //         // 生成直後に依存を注入
        //         // [Inject] 属性が付いたフィールドやメソッドに値がセットされる
        //         Container.Inject(instance);
        //     }

        // }

 


    }
}
