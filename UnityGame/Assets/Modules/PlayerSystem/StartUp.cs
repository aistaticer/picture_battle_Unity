using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerSystemStartUp
{
    [Inject] public PlayerModelSet playerModelSet;
    [Inject] public Movement movement;
    [Inject] public PlayerManager playerManager; // DIコンテナから注入される

    [Inject]
    public void Construct()
    {
        // PlayerManagerの初期化処理（元のAwakeの内容）
        List<PlayerInfoState> playerInfos = JsonLoader.LoadFromStreamingAssets<List<PlayerInfoState>>("player.json");

        foreach (var info in playerInfos)
        {
            // PlayerState を生成
            PlayerState playerState = new PlayerState(info);

            // モデルを取得して生成
            GameObject modelPrefab = playerModelSet.GetModel(playerState.Info.ModelType);

            if (modelPrefab == null)
            {
                Debug.LogError($"モデルが見つかりません: ModelType={playerState.Info.ModelType}");
                continue;
            }

            GameObject playerObject = GameObject.Instantiate(modelPrefab, playerState.Info.Position.ToVector3(), Quaternion.identity);

            // PlayerManagerに登録（既にDI注入されている）
            playerManager.RegisterPlayer(playerState, playerObject);
        }
    }
}
