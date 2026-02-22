using UnityEngine;
using Zenject;
using picture_game_view.Assets.Modules.GameLogic.Scripts.Controllers;

/// <summary>
/// タイル選択機能を提供する外部API的なマネージャークラス
/// </summary>
public class SelectionManager : MonoBehaviour
{
	[Inject] private readonly GameLogicContext gameLogicContext;
	[Inject] private readonly SelectionController _selectionController;

	void Update()
	{
		// マウスクリック処理
		if (Input.GetMouseButtonDown(0))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out var hit))
			{
				gameLogicContext._clickState.SetClickedObject(hit.collider.gameObject);
				gameLogicContext._clickEvent.OnClick(gameLogicContext._clickState.LastClickedObject);
			}
		}
	}

	// ========== 外部API ==========

	/// <summary>
	/// 選択モードを開始する
	/// </summary>
	public void StartSelectionMode()
	{
		_selectionController.StartSelectionMode("player001");
	}

	/// <summary>
	/// 選択モードをキャンセルする
	/// </summary>
	public void CancelSelectionMode()
	{
		_selectionController.CancelSelectionMode();
	}

	/// <summary>
	/// 選択モードがアクティブかどうかを取得する
	/// </summary>
	public bool IsSelectionModeActive => _selectionController.IsSelectionModeActive;

	/// <summary>
	/// 現在選択中のタイルキーを取得する
	/// </summary>
	public string CurrentSelectedTileKey => _selectionController.CurrentSelectedTileKey;
}

