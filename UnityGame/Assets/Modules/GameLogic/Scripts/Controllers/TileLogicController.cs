using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared.helper;
using Zenject;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
    public class TileLogicController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;

        public TileLogicController(SignalBus signalBus) { _signalBus = signalBus; }

        public void Initialize()
        {
            _signalBus.Subscribe<TileClickedSignal>(OnTileClicked); 
        }

        internal void OnTileClicked(TileClickedSignal signal)
        {
            // ここでゲームルールに従って状態を変える（移動可能判定、選択状態更新等）
            var tile = signal.TileHighlighter.tile;

            Debug.Log(tile);
            Debug.Log("aaaああ");
            // 例: service.DoSomething(tile);
        }

        public void Dispose() => _signalBus.Unsubscribe<TileClickedSignal>(OnTileClicked);
    }

}