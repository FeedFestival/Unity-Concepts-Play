using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TexturePlayDebugSettings", order = 1)]
    public class TexturePlayDebug : ScriptableObject
    {
        [Header("DebugColors")]
        public Material InnerEdgePoint;
        public Material OuterEdgePoint;
        public Material MiddlePoint;
        public Material GridPoint;
    }
}