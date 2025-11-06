using UnityEngine;
using System;

[Serializable]
public class TileData
{
    public string Key;
    public TileType Type;
    public Position Position;
    
    public TileData(string key, TileType type, Position pos) {
        Key = key;
        Type = type;
        Position = pos;
    }
}
