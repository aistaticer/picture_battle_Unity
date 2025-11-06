using UnityEngine;

[CreateAssetMenu(menuName = "Config/PlayerModelSet")]
public class PlayerModelSet : ScriptableObject
{
	public GameObject Ghost;
	public GameObject Cube; 
	

	public GameObject GetModel(PlayerModelType type)
	{
			switch (type)
			{
					case PlayerModelType.Ghost: return Ghost;
					case PlayerModelType.Cube: return Cube;
					default: return null;
			}
	}
}	