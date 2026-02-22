using UnityEngine;
using System.Collections;
using Zenject;

namespace CameraSystem.Services
{
    /// <summary>
    /// カメラシェイク効果を提供するコンポーネント
    /// </summary>
    public class CameraShaker : MonoBehaviour
    {
        private CameraSystemContext _cameraSystemContext;
        private Vector3 _originalPosition;
        private bool _isShaking = false;

        [Inject]
        public void Construct(CameraSystemContext cameraSystemContext)
        {
            _cameraSystemContext = cameraSystemContext;
        }

        private void Start()
        {
            if (_cameraSystemContext?.mainCameraState?.Camera != null)
            {
                _originalPosition = _cameraSystemContext.mainCameraState.Camera.transform.localPosition;
            }
        }

        /// <summary>
        /// カメラを短時間震わせる
        /// </summary>
        /// <param name="duration">震える時間（秒）</param>
        /// <param name="magnitude">震えの強さ</param>
        public void Shake(float duration = 0.2f, float magnitude = 0.1f)
        {
            if (_cameraSystemContext?.mainCameraState?.Camera == null || _isShaking)
                return;

            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

        private IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            _isShaking = true;
            float elapsed = 0f;

            var cameraTransform = _cameraSystemContext.mainCameraState.Camera.transform;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                cameraTransform.localPosition = _originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            cameraTransform.localPosition = _originalPosition;
            _isShaking = false;
        }
    }
}
