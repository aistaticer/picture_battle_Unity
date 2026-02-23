using UnityEngine;
using System.Collections;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Effects
{
    /// <summary>
    /// プレイヤーをスタン状態にする効果
    /// 震えるアニメーション + 移動不可
    /// </summary>
    public class StunEffect : MonoBehaviour
    {
        private bool _isStunned = false;
        private float _stunEndTime = 0f;
        private Vector3 _originalPosition;
        private Coroutine _shakeCoroutine;

        /// <summary>
        /// スタン中かどうか
        /// </summary>
        public bool IsStunned => _isStunned && Time.time < _stunEndTime;

        /// <summary>
        /// スタン効果を適用
        /// </summary>
        /// <param name="duration">スタン時間（秒）</param>
        /// <param name="shakeIntensity">震えの強さ</param>
        public void ApplyStun(float duration, float shakeIntensity = 0.1f)
        {
            _isStunned = true;
            _stunEndTime = Time.time + duration;
            _originalPosition = transform.position;

            // 既存の震えを停止
            if (_shakeCoroutine != null)
            {
                StopCoroutine(_shakeCoroutine);
            }

            // 震えアニメーション開始
            _shakeCoroutine = StartCoroutine(ShakeCoroutine(duration, shakeIntensity));

            Debug.Log($"{gameObject.name} がスタン状態になりました（{duration}秒）");
        }

        private IEnumerator ShakeCoroutine(float duration, float intensity)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                // ランダムな方向に少し震える
                float offsetX = Random.Range(-intensity, intensity);
                float offsetZ = Random.Range(-intensity, intensity);

                transform.position = _originalPosition + new Vector3(offsetX, 0, offsetZ);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // 元の位置に戻す
            transform.position = _originalPosition;
            _isStunned = false;

            Debug.Log($"{gameObject.name} のスタンが解除されました");
        }

        /// <summary>
        /// 現在のスタン状態をチェック
        /// </summary>
        public void Update()
        {
            // スタン時間が切れたら自動的に解除
            if (_isStunned && Time.time >= _stunEndTime)
            {
                if (_shakeCoroutine != null)
                {
                    StopCoroutine(_shakeCoroutine);
                    _shakeCoroutine = null;
                }
                transform.position = _originalPosition;
                _isStunned = false;
            }
        }
    }
}
