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

}

