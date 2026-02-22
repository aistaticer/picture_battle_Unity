using UnityEngine;
using System.Collections;

/// <summary>
/// プレイヤーのジャンプ移動を処理するコンポーネント
/// </summary>
public class PlayerMover : MonoBehaviour
{
	private bool _isMoving = false;

	/// <summary>
	/// 指定のTransformを指定位置にジャンプして移動する
	/// </summary>
	/// <param name="target">移動させる対象のTransform</param>
	/// <param name="targetPosition">目標位置</param>
	/// <param name="direction">進行方向（回転用）</param>
	/// <param name="duration">ジャンプにかかる時間（秒）</param>
	public void JumpToPosition(Transform target, Vector3 targetPosition, Vector3 direction, float duration = 0.3f)
	{
		if (target == null)
		{
			Debug.LogWarning("JumpToPosition: target is null");
			return;
		}

		StartCoroutine(JumpCoroutine(target, targetPosition, direction, duration));
	}

	private IEnumerator JumpCoroutine(Transform target, Vector3 targetPosition, Vector3 direction, float duration)
	{
		_isMoving = true;
		Vector3 startPosition = target.position;
		float elapsed = 0f;

		// 進行方向を向く
		if (direction != Vector3.zero)
		{
			Quaternion targetRotation = Quaternion.LookRotation(direction);
			target.rotation = targetRotation;
		}

		while (elapsed < duration)
		{
			float t = elapsed / duration;

			// 水平方向の移動（線形補間）
			Vector3 currentPosition = Vector3.Lerp(startPosition, targetPosition, t);

			// ジャンプの高さ（放物線）- sin曲線でより自然なジャンプ
			float jumpHeight = Mathf.Sin(t * Mathf.PI) * 0.5f; // 最大高さ0.5単位
			currentPosition.y = Mathf.Lerp(startPosition.y, targetPosition.y, t) + jumpHeight;

			target.position = currentPosition;

			elapsed += Time.deltaTime;
			yield return null;
		}

		// 最終位置に正確に配置
		target.position = targetPosition;
		_isMoving = false;
	}

	/// <summary>
	/// 現在移動中かどうか
	/// </summary>
	public bool IsMoving => _isMoving;
}
