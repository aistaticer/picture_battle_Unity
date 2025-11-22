using UnityEngine;
using System.Collections.Generic;
using Zenject;

public class TileActionService
{

	private TileManager _tileManager;

    [Inject]
    public void Construct(TileManager tileManager)
    {
        _tileManager = tileManager;
    }

	// 4方向オフセット (XとZだけ)
	private static readonly Vector3Int[] offsets4 = new Vector3Int[]
	{
			new Vector3Int( 1, 0,  0), // +X
			new Vector3Int(-1, 0,  0), // -X
			new Vector3Int( 0, 0,  1), // +Z
			new Vector3Int( 0, 0, -1)  // -Z
	};

	public void HandleTileClick(Tile tile)
	{
		if (tile.Type != TileType.clickableTeamA) return;

		var updateTiles = CreateUpdateTileDatas(tile);

		foreach (var data in updateTiles)
		{
			_tileManager.UpdateTile(data);
		}
	}

	/// <summary>
	/// タイルがクリックされた場合その四方のタイルがクリック可能状態になるため、それらのタイルをListに入れて返す
	/// </summary>
	private List<TileData> CreateUpdateTileDatas(Tile selectTile)
	{
			List<TileData> updateTileDatas = new List<TileData>();

			if (selectTile != null)
			{
				// 状態を Clicked に更新されたTileDataをListに格納
				_tileManager.UpdateTileType(selectTile.Key, TileType.clickedTeamA);
				var copiedTileData = _tileManager.GetTileData(selectTile.Key);
				var updatedTileData = new TileData(copiedTileData.Key, TileType.clickedTeamA, copiedTileData.Position, copiedTileData.UserId);
				updateTileDatas.Add(updatedTileData);

				string adjacentKey = null;

				for (int i = 0; i < offsets4.Length; i++)
				{
					int newX = (int)selectTile.Pos.x + offsets4[i].x;
					int newY = (int)selectTile.Pos.y + offsets4[i].y; // 今回は常に同じ
					int newZ = (int)selectTile.Pos.z + offsets4[i].z;
					adjacentKey = $"{newX}-{newY}-{newZ}";

					copiedTileData = _tileManager.GetTileData(adjacentKey);
					if (copiedTileData != null && copiedTileData.Type != TileType.clickedTeamA)
					{
						updatedTileData = new TileData(copiedTileData.Key, TileType.clickableTeamA, copiedTileData.Position, copiedTileData.UserId);
						updateTileDatas.Add(updatedTileData);
					}
				}
			}
				return updateTileDatas;
	}

	/// <summary>
	/// 二つの地点の最短経路を導き出す
	/// </summary>
	/// <param name="startTileKey"></param>
	/// <param name="endTileKey"></param>
	/// <returns></returns>
	public List<TileData> FindShortestPath(string startTileKey, string endTileKey)
	{
		// 探索用のデータ構造
		var queue = new Queue<TileData>();
		// 経路復元用: Keyに移動先タイルのキー、Valueにそのタイルへの移動元タイルを格納
		var cameFrom = new Dictionary<string, TileData>();
		// 訪問済みタイルを管理
		var visited = new HashSet<string> { startTileKey };

		queue.Enqueue(_tileManager.GetTileData(startTileKey));

		TileData current = null;
		bool pathFound = false;

		while (queue.Count > 0)
		{
			current = queue.Dequeue();

			if (current.Key == endTileKey)
			{
				pathFound = true;
				break;
			}

			// 4方向の隣接タイルを調べる
			foreach (var offset in offsets4)
			{
				var adjacentPos = new Position {
					x = current.Position.x + offset.x,
					y = current.Position.y + offset.y,
					z = current.Position.z + offset.z
				};

				var adjacentKey = $"{adjacentPos.x}-{adjacentPos.y}-{adjacentPos.z}";
				
				// 訪問済みならスキップ
				if (visited.Contains(adjacentKey))
				{
					continue;
				}

				var adjacentTileData = _tileManager.GetTileData(adjacentKey);

				// 隣接タイルが存在し、かつ通行可能かチェック (ルールに合わせて条件を変更してください)
				if (adjacentTileData != null && adjacentTileData.Type != TileType.clickedTeamA)
				{
					visited.Add(adjacentKey);
					// 目的地タイルから逆算して隣り合ったタイルをvalueに入れている.Keyは移動先タイルのキー(逆算して中身を入れていくため).
					cameFrom[adjacentTileData.Key] = current;
					// fifoで中にTIleDataを入れている
					// ここでは距離順を保持されず、距離順はcameFromがKeyとValueで数珠繋ぎのように保持している
					queue.Enqueue(adjacentTileData);
				}
			}
		}

		// 経路を復元
		if (pathFound)
		{
			var path = new List<TileData>();

			var currentKey = endTileKey;
			while (endTileKey != null)
			{
					var tile = _tileManager.GetTileData(currentKey);
					if (tile == null) break;

					path.Add(tile);

					// 親があるなら親キーへ
					if (cameFrom.ContainsKey(currentKey))
					{
							currentKey = cameFrom[currentKey].Key;
					}
					else
					{
							// 始点に到達
							break;
					}
			}

			path.Reverse();
			return path;
		}

		return new List<TileData>(); // 経路が見つからなかった場合
	}
}
