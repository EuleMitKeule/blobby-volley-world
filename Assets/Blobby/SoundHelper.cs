using Blobby.Game;
using Blobby.Networking;
using UnityEngine;

namespace Blobby
{
	public static class SoundHelper
	{
		public enum SoundClip { WallHit, Whistle, Bomb, Explosion };

		static AudioClip _wallHitClip, _whistleClip, _bombClip, _explosionClip;

		static AudioSource _wallHit, _whistle, _bomb, _explosion;

		static float _lastWallSound;
		static float _wallSoundThreshold = 0.1f;

		static GameObject _soundObject;

		[RuntimeInitializeOnLoadMethod]
		static void Initialize()
		{
			if (ServerHandler.IsServer) return;

			_soundObject = new GameObject("sound");

			_wallHitClip = Resources.Load<AudioClip>("Sounds/wallHit");
			_bombClip = Resources.Load<AudioClip>("Sounds/bomb");
			_explosionClip = Resources.Load<AudioClip>("Sounds/explosion");
			_whistleClip = Resources.Load<AudioClip>("Sounds/whistle");

			_wallHit = _soundObject.AddComponent<AudioSource>();
			_wallHit.clip = _wallHitClip;

			_whistle = _soundObject.AddComponent<AudioSource>();
			_whistle.clip = _whistleClip;

			_bomb = _soundObject.AddComponent<AudioSource>();
			_bomb.clip = _bombClip;

			_explosion = _soundObject.AddComponent<AudioSource>();
			_explosion.clip = _explosionClip;

			ClientConnection.SoundReceived += PlayAudio;
		}

		public static void PlayAudio(SoundClip soundClip)
		{
			switch (soundClip)
			{
				case SoundClip.WallHit:
					if (Time.time > _lastWallSound)
					{
						_lastWallSound = Time.time + _wallSoundThreshold;
						_wallHit.Play();
					}
					break;
				case SoundClip.Whistle:
					_whistle.Play();
					break;
				case SoundClip.Bomb:
					_bomb.Play();
					break;
				case SoundClip.Explosion:
					_explosion.Play();
					break;
				default:
					break;
			}
		}
	}
}

