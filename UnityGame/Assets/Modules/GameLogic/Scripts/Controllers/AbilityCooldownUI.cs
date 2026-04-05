using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UnityGame.Assets.Modules.GameLogic.Scripts.Controllers
{
	public class AbilityCooldownUI : MonoBehaviour, ITickable
	{
		[Header("Ability Settings")]
		[SerializeField] private string _targetUserId;
		[SerializeField] private string _targetAbilityName;
		[SerializeField] private float _cooldownDuration;

		[Header("UI Image")]
		[SerializeField] private Image _image;

		private Material _imageMaterial;
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
			if (_image == null)
			{
				string objectName = $"{_targetAbilityName}CoolDownTime";
				GameObject obj = GameObject.Find(objectName);
				if (obj != null)
					_image = obj.GetComponent<Image>();
			}

			if (_image != null)
				_imageMaterial = _image.material;
			else
				Debug.LogWarning($"[AbilityCooldownUI] '{_targetAbilityName}CoolDownTime' の Image が見つかりません。インスペクタで _image を設定してください。");
		}

		public void Tick()
		{
			if (_imageMaterial == null) return;

			float cooldownRemaining = _playerManager.GetAbilityCooldownRemaining(_targetUserId, _targetAbilityName);
			float percentage = cooldownRemaining > 0
				? 1.0f - (cooldownRemaining / _cooldownDuration)
				: 1.0f;

			_imageMaterial.SetFloat("_percentage", percentage);
		}
	}
}
