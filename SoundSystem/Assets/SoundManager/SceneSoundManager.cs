using System.Collections.Generic;
using UnityEngine;
using static Game.Constants.AudioClips;

namespace Game.Sound
{
    public class SceneSoundManager : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _ambientAudioSource;
        [SerializeField]
        private AudioSource _fxAudioSource;

        [SerializeField]
        private List<Ambient> _ambientClipsName;

        [SerializeField]
        private List<FX> _fxClipsName;

        private Dictionary<Ambient, AudioClip> _ambientAudioClips;
        private Dictionary<FX, AudioClip> _fxAudioClips;

        private void Awake()
        {
            _ambientAudioClips = new Dictionary<Ambient, AudioClip>();
            foreach (var ambientClip in _ambientClipsName)
            {
                var path = "Sounds/Ambient/" + AmbientAudioClips[ambientClip];
                AudioClip audioClip = Resources.Load<AudioClip>(path);
                _ambientAudioClips.Add(ambientClip, audioClip);
            }

            _fxAudioClips = new Dictionary<FX, AudioClip>();
            foreach (var fxClip in _fxClipsName)
            {
                var path = "SoundFX/Ambient/" + FXAudioClips[fxClip];
                AudioClip audioClip = Resources.Load<AudioClip>(path);
                _fxAudioClips.Add(fxClip, audioClip);
            }
        }

        public void PlayAmbient(Ambient ambientClip, bool loop = false)
        {
            _ambientAudioSource.clip = _ambientAudioClips[ambientClip];
            _ambientAudioSource.loop = loop;
            _ambientAudioSource.Play();
        }

        public void PlayFX(FX fxClip)
        {
            if (_fxAudioClips.)
            {
                Debug.LogWarning("FX " + fxClip.ToString() + " is not declared to be loaded in the SceneSoundManager of this scene. Please add it if you intend to use it.");
                return;
            }
            _fxAudioSource.clip = _fxAudioClips[fxClip];
            _fxAudioSource.Play();
        }
    }
}
