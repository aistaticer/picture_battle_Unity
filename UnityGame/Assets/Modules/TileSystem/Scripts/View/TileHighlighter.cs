using UnityEngine;
using Enums;
using System;

/// <summary>
/// tileの画面上の状態を司るクラス 
/// renderなどの付与、操作を行う 
/// </summary>
public class TileHighlighter : MonoBehaviour
{
	public Renderer renderer;
	
	[SerializeField]
	public TileMaterialSet TileMatSet;

	/// <summary>
	/// アタッチされるtile
	/// </summary>
	/// <value></value>
	public Tile tile { get; private set; }

	private void Awake()
	{
		renderer = GetComponent<Renderer>();
	}

	/// <summary>
	/// TileとTileHighlighterを結びつける
	/// </summary>
	/// <param name="addTile"></param>
	internal void Bind(Tile addTile)
	{
		tile = addTile;
		Apply(tile.Type);
	}


	public void Apply(TileType type)
	{
		switch (type)
		{
			case TileType.Empty: renderer.material = TileMatSet.emptyMat; break;
			case TileType.clickableTeamA: renderer.material = TileMatSet.clickableTeamAMat; break;
			case TileType.clickableTeamB: renderer.material = TileMatSet.clickableTeamBMat; break;
			case TileType.clickedTeamA: renderer.material = TileMatSet.clickedTeamAMat; break;
			case TileType.clickedTeamB: renderer.material = TileMatSet.clickedTeamBMat; break;
		}
	}
}