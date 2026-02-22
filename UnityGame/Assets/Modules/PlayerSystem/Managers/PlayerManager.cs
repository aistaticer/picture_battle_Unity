using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// ゲーム内のプレイヤー情報を管理
/// </summary>
public class PlayerManager
{
	private readonly Dictionary<PlayerState, GameObject> _playerObjects
			= new Dictionary<PlayerState, GameObject>();

	// Awake()とConstruct()は削除 - 初期化はPlayerSystemStartUpで行う

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

}

