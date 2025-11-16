using UnityEngine;
using System;

[Serializable]
public class TileData
{
    public string Key;
    public TileType Type;
    public Position Position;

    public string UserId;
    
    public TileData(string key, TileType type, Position pos, string userId) {
        Key = key;
        Type = type;
        Position = pos;
        UserId = userId;
    }
}
