using UnityEngine;
using Zenject;

namespace picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers
{
	/// <summary>
	/// ゲーム全体の状態を管理するコントローラー
	/// どの操作が可能かを制御する
	/// </summary>
	public class GameStateController : IInitializable
	{
		// 現在のゲーム状態
		private GameState _currentState;

		// 現在のターンのプレイヤーID
		private string _currentPlayerId;

		// プレイヤーIDのリスト
		private readonly string[] _playerIds = { "player001", "player002" }; // Alice, Bob

		public void Initialize()
		{
			// 初期状態はIdle（待機状態）
			_currentState = GameState.Idle;

			// 最初のターンはAlice（player001）
			_currentPlayerId = _playerIds[0];

			Debug.Log($"ゲーム開始: {_currentPlayerId} のターン (状態: {_currentState})");
		}

		/// <summary>
		/// 現在のゲーム状態を取得する
		/// </summary>
		public GameState GetCurrentState()
		{
			return _currentState;
		}

		/// <summary>
		/// 現在のターンのプレイヤーIDを取得する
		/// </summary>
		public string GetCurrentPlayerId()
		{
			return _currentPlayerId;
		}

		/// <summary>
		/// 選択モードを開始できるかチェック
		/// </summary>
		public bool CanStartSelection()
		{
			return _currentState == GameState.Idle;
		}

		/// <summary>
		/// 選択モードを開始する（状態をSelectingに変更）
		/// </summary>
		public void StartSelection()
		{
			if (_currentState == GameState.Idle)
			{
				_currentState = GameState.Selecting;
				Debug.Log($"選択モード開始: {_currentPlayerId} (状態: {_currentState})");
			}
			else
			{
				Debug.LogWarning($"選択モードを開始できません。現在の状態: {_currentState}");
			}
		}

		/// <summary>
		/// 選択をキャンセルする（状態をIdleに戻す）
		/// </summary>
		public void CancelSelection()
		{
			if (_currentState == GameState.Selecting)
			{
				_currentState = GameState.Idle;
				Debug.Log($"選択キャンセル (状態: {_currentState})");
			}
		}

		/// <summary>
		/// プレイヤーの移動を確定する（状態をIdleに戻す）
		/// </summary>
		public void ConfirmMove()
		{
			if (_currentState == GameState.Selecting)
			{
				_currentState = GameState.Idle;
				Debug.Log($"移動確定 (状態: {_currentState})");
			}
		}

		/// <summary>
		/// 現在のターンのプレイヤー名を取得する（デバッグ用）
		/// </summary>
		public string GetCurrentPlayerName()
		{
			return _currentPlayerId == "player001" ? "Alice" : "Bob";
		}
	}
}
