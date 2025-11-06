using UnityEngine;
using System.Collections;
using Zenject;

public class Movement
{
    public float moveSpeed = 5f;

    public void MoveByKey(Transform transform)
    {
        float horizontal = Input.GetAxisRaw("Horizontal"); // ← →
        float vertical = Input.GetAxisRaw("Vertical");     // ↑ ↓

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
}
