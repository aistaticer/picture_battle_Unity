/// <summary>プレイヤーの基本情報（ユーザー名・チーム名など）</summary>
public class PlayerInfoState
{
	public string UserName { get; private set; }
	public string TeamName { get; private set; }
	public string UserId { get; private set; }

	/// <summary>どのモデルでスポーンさせるか（Ghost / Cube）</summary>
	public PlayerModelType ModelType { get; private set; }
	
	public Position Position { get; private set; }

	public PlayerInfoState(string userName, string teamName, PlayerModelType modelType, string userId = null, Position position = null)
	{
		UserName = userName;
		TeamName = teamName;
		UserId = userId;
		ModelType = modelType;
		Position = position;
	}
}

