using UnityEngine;
using System.Collections;
using Zenject;
using Unity.VisualScripting;

public class CameraMover : ICameraMover
{
    private readonly float _speed;
    private readonly float _followSpeed = 10f; // カメラの追従速度

    private readonly CoroutineRunner _runner;
    private readonly PlayerManager _playerManager;

    private Coroutine _moveCoroutine;
    private Vector3 _offset; // Aliceとカメラの初期オフセット
    private Vector3 _initialAlicePosition; // Aliceの初期位置
    private bool _offsetInitialized = false;

    private const string ALICE_USER_ID = "player001";
    private const float Z_FOLLOW_RATIO = 0.5f; // Z軸の追従比率（調整可能）

    [Inject]
    public CameraMover(CoroutineRunner runner, PlayerManager playerManager, float speed = 5f)
    {
        _speed = speed;
        _runner = runner;
        _playerManager = playerManager;
    }
    

    public void Move(Transform cameraTransform)
    {
        // Aliceの現在位置を取得
        var alicePosition = _playerManager.GetPlayerPositionByUserId(ALICE_USER_ID);
        if (alicePosition == null)
            return;

        // 初回のみ：カメラとAliceの初期オフセットと初期位置を記録
        if (!_offsetInitialized)
        {
            _offset = cameraTransform.position - alicePosition.Value;
            _initialAlicePosition = alicePosition.Value;
            _offsetInitialized = true;

            // 初期位置でAliceのX軸と合わせる（カメラのX位置をAliceと同じにする）
            Vector3 initialPosition = cameraTransform.position;
            initialPosition.x = alicePosition.Value.x;
            cameraTransform.position = initialPosition;

            // X軸のオフセットを0にする（完全に同じX位置から開始）
            _offset.x = 0;
        }

        // X軸は完全追従、Z軸は移動量の半分で追従、Y軸は固定
        Vector3 currentPosition = cameraTransform.position;

        // Aliceの移動量を計算
        float aliceZMovement = alicePosition.Value.z - _initialAlicePosition.z;

        // カメラの目標位置を計算（X軸はオフセット0なのでAliceと同じ位置）
        float targetX = alicePosition.Value.x;
        float targetZ = _initialAlicePosition.z + _offset.z + (aliceZMovement * Z_FOLLOW_RATIO);

        // X軸とZ軸を滑らかに追従（Y軸は変更しない）
        currentPosition.x = Mathf.Lerp(
            currentPosition.x,
            targetX,
            _followSpeed * Time.deltaTime
        );
        currentPosition.z = Mathf.Lerp(
            currentPosition.z,
            targetZ,
            _followSpeed * Time.deltaTime
        );

        cameraTransform.position = currentPosition;
    }

    public void MoveTo(GameObject obj, Vector3 targetPosition, float duration)
    {
        if (_moveCoroutine != null)
            _runner.StopCoroutine(_moveCoroutine);
        _moveCoroutine = _runner.StartCoroutine(MoveCoroutine(obj.transform, targetPosition, duration));
    }

    private IEnumerator MoveCoroutine(Transform obj, Vector3 target, float duration)
    {
        Vector3 start = obj.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            obj.position = Vector3.Lerp(start, target, elapsed / duration);
            yield return null;
        }

        obj.position = target; // 最終位置をしっかり合わせる
    }
}