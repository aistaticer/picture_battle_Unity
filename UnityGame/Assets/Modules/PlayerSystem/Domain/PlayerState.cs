using UnityEngine;
using System.Collections.Generic;

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

    /// <summary>アビリティのクールダウン終了時刻（アビリティ名 → 終了時刻）</summary>
    public Dictionary<string, float> AbilityCooldowns { get; private set; }

    /// <summary>最後の移動方向（正規化済み）</summary>
    public Vector3 LastMovementDirection { get; private set; } = Vector3.forward;

    /// <summary>このプレイヤーに紐づくアビリティインスタンス</summary>
    public List<IPlayerAbility> Abilities { get; private set; }

    public PlayerState(PlayerInfoState info)
    {
        Info = info;
        State = PlayerActionState.Idle;
        StateChangedTime = Time.time;
        AbilityCooldowns = new Dictionary<string, float>();
        Abilities = new List<IPlayerAbility>();
    }

    /// <summary>
    /// アビリティインスタンスを追加する
    /// </summary>
    public void AddAbility(IPlayerAbility ability)
    {
        if (ability != null)
            Abilities.Add(ability);
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

    /// <summary>
    /// アビリティのクールダウンを設定
    /// </summary>
    public void SetAbilityCooldown(string abilityName, float cooldownEndTime)
    {
        AbilityCooldowns[abilityName] = cooldownEndTime;
    }

    /// <summary>
    /// アビリティがクールダウン中かチェック
    /// </summary>
    public bool IsAbilityOnCooldown(string abilityName)
    {
        if (!AbilityCooldowns.ContainsKey(abilityName))
            return false;

        if (Time.time >= AbilityCooldowns[abilityName])
        {
            // クールダウン終了したので削除
            AbilityCooldowns.Remove(abilityName);
            return false;
        }

        return true;
    }

    /// <summary>
    /// アビリティのクールダウン残り時間を取得
    /// </summary>
    public float GetAbilityCooldownRemaining(string abilityName)
    {
        if (!IsAbilityOnCooldown(abilityName))
            return 0f;

        return Mathf.Max(0f, AbilityCooldowns[abilityName] - Time.time);
    }

    /// <summary>
    /// アビリティのクールダウンをクリア
    /// </summary>
    public void ClearAbilityCooldown(string abilityName)
    {
        if (AbilityCooldowns.ContainsKey(abilityName))
        {
            AbilityCooldowns.Remove(abilityName);
        }
    }

    /// <summary>
    /// 最後の移動方向を設定
    /// </summary>
    public void SetLastMovementDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            LastMovementDirection = direction.normalized;
        }
    }
}

