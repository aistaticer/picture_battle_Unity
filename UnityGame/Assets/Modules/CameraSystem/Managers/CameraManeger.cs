using UnityEngine;
using Zenject;
using System;

public class CameraManeger
{

	// [SerializeField] private CameraObjSet cameraObjSet;

	// private CameraMover mainCameraMover;

	// private ClickState _clickState;


	[Inject]
	CameraSystemContext _cameraSystemContext;

	public event Action<Transform> OnTargetChanged;

	public void SetTarget(Transform target)
	{
		_cameraSystemContext.mainCameraState.SetTarget(target);
		OnTargetChanged?.Invoke(target);
	}
}