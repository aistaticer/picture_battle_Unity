using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using System.Threading.Tasks;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    public class DisplayTileState
    {
        private TileManager _tileManager;

        [Inject]
        public void Construct(TileManager tileManager)
        {
            _tileManager = tileManager;
        }

        public DisplayTileState(TileManager tileManager)
		{
			_tileManager = tileManager;
		}

        private static readonly Vector3Int[] offsets4 = new Vector3Int[]
        {
                new Vector3Int( 1, 0,  0), // +X
                new Vector3Int(-1, 0,  0), // -X
                new Vector3Int( 0, 0,  1), // +Z
                new Vector3Int( 0, 0, -1)  // -Z
        };

        public void DisplayClicableTile(string onTileKey, int moveDistance)
		{
            if (_tileManager.GetTileData(onTileKey) == null)
            return;

            var startTile = _tileManager.GetTileData(onTileKey);

            var visited = new HashSet<TileData>();
            var queue = new Queue<(TileData tile, int distance)>();

            queue.Enqueue((startTile, 0));
            visited.Add(startTile);

            while (queue.Count > 0)
            {
                var (current, distance) = queue.Dequeue();

                if (distance > moveDistance)
                    continue;


                // 強調表示
                if (distance == moveDistance)
                    _tileManager.ChangesetColor(current.Key, TileType.clickedTeamB);

                foreach (var offset in offsets4)
                {
                    int newX = (int)current.Position.x + offset.x;
					int newY = (int)current.Position.y + offset.y;
					int newZ = (int)current.Position.z + offset.z;
                    
					var adjacentKey = $"{newX}-{newY}-{newZ}";
                    // var nextPos = currentPos + offset;

                    var nextTileData = _tileManager.GetTileData(adjacentKey);

                    if (visited.Contains(nextTileData))
                        continue;

                    if (_tileManager.GetTileData(adjacentKey) == null)
                        continue;

                    // visitedのタイミングがおかしい気がする
                    // 引数から
                    visited.Add(nextTileData);
                    queue.Enqueue((nextTileData, distance + 1));
                }
            }
		}
    }
}

