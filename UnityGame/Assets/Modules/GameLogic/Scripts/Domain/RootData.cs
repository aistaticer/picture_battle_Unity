using UnityEngine;
using System;
using System.Collections.Generic;
using UnityGame.Assets.Modules.TileSystem.Scripts.Domain.Tile;

[Serializable]
public class RootData {
	public string type;
	public string action;
	public string myTeamName;
	public List<string> teamNames;
	public BoardData board;
	public Dictionary<string, string> ownerRelation;
	}
