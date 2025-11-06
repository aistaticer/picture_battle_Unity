using UnityEngine;
using System.Collections;
using Zenject;
using Unity.VisualScripting;

public class CameraMover : ICameraMover
{
    private readonly float _speed;

    private readonly CoroutineRunner _runner;

    private Coroutine _moveCoroutine;

    [Inject]
    public CameraMover(CoroutineRunner runner,float speed = 5f)
    {
        _speed = speed;
        _runner = runner;
    }

    public void Move(Transform transform)
    {

        float horizontal = Input.GetAxisRaw("Horizontal"); // ← →
        float vertical = Input.GetAxisRaw("Vertical");     // ↑ ↓
        float moveSpeed = 5f;

        Vector3 direction = Vector3.zero;

        // 左右 = X軸
        direction.x = horizontal;

        // 前後 = Z軸（ここに上下キーを割り当てる）
        direction.z = vertical;

        // 高さ = Y軸（ここは別のキーで動かす例）
        if (Input.GetKey(KeyCode.Space))  // スペースで上昇
            direction.y = 1;
        else if (Input.GetKey(KeyCode.LeftShift)) // シフトで下降
            direction.y = -1;

        // 実際に動かす
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime, Space.World);
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