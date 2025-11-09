using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers;
using UnityGame.Assets.Modules.GameLogic.Scripts.Controllers;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts
{
    public class StartUp
    {
        private DiContainer Container;

        GameLogicContext GameLogicContext;

        [Inject]
        public void Construct(DiContainer diContainer){
            Container = diContainer;

            // SignalBus 自体のバインド
            SignalBusInstaller.Install(Container);

            // シグナル型の登録
            Container.DeclareSignal<TileClickedSignal>();

            // Signal購読の紐付け
            // SignalBus の「購読」と「ハンドラメソッド」を結びつける専用構文
            // TileClickedSignalが発火したらTileLogicController.OnTileClickedメソッドを自動的に呼ぶと定義してくれている
            Container.BindSignal<TileClickedSignal>()
                .ToMethod<TileLogicController>(x => x.OnTileClicked)
                .FromResolve();

            // ITickable リストに登録
            // InputController.Tick() が毎フレーム呼ばれるようにする
            Container.BindInterfacesAndSelfTo<InputController>().AsSingle();

            // Context作成。必要なプロパティを設定。DI登録。
            GameLogicContext = new GameLogicContext();
        }
    }
}