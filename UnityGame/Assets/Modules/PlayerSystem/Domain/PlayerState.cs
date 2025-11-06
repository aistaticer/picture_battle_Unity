using UnityEngine;

/// <summary>プレイヤーの動的状態（位置・回転など）</summary>
public class PlayerState
{
    public PlayerInfoState Info { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    public PlayerState(PlayerInfoState info)
    {
        Info = info;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }
}

