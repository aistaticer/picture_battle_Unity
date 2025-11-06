// Assets/Scripts/Models/CameraState.cs
using UnityEngine;

/// <summary>
/// カメラの現在の状態を保持するモデル
/// </summary>
public class CameraState
{
    /// <summary>使用カメラ</summary>
    public GameObject Camera;

    /// <summary>現在注目しているターゲット（プレイヤーやタイルなど）</summary>
    public Transform Target { get; private set; }

    /// <summary>カメラの現在位置</summary>
    public Vector3 Position { get; private set; }

    /// <summary>カメラの注視点</summary>
    public Vector3 LookAt { get; private set; }

	public CameraState(GameObject CameraObj)
    {
        Camera = CameraObj;
        Position = Camera.transform.position;
	}

    /// <summary>
    /// ターゲットを更新する
    /// </summary>
    public void SetTarget(Transform target)
    {
        Target = target;
    }

    /// <summary>
    /// カメラの位置を更新する
    /// </summary>
    public void SetPosition(Vector3 position)
    {
        Position = position;
    }

    /// <summary>
    /// カメラの注視点を更新する
    /// </summary>
    public void SetLookAt(Vector3 lookAt)
    {
        LookAt = lookAt;
    }

    /// <summary>
    /// ターゲット・位置・注視点をまとめて更新するユーティリティ
    /// </summary>
    public void UpdateCameraState(Transform target, Vector3 position, Vector3 lookAt)
    {
        Target = target;
        Position = position;
        LookAt = lookAt;
    }
}

