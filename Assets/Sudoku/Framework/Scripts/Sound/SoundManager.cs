using System.Collections.Generic;
using dotmob;
using Sudoku.Framework.Scripts.Save;
using UnityEngine;

namespace Sudoku.Framework.Scripts.Sound
{
	public class SoundManager : SingletonComponent<SoundManager>, ISaveable
	{
		#region Classes

		[System.Serializable]
		public class SoundInfo
		{
			public string		id					= "";
			public AudioClip	audioClip			= null;
			public SoundType	type				= SoundType.SoundEffect;
			public bool			playAndLoopOnStart	= false;

			[Range(0, 1)] public float clipVolume = 1;
		}

		public class PlayingSound
		{
			public SoundInfo	SoundInfo	= null;
			public AudioSource	AudioSource	= null;
		}

		#endregion

		#region Enums

		public enum SoundType
		{
			SoundEffect,
			Music
		}

		#endregion


		public void PauseMusic()
		{
			if (audioSource != null && audioSource.isPlaying)
			{
				audioSource.Pause();
				currentMusicSource.Pause();
				Debug.Log("Music paused.");
			}
		}

		public void ResumeMusic()
		{
			if (audioSource != null && !audioSource.isPlaying)
			{				audioSource.UnPause();

				currentMusicSource.UnPause();
				Debug.Log("Music resumed.");
			}
		}



		#region Inspector Variables

		[SerializeField] private List<SoundInfo> soundInfos = null;

		#endregion

		#region Member Variables
		private AudioSource currentMusicSource = null; // Для хранения текущего музыкального источника
		private AudioSource audioSource = null;
		private List<PlayingSound> playingAudioSources;
		private List<PlayingSound> loopingAudioSources;

		public string SaveId { get { return "sound_manager"; } }

		#endregion

		#region Properties

		public bool IsMusicOn			{ get; private set; }
		public bool IsSoundEffectsOn	{ get; private set; }

		#endregion

		#region Unity Methods

		protected override void Awake()
		{
			base.Awake();

			SaveManager.Instance.Register(this);
    
			// Загружаем сохраненные значения
			bool isLoaded = LoadSave();

			// Если загрузка не удалась, устанавливаем значения по умолчанию
			if (!isLoaded)
			{
				Debug.Log("No saved data found, setting default values.");
				IsMusicOn = true; // или false, в зависимости от вашего дизайна
				IsSoundEffectsOn = true; // или false, в зависимости от вашего дизайна
			}
			else
			{
				Debug.Log("Saved data found, setting default values.");

			}


			playingAudioSources = new List<PlayingSound>();
			loopingAudioSources = new List<PlayingSound>();

			// Применяем настройки после загрузки
			if (!IsMusicOn)
			{
				Stop(SoundType.Music); // Останавливаем музыку, если она была выключена
			}
			else
			{
			//	StartPlayMusic(); // Воспроизводим музыку, если она включена
			}
		}


		public void StartPlayMusic()
		{
			for (int i = 0; i < soundInfos.Count; i++)
			{
				SoundInfo soundInfo = soundInfos[i];

				// Проверяем, должна ли музыка воспроизводиться при старте
				if (soundInfo.playAndLoopOnStart)
				{
					// Проверяем, воспроизводится ли уже музыка с этим ID
					bool isAlreadyPlaying = loopingAudioSources.Exists(sound => sound.SoundInfo.id == soundInfo.id);

					if (!isAlreadyPlaying)
					{
						Play(soundInfo.id, true, 0); // Воспроизводим только если музыка ещё не играет
					}
				}
			}
		}

		private void Update()
		{
			for (var i = 0; i < playingAudioSources.Count; i++)
			{
				var audioSource = playingAudioSources[i].AudioSource;

				// If the Audio Source is no longer playing then return it to the pool so it can be re-used
				if (!audioSource.isPlaying)
				{
					Destroy(audioSource.gameObject);
					playingAudioSources.RemoveAt(i);
					i--;
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Plays the sound with the give id
		/// </summary>
		public void Play(string id)
		{
			Play(id, false, 0);
		}

		/// <summary>
		/// Plays the sound with the give id, if loop is set to true then the sound will only stop if the Stop method is called
		/// </summary>
		public void Play(string id, bool loop, float playDelay)
		{
			SoundInfo soundInfo = GetSoundInfo(id);

			if (soundInfo == null)
			{
				Debug.LogError("[SoundManager] There is no Sound Info with the given id: " + id);

				return;
			}

			if ((soundInfo.type == SoundType.Music && !IsMusicOn) ||
			    (soundInfo.type == SoundType.SoundEffect && !IsSoundEffectsOn))
			{
				return;
			}

			 audioSource = CreateAudioSource(id);

			audioSource.clip	= soundInfo.audioClip;
			audioSource.loop	= loop;
			audioSource.time	= 0;
			audioSource.volume	= soundInfo.clipVolume;

			if (playDelay > 0)
			{
				audioSource.PlayDelayed(playDelay);
			}
			else
			{
				audioSource.Play();
			}

			PlayingSound playingSound = new PlayingSound();

			playingSound.SoundInfo		= soundInfo;
			playingSound.AudioSource	= audioSource;

			if (loop)
			{
				loopingAudioSources.Add(playingSound);
			}
			else
			{
				playingAudioSources.Add(playingSound);
			}
		}

		/// <summary>
		/// Stops all playing sounds with the given id
		/// </summary>
		public void Stop(string id)
		{
			StopAllSounds(id, playingAudioSources);
			StopAllSounds(id, loopingAudioSources);
		}

		/// <summary>
		/// Stops all playing sounds with the given type
		/// </summary>
		public void Stop(SoundType type)
		{
			StopAllSounds(type, playingAudioSources);
			StopAllSounds(type, loopingAudioSources);
		}

		/// <summary>
		/// Sets the SoundType on/off
		/// </summary>
		public void SetSoundTypeOnOff(SoundType type, bool isOn)
		{
			switch (type)
			{
				case SoundType.SoundEffect:
					if (isOn == IsSoundEffectsOn)
						return;
					IsSoundEffectsOn = isOn;
					break;
				case SoundType.Music:
					if (isOn == IsMusicOn)
						return;
					IsMusicOn = isOn;
					break;
			}

			if (!isOn)
			{
				Stop(type); // Останавливаем все звуки этого типа
			}
			else
			{
				PlayAtStart(type); // Воспроизводим все звуки этого типа, если они должны играть
			}

			SaveManager.Instance.Save(); // Сохраняем состояние
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Plays all sounds that are set to play on start and loop and are of the given type
		/// </summary>
		public void PlayAtStart(SoundType type)
		{
			for (int i = 0; i < soundInfos.Count; i++)
			{
				SoundInfo soundInfo = soundInfos[i];

				if (soundInfo.type == type && soundInfo.playAndLoopOnStart && soundInfo.playAndLoopOnStart == IsMusicOn)
				{
					Play(soundInfo.id, true, 0);
				}
			}
		}


		/// <summary>
		/// Stops all sounds with the given id
		/// </summary>
		public void StopAllSounds(string id, List<PlayingSound> playingSounds)
		{
			for (int i = 0; i < playingSounds.Count; i++)
			{
				PlayingSound playingSound = playingSounds[i];

				if (id == playingSound.SoundInfo.id)
				{
					playingSound.AudioSource.Stop();
					Destroy(playingSound.AudioSource.gameObject);
					playingSounds.RemoveAt(i);
					i--;
				}
			}
		}
		
		
		/// <summary>
		/// Stops all sounds with the given type
		/// </summary>
		private void StopAllSounds(SoundType type, List<PlayingSound> playingSounds)
		{
			for (int i = 0; i < playingSounds.Count; i++)
			{
				PlayingSound playingSound = playingSounds[i];

				if (type == playingSound.SoundInfo.type)
				{
					playingSound.AudioSource.Stop();
					Destroy(playingSound.AudioSource.gameObject);
					playingSounds.RemoveAt(i);
					i--;
				}
			}
		}

		private SoundInfo GetSoundInfo(string id)
		{
			for (int i = 0; i < soundInfos.Count; i++)
			{
				if (id == soundInfos[i].id)
				{
					return soundInfos[i];
				}
			}

			return null;
		}

		#region Member Variables


		#endregion

		#region Public Methods

		/// <summary>
		/// Plays music if it's not already playing.
		/// </summary>
		/// <summary>
		/// Plays music if it's not already playing.
		/// </summary>
		public void PlayMusic(string id)
		{
			// Проверяем, включена ли музыка
			if (!IsMusicOn)
			{
				Debug.Log("[SoundManager] Music is turned off.");
				return;
			}

			// Получаем информацию о звуке
			SoundInfo soundInfo = GetSoundInfo(id);
    
			if (soundInfo == null)
			{
				Debug.LogError("[SoundManager] There is no Sound Info with the given id: " + id);
				return;
			}

			// Если текущая музыка другая, останавливаем её
			if (currentMusicSource != null && currentMusicSource.isPlaying)
			{
				currentMusicSource.Stop();
			}

			// Создаем новый аудио источник и воспроизводим звук
			currentMusicSource = CreateAudioSource(id);
			currentMusicSource.clip = soundInfo.audioClip;
			currentMusicSource.loop = true; // Обычно музыка зацикливается
			currentMusicSource.volume = soundInfo.clipVolume;
			currentMusicSource.Play();
			
		}

		#endregion

		
		private AudioSource CreateAudioSource(string id)
		{
			GameObject obj = new GameObject("sound_" + id);

			obj.transform.SetParent(transform);

			return obj.AddComponent<AudioSource>();;
		}

		#endregion

		#region Save Methods

		public Dictionary<string, object> Save()
		{
			Dictionary<string, object> json = new Dictionary<string, object>();

			json["is_music_on"]			= IsMusicOn;
			json["is_sound_effects_on"]	= IsSoundEffectsOn;

			return json;
		}

		public bool LoadSave()
		{
			JSONNode json = SaveManager.Instance.LoadSave(this);

			if (json == null)
			{
				Debug.Log("No saved data found.");
				return false; // Если сохранений нет, возвращаем false
			}

			IsMusicOn = json["is_music_on"].AsBool;
			IsSoundEffectsOn = json["is_sound_effects_on"].AsBool;

			Debug.Log($"Loaded settings - IsMusicOn: {IsMusicOn}, IsSoundEffectsOn: {IsSoundEffectsOn}");

			return true; // Успешная загрузка
		}


		#endregion
	}
}
