using UnityEngine;
using System.Collections.Generic;
using Zenject;

/// <summary>
/// ゲーム内のプレイヤー情報を管理
/// </summary>
public class PlayerManager
{
	private readonly Dictionary<PlayerState, GameObject> _playerObjects
			= new Dictionary<PlayerState, GameObject>();

	private PlayerMover _playerMover;

	[Inject]
	public void Construct(PlayerMover playerMover)
	{
		_playerMover = playerMover;
	}

	/// <summary>
	/// 受け取った情報からオブジェクトを生成し、playerのオブジェクトとして配置する
	/// </summary>
	/// <param name="info">プレイヤー情報</param>
	/// <param name="prefab">プレイヤーのオブジェクト</param>
	/// <param name="position">プレイヤーの位置</param>
	/// <returns></returns> <summary>
	/// 
	/// </summary>
	public GameObject CreatePlayer(PlayerState info, GameObject prefab, Vector3 position)
	{
		var obj = GameObject.Instantiate(prefab, position, Quaternion.identity);
		_playerObjects[info] = obj;
		return obj;
	}

	public void RegisterPlayer(PlayerState state, GameObject obj)
	{
		if (!_playerObjects.ContainsKey(state))
		{
			_playerObjects[state] = obj;
		}
	}

	public GameObject GetPlayerObject(PlayerState state)
	{
		return _playerObjects.TryGetValue(state, out var obj) ? obj : null;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーのタイルキー（"x-y-z"形式）を取得
	/// </summary>
	public string GetPlayerTileKeyByUserId(string userId)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				var pos = kvp.Key.Info.Position;
				return $"{pos.x}-{pos.y}-{pos.z}";
			}
		}
		return null;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーの現在位置を取得
	/// </summary>
	public Vector3? GetPlayerPositionByUserId(string userId)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				return kvp.Value.transform.position;
			}
		}
		return null;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーのPlayerInfoStateを取得
	/// </summary>
	public PlayerInfoState GetPlayerInfoByUserId(string userId)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				return kvp.Key.Info;
			}
		}
		return null;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーのGameObjectを取得
	/// </summary>
	public GameObject GetPlayerGameObjectByUserId(string userId)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				return kvp.Value;
			}
		}
		return null;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーオブジェクトを指定座標に移動させる
	/// </summary>
	/// <param name="userId">移動させるプレイヤーのUserId</param>
	/// <param name="targetPosition">移動先の座標</param>
	/// <returns>移動に成功した場合true、失敗した場合false</returns>
	public bool MovePlayerToPosition(string userId, Vector3 targetPosition)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				if (kvp.Value != null)
				{
					// GameObjectの位置を更新
					kvp.Value.transform.position = targetPosition;

					// PlayerStateの位置を更新
					kvp.Key.UpdateTransform(targetPosition, kvp.Value.transform.rotation);

					// PlayerInfoStateの位置を更新
					Position newPosition = Position.FromVector3(targetPosition);
					kvp.Key.Info.UpdatePosition(newPosition);

					return true;
				}
				else
				{
					Debug.LogWarning($"UserId {userId} のプレイヤーオブジェクトがnullです");
					return false;
				}
			}
		}
		Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません");
		return false;
	}

	/// <summary>
	/// UserIdに対応するプレイヤーオブジェクトを指定タイルキーの座標に移動させる
	/// </summary>
	/// <param name="userId">移動させるプレイヤーのUserId</param>
	/// <param name="tileKey">移動先のタイルキー（"x-y-z"形式）</param>
	/// <returns>移動に成功した場合true、失敗した場合false</returns>
	public bool MovePlayerToTileKey(string userId, string tileKey)
	{
		// タイルキーを座標に変換（"1-0-1" → Vector3(1, 0, 1)）
		var parts = tileKey.Split('-');
		if (parts.Length != 3)
		{
			Debug.LogWarning($"無効なタイルキー形式: {tileKey}");
			return false;
		}

		if (int.TryParse(parts[0], out int x) &&
		    int.TryParse(parts[1], out int y) &&
		    int.TryParse(parts[2], out int z))
		{
			Vector3 targetPosition = new Vector3(x, y, z);
			return MovePlayerToPosition(userId, targetPosition);
		}
		else
		{
			Debug.LogWarning($"タイルキーのパースに失敗: {tileKey}");
			return false;
		}
	}

	/// <summary>
	/// UserIdに対応するプレイヤーオブジェクトを指定タイルキーの座標に移動させ、進行方向を向く
	/// </summary>
	/// <param name="userId">移動させるプレイヤーのUserId</param>
	/// <param name="tileKey">移動先のタイルキー（"x-y-z"形式）</param>
	/// <param name="direction">進行方向ベクトル</param>
	/// <returns>移動に成功した場合true、失敗した場合false</returns>
	public bool MovePlayerToTileKeyWithDirection(string userId, string tileKey, Vector3 direction)
	{
		// タイルキーを座標に変換
		var parts = tileKey.Split('-');
		if (parts.Length != 3)
		{
			Debug.LogWarning($"無効なタイルキー形式: {tileKey}");
			return false;
		}

		if (int.TryParse(parts[0], out int x) &&
		    int.TryParse(parts[1], out int y) &&
		    int.TryParse(parts[2], out int z))
		{
			Vector3 targetPosition = new Vector3(x, y, z);

			foreach (var kvp in _playerObjects)
			{
				if (kvp.Key.Info.UserId == userId)
				{
					if (kvp.Value != null)
					{
						// DI登録されたPlayerMoverを使用してジャンプ移動
						if (_playerMover != null)
						{
							_playerMover.JumpToPosition(kvp.Value.transform, targetPosition, direction);
						}

						// PlayerStateの位置と回転を更新（最終位置で更新）
						kvp.Key.UpdateTransform(targetPosition, kvp.Value.transform.rotation);

						// 移動方向を記録
						kvp.Key.SetLastMovementDirection(direction);

						// PlayerInfoStateの位置を更新
						Position newPosition = Position.FromVector3(targetPosition);
						kvp.Key.Info.UpdatePosition(newPosition);

						return true;
					}
					else
					{
						Debug.LogWarning($"UserId {userId} のプレイヤーオブジェクトがnullです");
						return false;
					}
				}
			}
			Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません");
			return false;
		}
		else
		{
			Debug.LogWarning($"タイルキーのパースに失敗: {tileKey}");
			return false;
		}
	}

	/// <summary>
	/// 全プレイヤーのPlayerStateを取得
	/// </summary>
	public List<PlayerState> GetAllPlayers()
	{
		return new List<PlayerState>(_playerObjects.Keys);
	}

	/// <summary>
	/// 全プレイヤーのUserIdを取得
	/// </summary>
	public List<string> GetAllPlayerUserIds()
	{
		var userIds = new List<string>();
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId != null)
			{
				userIds.Add(kvp.Key.Info.UserId);
			}
		}
		return userIds;
	}

	/// <summary>
	/// UserIdに対応するPlayerStateを取得
	/// </summary>
	public PlayerState GetPlayerStateByUserId(string userId)
	{
		foreach (var kvp in _playerObjects)
		{
			if (kvp.Key.Info.UserId == userId)
			{
				return kvp.Key;
			}
		}
		return null;
	}

	/// <summary>
	/// 指定されたプレイヤーの状態を設定
	/// </summary>
	public void SetPlayerState(string userId, PlayerActionState newState)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			playerState.SetState(newState);
		}
		else
		{
			Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（SetPlayerState）");
		}
	}

	/// <summary>
	/// 指定されたプレイヤーの状態を取得
	/// </summary>
	public PlayerActionState GetPlayerActionState(string userId)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			return playerState.State;
		}
		Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（GetPlayerActionState）");
		return PlayerActionState.Idle;
	}

	/// <summary>
	/// アビリティのクールダウンを設定
	/// </summary>
	public void SetAbilityCooldown(string userId, string abilityName, float cooldownEndTime)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			playerState.SetAbilityCooldown(abilityName, cooldownEndTime);
		}
		else
		{
			Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（SetAbilityCooldown）");
		}
	}

	/// <summary>
	/// アビリティがクールダウン中かチェック
	/// </summary>
	public bool IsAbilityOnCooldown(string userId, string abilityName)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			return playerState.IsAbilityOnCooldown(abilityName);
		}
		Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（IsAbilityOnCooldown）");
		return false;
	}

	/// <summary>
	/// アビリティのクールダウン残り時間を取得
	/// </summary>
	public float GetAbilityCooldownRemaining(string userId, string abilityName)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			return playerState.GetAbilityCooldownRemaining(abilityName);
		}
		Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（GetAbilityCooldownRemaining）");
		return 0f;
	}

	/// <summary>
	/// アビリティのクールダウンをクリア
	/// </summary>
	public void ClearAbilityCooldown(string userId, string abilityName)
	{
		var playerState = GetPlayerStateByUserId(userId);
		if (playerState != null)
		{
			playerState.ClearAbilityCooldown(abilityName);
		}
		else
		{
			Debug.LogWarning($"UserId {userId} のプレイヤーが見つかりません（ClearAbilityCooldown）");
		}
	}

}

