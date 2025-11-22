using UnityEngine;
using Zenject;

public class MapController : MonoBehaviour 
{
    private TileSpawner Spawner;

    [Inject]
    public void Construct(TileSpawner tileSpawner)
    {
        Spawner = tileSpawner;
    }

    void Start()
    {
        RootData root = JsonLoader.LoadFromStreamingAssets<RootData>("test.json");
        if (root?.board?.tiles == null)
        {
            Debug.LogError("Tiles not found in JSON!");
            return;
        }

        Spawner.SpawnTiles(root.board.tiles, root.ownerRelation);
    }
}
