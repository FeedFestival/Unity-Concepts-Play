using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GradientSettings", order = 1)]
public class GradientSettingsSO : ScriptableObject
{
    public List<GradientPreset> Gradients;

    [Serializable]
    public struct GradientPreset
    {
        public int Degrees;
        public Texture2D Texture;
    }

    public Texture2D GetGradientTexture(int degrees)
    {
        return Gradients.Find(g => g.Degrees == degrees).Texture;
    }
}
