using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Zenject;

/// <summary>
/// プレイヤーのジャンプ移動を処理するコンポーネント
/// 複数のターゲットを同時に管理可能
/// </summary>
public class PlayerMover : MonoBehaviour
{
	// 移動中のターゲットを追跡（複数のプレイヤーが同時に移動可能）
	private HashSet<Transform> _movingTargets = new HashSet<Transform>();

	// AudioManagerへの参照（DIで注入）
	private picture_game_view.Assets.Modules.AudioSystem.Services.AudioManager _audioManager;

	[Inject]
	public void Construct(picture_game_view.Assets.Modules.AudioSystem.Services.AudioManager audioManager)
	{
		_audioManager = audioManager;
	}

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
		// このターゲットを移動中リストに追加
		_movingTargets.Add(target);

		// ジャンプ音を再生
		if (_audioManager != null)
		{
			_audioManager.PlaySE("キックの衣擦れ2");
		}

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

		// このターゲットを移動中リストから削除
		_movingTargets.Remove(target);
	}

	/// <summary>
	/// 指定されたターゲットが現在移動中かどうか
	/// </summary>
	/// <param name="target">チェックするターゲット</param>
	/// <returns>移動中の場合true</returns>
	public bool IsMoving(Transform target)
	{
		return _movingTargets.Contains(target);
	}

	/// <summary>
	/// 何かしらのターゲットが移動中かどうか
	/// </summary>
	public bool IsAnyMoving => _movingTargets.Count > 0;

	/// <summary>
	/// 現在移動中のターゲット数
	/// </summary>
	public int MovingCount => _movingTargets.Count;
}
