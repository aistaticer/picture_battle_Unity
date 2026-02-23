/// <summary>
/// プレイヤーの行動状態
/// </summary>
public enum PlayerActionState
{
	/// <summary>待機中（通常状態）</summary>
	Idle,

	/// <summary>移動中</summary>
	Moving,

	/// <summary>スタン状態（行動不能）</summary>
	Stunned
}
