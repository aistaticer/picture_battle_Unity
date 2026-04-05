using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSystemStartUp
{
    [Inject] public PlayerModelSet playerModelSet;
    [Inject] public Movement movement;
    [Inject] public PlayerManager playerManager;

    [Inject]
    public void Construct()
    {
        List<PlayerInfoState> playerInfos = JsonLoader.LoadFromStreamingAssets<List<PlayerInfoState>>("player.json");

        foreach (var info in playerInfos)
        {
            PlayerState playerState = new PlayerState(info);

            GameObject modelPrefab = playerModelSet.GetModel(playerState.Info.ModelType);

            if (modelPrefab == null)
            {
                Debug.LogError($"モデルが見つかりません: ModelType={playerState.Info.ModelType}");
                continue;
            }

            GameObject playerObject = GameObject.Instantiate(modelPrefab, playerState.Info.Position.ToVector3(), Quaternion.identity);

            playerManager.RegisterPlayer(playerState, playerObject);
        }
    }
}
