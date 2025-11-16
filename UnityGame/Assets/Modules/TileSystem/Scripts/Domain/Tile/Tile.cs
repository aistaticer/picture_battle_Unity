using UnityEngine;
using System;
using UnityEditor.Rendering;

public class Tile
{
	public string Key { get; private set; }
	public TileType Type { get; private set; }

	public Position Pos { get; private set; }

	internal string UserId { get; set; }

	public TileHighlighter TileHighlighter {get; internal set;}

	public Tile(string key, TileType type, Position pos)
	{
		Key = key;
		Type = type;
		Pos = pos;
	}

	  public bool CanClick(string userId)
		{
			if(UserId == userId){
				return true;
			}

			return false;
		}

	internal void SetType(TileType newType)
	{
		Type = newType;
	}

	internal void SetKey(String newKey)
	{
		Key = newKey;
	}

	internal void SetPos(Position newPos)
	{
		Pos = newPos;
	}


	/// <summary>
	/// 状態を更新するメソッド
	/// </summary>
	internal void SetState(string key, TileType type,Position pos)
	{
		Key = key;
		Type = type;
		Pos = pos;
	}

	/// <summary>
	/// 状態を更新するメソッド
	/// 盤状の見た目もフィールド変数も更新する
	/// 既存の Position を維持
	/// </summary>
	internal void SetState(string key, TileType type)
	{
		SetState(key, type, this.Pos);
	}

	/// <summary>
	/// 盤上の状態は更新しない初期化メソッド
	/// </summary>
	internal void SetData(string key, TileType type)
	{
		Key = key;
		Type = type;
	}
	
	internal TileData ToData()
	{
		return new TileData(this.Key, this.Type, this.Pos, this.UserId);
	}
}
