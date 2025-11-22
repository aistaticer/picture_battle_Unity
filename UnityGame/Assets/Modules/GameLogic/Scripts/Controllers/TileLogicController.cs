using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared.helper;
using UnityGame.Assets.Modules.UserSystem.Domain;
using Zenject;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
    public class TileLogicController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly TileManager _tileManager;
        private readonly TileActionService _tileActionService;
        private readonly UserState _userState;

        public TileLogicController(SignalBus signalBus,TileManager tileManager, TileActionService tileActionService, UserState userState)
        {
            _signalBus = signalBus;
            _tileManager = tileManager;
            _tileActionService = tileActionService;
            _userState = userState;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<TileClickedSignal>(OnTileClicked); 
        }

        internal void OnTileClicked(TileClickedSignal signal)
        {
            // ここでゲームルールに従って状態を変える（移動可能判定、選択状態更新等）
            var tileKey = signal.TileHighlighter.TileKey;

			if (_tileManager.GetOwnerInfo(tileKey) == "testId")
			{
                var a = _tileActionService.FindShortestPath(tileKey, "0-0-0");

                foreach (var item in a)
                {
                    _tileManager.ChangesetColor(item.Key, TileType.clickedTeamA);
                }

                //_tileManager.ChangesetColor(tileKey, TileType.clickableTeamA);
			}

            // 例: service.DoSomething(tile);
        }

        public void Dispose() => _signalBus.Unsubscribe<TileClickedSignal>(OnTileClicked);
    }

}