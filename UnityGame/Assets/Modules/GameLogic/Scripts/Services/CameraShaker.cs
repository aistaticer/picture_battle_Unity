using UnityEngine;
using System.Collections;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// カメラシェイク効果を提供するコンポーネント
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        private Camera _mainCamera;
        private Vector3 _originalPosition;
        private bool _isShaking = false;

        private void Start()
        {
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                _originalPosition = _mainCamera.transform.localPosition;
            }
        }

        /// <summary>
        /// カメラを短時間震わせる
        /// </summary>
        /// <param name="duration">震える時間（秒）</param>
        /// <param name="magnitude">震えの強さ</param>
        public void Shake(float duration = 0.2f, float magnitude = 0.1f)
        {
            if (_mainCamera == null || _isShaking)
                return;

            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            _isShaking = true;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                _mainCamera.transform.localPosition = _originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            _mainCamera.transform.localPosition = _originalPosition;
            _isShaking = false;
        }
    }
}
