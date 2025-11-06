using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{

	private CameraMover _mainCameraMover;
	private CameraMover _subCameraMover;

	[Inject]
	CameraSystemContext _cameraSystemContext;

	public CameraController(CameraSystemContext cameraSystemContext)
	{
		_cameraSystemContext = cameraSystemContext;
	}

	void Awake()
	{
		_mainCameraMover = new CameraMover(new CoroutineRunner());
		_subCameraMover = new CameraMover(new CoroutineRunner());

	}

	void Update()
	{
		_mainCameraMover.Move(_cameraSystemContext.mainCameraState.Camera.transform);
	}
}