using UnityEngine;

namespace picture_game_view.Assets.Modules.AudioSystem.Services
{
	/// <summary>
	/// BGMとSEを統一管理するオーディオマネージャー
	/// </summary>
	public class AudioManager : MonoBehaviour
	{
		private AudioSource _bgmSource;
		private AudioSource _seSource;

		private void Awake()
		{
			// BGM用AudioSource（ループ再生）
			_bgmSource = gameObject.AddComponent<AudioSource>();
			_bgmSource.loop = true;
			_bgmSource.playOnAwake = false;
			_bgmSource.volume = 0.5f; // BGMは少し小さめ

			// SE用AudioSource
			_seSource = gameObject.AddComponent<AudioSource>();
			_seSource.playOnAwake = false;
			_seSource.volume = 0.3f;

			Debug.Log("AudioManager initialized");
		}

		private void Start()
		{
			// 戦闘BGMを自動再生
			PlayBGM("Let_me_think_!");
		}

		/// <summary>
		/// BGMを再生する
		/// </summary>
		/// <param name="bgmName">Resourcesフォルダ内のSound/以下のファイル名（拡張子なし）</param>
		public void PlayBGM(string bgmName)
		{
			var clip = Resources.Load<AudioClip>($"Sound/{bgmName}");
			if (clip != null)
			{
				_bgmSource.clip = clip;
				_bgmSource.Play();
				Debug.Log($"BGM再生: {bgmName}");
			}
			else
			{
				Debug.LogWarning($"BGMのロードに失敗: Sound/{bgmName}");
			}
		}

		/// <summary>
		/// BGMを停止する
		/// </summary>
		public void StopBGM()
		{
			_bgmSource.Stop();
		}

		/// <summary>
		/// BGMの音量を設定する
		/// </summary>
		/// <param name="volume">音量（0.0～1.0）</param>
		public void SetBGMVolume(float volume)
		{
			_bgmSource.volume = Mathf.Clamp01(volume);
		}

		/// <summary>
		/// SEを再生する（ワンショット）
		/// </summary>
		/// <param name="seName">Resourcesフォルダ内のSound/以下のファイル名（拡張子なし）</param>
		public void PlaySE(string seName)
		{
			var clip = Resources.Load<AudioClip>($"Sound/{seName}");
			if (clip != null)
			{
				_seSource.PlayOneShot(clip);
			}
			else
			{
				Debug.LogWarning($"SEのロードに失敗: Sound/{seName}");
			}
		}

		/// <summary>
		/// SEの音量を設定する
		/// </summary>
		/// <param name="volume">音量（0.0～1.0）</param>
		public void SetSEVolume(float volume)
		{
			_seSource.volume = Mathf.Clamp01(volume);
		}

		/// <summary>
		/// BGMが再生中かどうか
		/// </summary>
		public bool IsBGMPlaying => _bgmSource != null && _bgmSource.isPlaying;
	}
}
