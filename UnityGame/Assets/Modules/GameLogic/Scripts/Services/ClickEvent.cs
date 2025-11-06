using UnityEngine;
using System.Collections.Generic;

public class ClickEvent : IClickable
{
	private readonly TileActionService _tileActionService;

	// これは Zenject が自動で注入してくれる
	public ClickEvent(TileActionService tileActionService)
	{
		_tileActionService = tileActionService;
	}

	public void OnClick(GameObject clickedObject)
	{
		if (!clickedObject.TryGetComponent<Tile>(out var tile))
			return;
		_tileActionService.HandleTileClick(tile);

	}
}
