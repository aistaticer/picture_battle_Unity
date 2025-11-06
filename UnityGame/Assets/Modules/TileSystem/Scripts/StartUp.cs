using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;


namespace TileSystem.Scripts
{
    public class StartUp
    {
        private DiContainer _container;

        [Inject] // ← コンストラクタインジェクション
        public void Construct(DiContainer diContainer)
        {

            // var tileManager = new TileManager();
            // _container.BindInstance(tileManager).AsSingle();
        }
    }
}
