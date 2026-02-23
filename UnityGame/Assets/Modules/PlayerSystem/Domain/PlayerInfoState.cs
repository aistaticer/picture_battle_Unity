using System.Collections.Generic;

/// <summary>プレイヤーの基本情報（ユーザー名・チーム名など）</summary>
public class PlayerInfoState
{
	public string UserName { get; private set; }
	public string TeamName { get; private set; }
	public string UserId { get; private set; }

	/// <summary>どのモデルでスポーンさせるか（Ghost / Cube）</summary>
	public PlayerModelType ModelType { get; private set; }

	public Position Position { get; private set; }

	/// <summary>このプレイヤーが持つアビリティのリスト</summary>
	public List<string> Abilities { get; private set; }

	public PlayerInfoState(string userName, string teamName, PlayerModelType modelType, string userId = null, Position position = null, List<string> abilities = null)
	{
		UserName = userName;
		TeamName = teamName;
		UserId = userId;
		ModelType = modelType;
		Position = position;
		Abilities = abilities ?? new List<string>();
	}

	/// <summary>
	/// プレイヤーの位置を更新する
	/// </summary>
	/// <param name="newPosition">新しい位置</param>
	public void UpdatePosition(Position newPosition)
	{
		Position = newPosition;
	}

	/// <summary>
	/// アビリティを追加する
	/// </summary>
	public void AddAbility(string abilityName)
	{
		if (!Abilities.Contains(abilityName))
		{
			Abilities.Add(abilityName);
		}
	}

	/// <summary>
	/// アビリティを持っているかチェック
	/// </summary>
	public bool HasAbility(string abilityName)
	{
		return Abilities.Contains(abilityName);
	}
}

