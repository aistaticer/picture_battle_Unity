using UnityEngine;
using Zenject;
using UnityGame.Assets.Modules.GameLogic.Scripts.Effects;

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
    /// Aliceが敵と同じタイルに移動すると、敵を1秒間スタンさせる
    /// 15秒のクールダウンあり
    /// </summary>
    public class GhostTouchAbility
    {
        private readonly PlayerManager _playerManager;
        private readonly TileManager _tileManager;

        private const string ALICE_USER_ID = "player001";
        private const string BOB_USER_ID = "player002";
        private const float STUN_DURATION = 2.0f;
        private const float SHAKE_INTENSITY = 0.1f;
        private const float COOLDOWN_DURATION = 15.0f; // 15秒クールダウン

        private StunEffect _bobStunEffect = null;

        // アビリティの状態管理
        private GhostTouchState _state = GhostTouchState.Ready;
        private float _cooldownEndTime = 0f;

        [Inject]
        public GhostTouchAbility(PlayerManager playerManager, TileManager tileManager)
        {
            _playerManager = playerManager;
            _tileManager = tileManager;
        }

        /// <summary>
        /// プレイヤー移動シグナルのハンドラ
        /// </summary>
        public void OnPlayerMoved(PlayerMovedSignal signal)
        {
            // Aliceが移動した時のみチェック
            if (signal.UserId != ALICE_USER_ID)
                return;

            // クールダウン中は発動しない
            if (_state == GhostTouchState.Cooldown)
            {
                // クールダウン解除チェック
                if (Time.time >= _cooldownEndTime)
                {
                    _state = GhostTouchState.Ready;
                    Debug.Log("【GhostTouch】クールダウン解除！再発動可能");
                }
                else
                {
                    // まだクールダウン中
                    return;
                }
            }

            // Bobの現在のタイルキーを取得
            string bobTileKey = _playerManager.GetPlayerTileKeyByUserId(BOB_USER_ID);
            if (bobTileKey == null)
                return;

            // Aliceと同じタイルにいるかチェック
            if (signal.TileKey == bobTileKey)
            {
                TriggerGhostTouch();
            }
        }

        /// <summary>
        /// GhostTouchアビリティを発動
        /// </summary>
        private void TriggerGhostTouch()
        {
            Debug.Log($"【GhostTouch発動】Aliceが敵と同じタイルに到達！（次回発動まで{COOLDOWN_DURATION}秒）");

            // BobのGameObjectを取得
            var bobGameObject = _playerManager.GetPlayerGameObjectByUserId(BOB_USER_ID);
            if (bobGameObject == null)
            {
                Debug.LogWarning("BobのGameObjectが見つかりません");
                return;
            }

            // StunEffectコンポーネントを取得または追加
            if (_bobStunEffect == null)
            {
                _bobStunEffect = bobGameObject.GetComponent<StunEffect>();
                if (_bobStunEffect == null)
                {
                    _bobStunEffect = bobGameObject.AddComponent<StunEffect>();
                }
            }

            // スタン効果を適用
            _bobStunEffect.ApplyStun(STUN_DURATION, SHAKE_INTENSITY);

            // クールダウン状態に移行
            _state = GhostTouchState.Cooldown;
            _cooldownEndTime = Time.time + COOLDOWN_DURATION;
        }

        /// <summary>
        /// 現在の状態を取得
        /// </summary>
        public GhostTouchState GetState() => _state;

        /// <summary>
        /// クールダウンの残り時間を取得
        /// </summary>
        public float GetCooldownRemaining()
        {
            if (_state != GhostTouchState.Cooldown)
                return 0f;

            return Mathf.Max(0f, _cooldownEndTime - Time.time);
        }

        /// <summary>
        /// 指定されたプレイヤーがスタン中かチェック
        /// </summary>
        public bool IsPlayerStunned(string userId)
        {
            if (userId != BOB_USER_ID)
                return false;

            if (_bobStunEffect == null)
                return false;

            return _bobStunEffect.IsStunned;
        }
    }
}
