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
}
