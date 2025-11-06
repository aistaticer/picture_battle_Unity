using UnityEngine;

public interface ICameraMover
{
	void Move(Transform transform);
	void MoveTo(GameObject obj, Vector3 targetPosition, float duration);
}
