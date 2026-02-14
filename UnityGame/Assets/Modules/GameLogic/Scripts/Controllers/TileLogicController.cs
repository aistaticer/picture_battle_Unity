using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared.helper;
using UnityGame.Assets.Modules.UserSystem.Domain;
using Zenject;
using UnityGame.Assets.Modules.UserSystem;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
    public class TileLogicController : IInitializable, IDisposable
    {
        private readonly SignalBus _signalBus;
        private readonly TileManager _tileManager;
        private readonly TileActionService _tileActionService;
        private readonly PlayerManager _playerManager;

        public TileLogicController(SignalBus signalBus,TileManager tileManager, TileActionService tileActionService, PlayerManager playerManager)
        {
            _signalBus = signalBus;
            _tileManager = tileManager;
            _tileActionService = tileActionService;
            _playerManager = playerManager;
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
                // Alice (player001)の位置を取得
                string playerTileKey = _playerManager.GetPlayerTileKeyByUserId("player001");

                if (playerTileKey != null)
                {
                    // プレイヤーの位置からクリックした位置までの最短経路を取得
                    var path = _tileActionService.FindShortestPath(playerTileKey, tileKey);

                    // 経路上の全タイルの色を変更
                    foreach (var item in path)
                    {
                        _tileManager.ChangesetColor(item.Key, TileType.clickedTeamA);
                    }
                }

                //_tileManager.ChangesetColor(tileKey, TileType.clickableTeamA);
			}

            // 例: service.DoSomething(tile);
        }

        public void Dispose() => _signalBus.Unsubscribe<TileClickedSignal>(OnTileClicked);
    }

}