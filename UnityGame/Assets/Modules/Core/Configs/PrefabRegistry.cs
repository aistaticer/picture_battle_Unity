using UnityEngine;
using System;
using Enums;
using System.Collections.Generic;

[Serializable]
public class TilePrefabEntry
{
    public TileFeildType type;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Config/PrefabRegistry")]
public class PrefabRegistry : ScriptableObject
{
    public List<TilePrefabEntry> tilePrefabs;
    public GameObject enemyPrefab;
    public GameObject itemPrefab;
}
