using Zenject;
using UnityEngine;
using picture_game_view.Assets.Modules.Shared;
using picture_game_view.Assets.Modules.AudioSystem.Services;

namespace picture_game_view.Assets.Modules.AudioSystem.Installer
{
	public class AudioSystemInstaller : MonoInstaller
	{
		public override void InstallBindings()
		{
			// AudioManagerをMonoBehaviourとしてバインド
			InstallerHelper.BindMono<AudioManager>(Container);

			Debug.Log("AudioSystemInstaller: Bindings completed");
		}
	}
}
