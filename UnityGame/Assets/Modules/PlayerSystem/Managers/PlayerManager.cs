using UnityEngine;
using Zenject;
using System.Collections.Generic;

/// <summary>
/// ゲーム内のプレイヤー情報を管理
/// </summary>
public class PlayerManager : MonoBehaviour
{
	private Movement _movement;

	//private PlayerModelSet _playerModelSet;
	// 一旦か二番で進める。後ほどAddressablesを用いた都度ダウンロードに切り替える
	[SerializeField] private PlayerModelSet _playerModelSet;

	private readonly Dictionary<PlayerState, GameObject> _playerObjects
			= new Dictionary<PlayerState, GameObject>();

	[Inject]
	public void Construct(Movement movement, PlayerModelSet playerModelSet)
	{
		_movement = movement;
		_playerModelSet = playerModelSet;
	}

	void Awake()
	{
		List<PlayerInfoState> playerInfos = JsonLoader.LoadFromStreamingAssets<List<PlayerInfoState>>("player.json");
		
		foreach (var info in playerInfos)
		{
			// PlayerState を生成
			PlayerState playerState = new PlayerState(info);

			// モデルを取得して生成
			GameObject modelPrefab = _playerModelSet.GetModel(playerState.Info.ModelType);

			if (modelPrefab == null)
			{
				Debug.LogError($"モデルが見つかりません: ModelType={playerState.Info.ModelType}");
				return; // または continue;
			}

			GameObject playerObject = Instantiate(modelPrefab, playerState.Info.Position.ToVector3(), Quaternion.identity);

			// 登録処理
			RegisterPlayer(playerState, playerObject);
		}
	}

	void Update()
	{

	}


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



}

