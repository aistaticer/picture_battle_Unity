using UnityEngine;
using System;
using System.Collections.Generic;
using UnityGame.Assets.Modules.TileSystem.Scripts.Domain.Tile;
using Codice.Client.BaseCommands.BranchExplorer.Layout;

public class TileManager
{
	/// <summary>
	/// タイル全体の状態を保持するDictionary
	/// </summary>
	/// <typeparam name="string"></typeparam>
	/// <typeparam name="Tile"></typeparam>
	/// <returns></returns>
	private Dictionary<string, Tile> tileDict = new Dictionary<string, Tile>();

	/// <summary>
	/// UserIdとTileOwner（所有タイル群）を紐付けた辞書型
	/// </summary>
	/// <typeparam name="string"></typeparam>
	/// <typeparam name="TileOwner"></typeparam>
	/// <returns></returns>
	private Dictionary<string, string> OwnerDict = new Dictionary<string, string>();



	/// <summary>
	/// プロパティにあるDictionaryにタイルを登録
	/// </summary>
	/// <param name="tile"></param>
	public void RegisterTile(Tile tile)
	{
		if (!tileDict.ContainsKey(tile.Key))
		{
			tileDict.Add(tile.Key, tile);
		}
	}

	/// <summary>
	/// プロパティにあるDictionaryにタイルを登録
	/// </summary>
	/// <param name="tile"></param>
	private Tile GetTile(String key)
	{
		if (tileDict.TryGetValue(key, out var tile))
		{
			return tile;
		}

		return null;
	}

	/// <summary>
	/// TileプロパティのTileHighlighterにアタッチされるTileHighlighterを設定
	/// </summary>
	/// <param name="tile"></param>
	public void BindTile(string key, TileType type,TileHighlighter tileHighlighter )
	{
		var tile = GetTile(key);
		tile.TileHighlighter = tileHighlighter;
		tileHighlighter.Bind(key, type);
	}

	public void SetTileKey(string key,Tile tile)
	{
		tile.SetKey(key);
	}

	/// <summary>
	/// 指定されたキーのタイルの状態を更新する
	/// </summary>
	public void UpdateTileType(string key, TileType newType)
	{
		if (tileDict.TryGetValue(key, out var tile))
		{
			tile.SetState(key, newType);
		}
	}

	/// <summary>
	/// 指定されたタイルの状態を更新する
	/// </summary>
	public void UpdateTile(Tile tile)
	{
		// 指定されたタイルのキーが辞書に存在する場合
		if (tileDict.ContainsKey(tile.Key))
		{	
			tileDict[tile.Key].SetState(tile.Key, tile.Type,tile.Pos);
		}
	}

	public Tile CreateFromTileData(TileData data)
	{
		return new Tile(data.Key, data.Type, data.Position);
	}

	/// <summary>
	/// 指定されたタイルの状態を更新する
	/// </summary>
	public void UpdateTile(TileData tile)
	{
		// 指定されたタイルのキーが辞書に存在する場合
		if (tileDict.ContainsKey(tile.Key))
		{
			tileDict[tile.Key].SetState(tile.Key, tile.Type);
		}
	}

	/// <summary>
	/// 指定されたキーとタイルの状態を持つ新しいタイルエンティティを生成する
	/// </summary>
	public Tile CreateTileEntity(string key, TileType type, Position pos)
	{
		return new Tile(key, type, pos);
	}

	/// <summary>
	/// 指定されたキーを持つタイルのデータのみを取得する
	/// </summary>
	public TileData GetTileData(string key)
	{
		if (tileDict.TryGetValue(key, out var tile))
		{
			return tile.ToData();
		}
		return null;
	}

	public void ChangesetColor(string key, TileType tileType)
	{
		if (tileDict.TryGetValue(key, out var tile))
		{
			tile.TileHighlighter.Apply(tileType);
		}
	}

	/// <summary>
	/// 所有者関係情報をOwnerDictに登録。
	/// </summary>
	/// <param name="tileOwner">登録する所有者情報</param>
	public void SetOwnerInfo(Dictionary<string, string> ownerRelation)
	{
		if (ownerRelation != null)
		{
			OwnerDict = ownerRelation;
		}
	}

	/// <summary>
	/// 指定したユーザーが所有する特定のタイルのデータを更新します。
	/// </summary>
	/// <param name="updateTileKey">タイルのキー</param>
		/// <param name="updateUserId">更新するUserId</param>
	public void UpdateOwnedTileData(string updateTileKey, string updateUserId)
	{
		if (string.IsNullOrEmpty(updateUserId) || updateTileKey == null) return;

		if (OwnerDict.ContainsKey(updateTileKey))
		{
			OwnerDict[updateTileKey] = updateUserId;
		}
	}


	/// <summary>
	/// 指定したユーザーの所有者情報を取得します。
	/// </summary>
	/// <param name="userId">所有者のID</param>
	/// <returns>見つかった場合はTileOwner、見つからない場合はnull</returns>
	public string GetOwnerInfo(string tileKey)
	{
		OwnerDict.TryGetValue(tileKey, out var userId);
		return userId;
	}
}
