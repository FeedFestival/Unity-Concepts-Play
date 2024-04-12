using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Constants
{
    public static class AudioClips
    {
        public enum Ambient
        {
            Mysterion
        }

        public enum FX
        {
            Select
        }

        public static Dictionary<Ambient, string> AmbientAudioClips = new Dictionary<Ambient, string>() {
            {
                Ambient.Mysterion,
                "mysterion-low-ship-humming-25686" // .mp3
            }
        };

        public static Dictionary<FX, string> FXAudioClips = new Dictionary<FX, string>() {
            {
                FX.Select,
                "select_2-96163" // .mp3
            }
        };

    }
}