using UnityEngine;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Effects;
using System.Collections.Generic;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Services
{
    /// <summary>
    /// GhostTouchアビリティの状態
    /// </summary>
    public enum GhostTouchState
    {
        Ready,      // 発動可能
        Cooldown    // クールダウン中（再発動不可）
    }

    /// <summary>
    /// GhostTouchアビリティ
    /// "GhostTouch"アビリティを持つプレイヤーが敵と同じタイルに移動すると、敵をスタンさせる
    /// 15秒のクールダウンあり
    /// </summary>
    public class GhostTouchAbility : IPlayerAbility
    {
        private readonly PlayerManager _playerManager;
        private readonly TileManager _tileManager;

        private const float STUN_DURATION = 2.0f;
        private const float SHAKE_INTENSITY = 0.1f;
        private const float COOLDOWN_DURATION = 15.0f; // 15秒クールダウン

        // プレイヤーごとのStunEffectキャッシュ
        private readonly Dictionary<string, StunEffect> _stunEffects = new Dictionary<string, StunEffect>();

        // プレイヤーごとのクールダウン管理
        private readonly Dictionary<string, float> _cooldownEndTimes = new Dictionary<string, float>();

        /// <summary>
        /// アビリティ名（IPlayerAbilityインターフェース実装）
        /// </summary>
        public string AbilityName => "GhostTouch";

        [Inject]
        public GhostTouchAbility(PlayerManager playerManager, TileManager tileManager)
        {
            _playerManager = playerManager;
            _tileManager = tileManager;
        }

        /// <summary>
        /// プレイヤー移動シグナルのハンドラ（IPlayerAbilityインターフェース実装）
        /// AbilityManagerから呼ばれる（アビリティ所持チェックはAbilityManagerが行う）
        /// </summary>
        public void OnPlayerMoved(PlayerMovedSignal signal)
        {
            // クールダウン中は発動しない
            if (IsOnCooldown(signal.UserId))
            {
                // クールダウン解除チェック
                if (Time.time >= _cooldownEndTimes[signal.UserId])
                {
                    _cooldownEndTimes.Remove(signal.UserId);
                    Debug.Log($"【GhostTouch】{signal.UserId} のクールダウン解除！再発動可能");
                }
                else
                {
                    // まだクールダウン中
                    return;
                }
            }

            // 全プレイヤーをチェックして同じタイルにいる敵を探す
            var allPlayers = _playerManager.GetAllPlayers();
            foreach (var targetPlayer in allPlayers)
            {
                // 自分自身はスキップ
                if (targetPlayer.Info.UserId == signal.UserId)
                    continue;

                // 同じタイルにいるかチェック
                string targetTileKey = _playerManager.GetPlayerTileKeyByUserId(targetPlayer.Info.UserId);
                if (targetTileKey == signal.TileKey)
                {
                    // 敵を発見！GhostTouchを発動
                    TriggerGhostTouch(signal.UserId, targetPlayer.Info.UserId);
                }
            }
        }

        /// <summary>
        /// クールダウン中かチェック
        /// </summary>
        private bool IsOnCooldown(string userId)
        {
            return _cooldownEndTimes.ContainsKey(userId) && Time.time < _cooldownEndTimes[userId];
        }

        /// <summary>
        /// GhostTouchアビリティを発動
        /// </summary>
        private void TriggerGhostTouch(string attackerUserId, string targetUserId)
        {
            Debug.Log($"【GhostTouch発動】{attackerUserId} が {targetUserId} と同じタイルに到達！（次回発動まで{COOLDOWN_DURATION}秒）");

            // ターゲットのGameObjectを取得
            var targetGameObject = _playerManager.GetPlayerGameObjectByUserId(targetUserId);
            if (targetGameObject == null)
            {
                Debug.LogWarning($"{targetUserId} のGameObjectが見つかりません");
                return;
            }

            // StunEffectコンポーネントを取得または追加（キャッシュ）
            if (!_stunEffects.ContainsKey(targetUserId))
            {
                var stunEffect = targetGameObject.GetComponent<StunEffect>();
                if (stunEffect == null)
                {
                    stunEffect = targetGameObject.AddComponent<StunEffect>();
                }
                _stunEffects[targetUserId] = stunEffect;
            }

            // GhostTouchエフェクトを表示（右上に配置）
            var effectPrefab = Resources.Load<GameObject>("Effect/GhostTouchEffect");
            if (effectPrefab != null)
            {
                // ターゲットの右上にエフェクトを配置（オフセット: X+1, Y+1）
                Vector3 effectPosition = targetGameObject.transform.position + new Vector3(1f, 1f, 0f);
                // Z軸で180度回転
                var effectInstance = GameObject.Instantiate(effectPrefab, effectPosition, Quaternion.Euler(0, 0, 180));

                // スタン時間後に自動削除
                GameObject.Destroy(effectInstance, STUN_DURATION);

                Debug.Log($"【GhostTouch】エフェクト表示: {targetUserId}");
            }
            else
            {
                Debug.LogWarning("GhostTouchEffect プレハブが見つかりません: Resources/Effect/GhostTouchEffect");
            }

            // プレイヤーの状態をStunnedに変更
            _playerManager.SetPlayerState(targetUserId, PlayerActionState.Stunned);

            // スタン効果を適用（ビジュアル）
            // スタン終了時にIdleに戻すコールバックを渡す
            _stunEffects[targetUserId].ApplyStun(STUN_DURATION, SHAKE_INTENSITY, () =>
            {
                // スタン終了時、状態をIdleに戻す
                _playerManager.SetPlayerState(targetUserId, PlayerActionState.Idle);
            });

            // アビリティ使用者のクールダウン開始
            _cooldownEndTimes[attackerUserId] = Time.time + COOLDOWN_DURATION;
        }

        /// <summary>
        /// 指定されたプレイヤーのクールダウン残り時間を取得
        /// </summary>
        public float GetCooldownRemaining(string userId)
        {
            if (!IsOnCooldown(userId))
                return 0f;

            return Mathf.Max(0f, _cooldownEndTimes[userId] - Time.time);
        }

        /// <summary>
        /// 指定されたプレイヤーがスタン中かチェック
        /// </summary>
        public bool IsPlayerStunned(string userId)
        {
            // PlayerManagerから状態を取得
            return _playerManager.GetPlayerActionState(userId) == PlayerActionState.Stunned;
        }
    }
}
