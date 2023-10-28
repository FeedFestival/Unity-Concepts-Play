using UnityEngine;

namespace TexturePlay
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TexturePlayDebugSettings", order = 1)]
    public class TexturePlayDebug : ScriptableObject
    {
        public GameObject Square;

        [Header("DebugColors")]
        public Material InnerEdgePoint;
        public Material OuterEdgePoint;
        public Material MiddlePoint;
        public Material GridPoint;
    }
}