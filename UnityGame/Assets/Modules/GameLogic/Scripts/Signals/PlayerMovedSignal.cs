using UnityEngine;

/// <summary>
/// プレイヤーが移動した時に発火するシグナル
/// </summary>
public class PlayerMovedSignal
{
    public string UserId { get; set; }
    public string TileKey { get; set; }
    public Vector3 Position { get; set; }

    public PlayerMovedSignal(string userId, string tileKey, Vector3 position)
    {
        UserId = userId;
        TileKey = tileKey;
        Position = position;
    }
}
