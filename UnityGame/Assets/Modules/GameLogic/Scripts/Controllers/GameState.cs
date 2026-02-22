namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
	/// <summary>
	/// ゲームの状態を表す列挙型
	/// </summary>
	public enum GameState
	{
		/// <summary>待機状態（操作可能）</summary>
		Idle,

		/// <summary>選択モード（タイル選択中）</summary>
		Selecting,

		/// <summary>プレイヤー移動中</summary>
		PlayerMoving,

		/// <summary>ゲーム終了</summary>
		GameOver
	}
}
