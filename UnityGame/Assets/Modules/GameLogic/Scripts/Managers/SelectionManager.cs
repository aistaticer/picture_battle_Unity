using UnityEngine;
using Zenject;

public class SelectionManager : MonoBehaviour
{

	[Inject]
	private GameLogicContext gameLogicContext;

	void Update()
	{
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
}

