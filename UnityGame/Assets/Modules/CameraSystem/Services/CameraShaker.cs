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
        private bool _isShaking = false;

        [Inject]
        public void Construct(CameraSystemContext cameraSystemContext)
        {
            _cameraSystemContext = cameraSystemContext;
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

            // shake開始時の現在位置を保存（追従後の位置）
            Vector3 originalPosition = cameraTransform.localPosition;

            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                cameraTransform.localPosition = originalPosition + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                yield return null;
            }

            // shake終了後、開始時の位置に戻す
            cameraTransform.localPosition = originalPosition;
            _isShaking = false;
        }
    }
}
