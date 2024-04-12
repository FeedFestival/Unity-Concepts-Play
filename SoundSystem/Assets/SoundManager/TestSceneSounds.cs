using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.Constants.AudioClips;

namespace Game.Sound
{
    public class TestSceneSounds : MonoBehaviour
    {
        private SceneSoundManager _sceneSoundManager;

        private void Awake()
        {
            _sceneSoundManager = GameObject.FindObjectOfType<SceneSoundManager>();
        }

        public void PlayAmbient()
        {
            _sceneSoundManager.PlayAmbient(Ambient.Mysterion);
        }

        public void PlayFX()
        {
            _sceneSoundManager.PlayFX(FX.Select);
        }
    }
}
