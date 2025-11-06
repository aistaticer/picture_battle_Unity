using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


namespace picture_game_view.Assets.Modules.GameLogic.Scripts
{
    public class StartUp
    {
        private DiContainer Container;

        GameLogicContext GameLogicContext;

        [Inject]
        public void Construct(DiContainer diContainer){
            Container = diContainer;

            // Context作成。必要なプロパティを設定。DI登録。
            GameLogicContext = new GameLogicContext();
        }
    }
}