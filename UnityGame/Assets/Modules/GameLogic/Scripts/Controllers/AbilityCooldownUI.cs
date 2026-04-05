using UnityEngine;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Controllers
{
	public class AbilityCooldownUI : MonoBehaviour, ITickable
	{
		[Header("Ability Settings")]
		[SerializeField] private string _targetUserId;
		[SerializeField] private string _targetAbilityName;
		[SerializeField] private float _cooldownDuration;

		[Header("Plane Material")]
		[SerializeField] private Renderer _planeRenderer;

		private Material _planeMaterial;
		private PlayerManager _playerManager;

		[Inject]
		public void Construct(PlayerManager playerManager)
		{
			_playerManager = playerManager;
		}

		public void Initialize(string userId, string abilityName, float cooldownDuration)
		{
			_targetUserId = userId;
			_targetAbilityName = abilityName;
			_cooldownDuration = cooldownDuration;
		}

		private void Start()
		{
			if (_planeRenderer == null)
			{
				GameObject plane = GameObject.Find("GhostTouchPlane");
				if (plane != null)
					_planeRenderer = plane.GetComponent<Renderer>();
			}

			if (_planeRenderer != null)
				_planeMaterial = _planeRenderer.material;
			else
				Debug.LogWarning("[AbilityCooldownUI] 'plane' オブジェクトが見つかりません。インスペクタで _planeRenderer を設定してください。");
		}

		public void Tick()
		{
			if (_planeMaterial == null) return;

			float cooldownRemaining = _playerManager.GetAbilityCooldownRemaining(_targetUserId, _targetAbilityName);
			float percentage = cooldownRemaining > 0
				? 1.0f - (cooldownRemaining / _cooldownDuration)
				: 1.0f;

			_planeMaterial.SetFloat("_percentage", percentage);
		}
	}
}
