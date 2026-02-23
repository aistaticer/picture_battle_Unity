using UnityEngine;

/// <summary>プレイヤーの動的状態（位置・回転など）</summary>
public class PlayerState
{
    public PlayerInfoState Info { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }

    /// <summary>プレイヤーの現在の行動状態</summary>
    public PlayerActionState State { get; private set; }

    /// <summary>状態が変わった時刻（Time.time）</summary>
    public float StateChangedTime { get; private set; }

    public PlayerState(PlayerInfoState info)
    {
        Info = info;
        State = PlayerActionState.Idle;
        StateChangedTime = Time.time;
    }

    public void UpdateTransform(Vector3 position, Quaternion rotation)
    {
        Position = position;
        Rotation = rotation;
    }

    /// <summary>
    /// プレイヤーの状態を変更する
    /// </summary>
    public void SetState(PlayerActionState newState)
    {
        if (State != newState)
        {
            State = newState;
            StateChangedTime = Time.time;
        }
    }

    /// <summary>
    /// 現在の状態の経過時間を取得
    /// </summary>
    public float GetStateDuration()
    {
        return Time.time - StateChangedTime;
    }
}

