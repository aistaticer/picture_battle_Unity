using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{

	private CameraMover _mainCameraMover;
	private CameraMover _subCameraMover;

	private CameraSystemContext _cameraSystemContext;
	private PlayerManager _playerManager;

	private CoroutineRunner _mainCoroutineRunner;
	private CoroutineRunner _subCoroutineRunner;

	[Inject]
	public void Construct(CameraSystemContext cameraSystemContext, PlayerManager playerManager)
	{
		_cameraSystemContext = cameraSystemContext;
		_playerManager = playerManager;
	}

	void Start()
	{
		// CoroutineRunner用のGameObjectを作成
		var mainRunnerObj = new GameObject("MainCameraCoroutineRunner");
		_mainCoroutineRunner = mainRunnerObj.AddComponent<CoroutineRunner>();

		var subRunnerObj = new GameObject("SubCameraCoroutineRunner");
		_subCoroutineRunner = subRunnerObj.AddComponent<CoroutineRunner>();

		// CameraMoverを作成（依存性を渡す）
		_mainCameraMover = new CameraMover(_mainCoroutineRunner, _playerManager);
		_subCameraMover = new CameraMover(_subCoroutineRunner, _playerManager);
	}

	void Update()
	{
		_mainCameraMover.Move(_cameraSystemContext.mainCameraState.Camera.transform);
	}
}