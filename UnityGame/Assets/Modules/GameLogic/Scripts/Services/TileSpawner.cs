using UnityEngine;
using System.Collections.Generic;
using picture_game_view.Assets.Modules.TileSystem.Configs;
using Zenject;
using System;
using UnityGame.Assets.Modules.TileSystem.Scripts.Domain.Tile;

public class TileSpawner
{
    private GameObject NomalTile { get; set; }

    private TileManager _tileManager;

    [Inject]
    public TileSpawner(TileManager manager, TIleObjSet TIleObjSet)
    {
        NomalTile = TIleObjSet.GetTileObj(TIleObjType.NomalTile);
        _tileManager = manager;
    }

    // タイル配置メソッド
    public void SpawnTiles(Dictionary<string, TileData> tiles, Dictionary<string, string> ownerRelation)
    {
        foreach (var entry in tiles)
        {
            TileData tileData = entry.Value;
            generateTile(tileData);

            _tileManager.SetOwnerInfo(ownerRelation);
        }
    }

    public void changeTile()
    {

    }

    /// <summary>
    /// タイルのDTOからタイルを作成
    /// 必要に応じたセットアップをしている(Dictionaryの登録などその他諸々)
    /// </summary>
    /// <param name="tileData">tileのDTO</param>
    public void generateTile(TileData tileData)
    {
        Vector3 pos = tileData.Position.ToVector3();
        GameObject obj = UnityEngine.Object.Instantiate(NomalTile, pos, Quaternion.identity);

        string key = tileData.Key;
        TileType tileType = tileData.Type;

        // 生成されたオブジェクトの名前をユニークにしている
        obj.name = $"Tile_{key}";

        // アタッチされたTileHighlighterを取得
        var view = obj.GetComponent<TileHighlighter>();

        _tileManager.RegisterTile(_tileManager.CreateFromTileData(tileData));

        _tileManager.BindTile(key, tileType,view);
    }
}
