using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Services;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Controllers
{
    /// <summary>
    /// アビリティのクールダウンをProgressBarに表示するUI
    /// AliceのGhostTouchアビリティのクールダウンを視覚化
    /// GameLogicからImageコンポーネントを直接操作し、必要な機能を実装
    /// </summary>
    public class AbilityCooldownUI : MonoBehaviour, ITickable
    {
        [Header("UI References (自動取得されます)")]
        [SerializeField] private Image _cooldownBarImage;
        [SerializeField] private Text _cooldownText;

        [Header("Visual Settings")]
        [SerializeField] private Color _readyColor = new Color(0.2f, 1f, 0.2f);      // 準備完了時の色（明るい緑）
        [SerializeField] private Color _cooldownColor = new Color(0.8f, 0.2f, 0.2f); // クールダウン中の色（赤）
        [SerializeField] private Color _alertColor = new Color(1f, 0.8f, 0f);        // 残り時間わずかの色（黄色）

        [Header("Alert Settings")]
        [SerializeField] private float _alertThreshold = 5.0f;  // この秒数以下でアラート色に変更

        [Header("Auto-Find Settings")]
        [SerializeField] private string _progressBarName = "UI ProgressBar";  // 探すProgressBarの名前

        private GhostTouchAbility _ghostTouchAbility;

        // AliceのUserID（操作プレイヤー）
        private const string ALICE_USER_ID = "player001";

        // GhostTouchのクールダウン時間（GhostTouchAbility.COOLDOWN_DURATIONと同じ）
        private const float COOLDOWN_DURATION = 15.0f;

        [Inject]
        public void Construct(GhostTouchAbility ghostTouchAbility)
        {
            _ghostTouchAbility = ghostTouchAbility;
        }

        /// <summary>
        /// 起動時にProgressBarのコンポーネントを自動的に取得
        /// </summary>
        private void Start()
        {
            // 手動でアサインされていない場合、自動的に取得
            if (_cooldownBarImage == null || _cooldownText == null)
            {
                AutoFindProgressBarComponents();
            }

            if (_cooldownBarImage == null)
            {
                Debug.LogError("[AbilityCooldownUI] ProgressBarのBarコンポーネントが見つかりません。" +
                              $"'{_progressBarName}'という名前のGameObjectが存在するか確認してください。");
            }
        }

        /// <summary>
        /// ProgressBarのコンポーネントを自動的に探して取得
        /// </summary>
        private void AutoFindProgressBarComponents()
        {
            // 名前でProgressBarオブジェクトを探す
            GameObject progressBarObject = GameObject.Find(_progressBarName);

            if (progressBarObject != null)
            {
                // "Bar"という名前の子オブジェクトからImageコンポーネントを取得
                Transform barTransform = progressBarObject.transform.Find("Bar");
                if (barTransform != null)
                {
                    _cooldownBarImage = barTransform.GetComponent<Image>();
                }

                // "Text"という名前の子オブジェクトからTextコンポーネントを取得
                Transform textTransform = progressBarObject.transform.Find("Text");
                if (textTransform != null)
                {
                    _cooldownText = textTransform.GetComponent<Text>();
                }

                if (_cooldownBarImage != null)
                {
                    Debug.Log($"[AbilityCooldownUI] ProgressBarコンポーネントを自動取得しました: {progressBarObject.name}");
                }
                else
                {
                    Debug.LogWarning($"[AbilityCooldownUI] '{progressBarObject.name}'内に'Bar'子オブジェクトが見つかりませんでした。");
                }
            }
            else
            {
                Debug.LogWarning($"[AbilityCooldownUI] '{_progressBarName}'という名前のProgressBarが見つかりませんでした。" +
                                "Inspectorで手動でアサインするか、ProgressBarの名前を確認してください。");
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる（ITickable実装）
        /// AliceのGhostTouchクールダウン残り時間を取得してProgressBarを更新
        /// </summary>
        public void Tick()
        {
            if (_cooldownBarImage == null)
                return;

            // Aliceのクールダウン残り時間を取得
            float cooldownRemaining = _ghostTouchAbility.GetCooldownRemaining(ALICE_USER_ID);

            if (cooldownRemaining > 0)
            {
                // クールダウン中 - 使用後に空から満タンへ向かって増加（15秒 → 0秒で0.0 → 1.0）
                float fillAmount = 1.0f - (cooldownRemaining / COOLDOWN_DURATION);
                _cooldownBarImage.fillAmount = fillAmount;

                // 残り時間に応じて色を変更
                if (cooldownRemaining <= _alertThreshold)
                {
                    // 残り時間がわずか（5秒以下）→ 警告色（黄色）
                    _cooldownBarImage.color = _alertColor;
                }
                else
                {
                    // クールダウン中 → 赤色
                    _cooldownBarImage.color = _cooldownColor;
                }

                // テキスト表示（オプション）
                if (_cooldownText != null)
                {
                    _cooldownText.text = $"GhostTouch: {cooldownRemaining:F1}s";
                }
            }
            else
            {
                // クールダウン終了 - 100%に戻す（使用可能状態）
                _cooldownBarImage.fillAmount = 1f;
                _cooldownBarImage.color = _readyColor;  // 準備完了 → 緑色

                // テキスト表示（オプション）
                if (_cooldownText != null)
                {
                    _cooldownText.text = "GhostTouch: Ready";
                }
            }
        }
    }
}
