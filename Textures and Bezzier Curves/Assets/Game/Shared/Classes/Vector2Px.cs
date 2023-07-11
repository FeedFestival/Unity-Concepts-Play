using System;
using UnityEngine;

namespace Game.Shared.Classes
{
    /// <summary>
    /// Since unity doesn't flag the Vector3 as serializable, we
    /// need to create our own version. This one will automatically convert
    /// between Vector3 and Vector2Px
    /// </summary>
    [System.Serializable]
    public struct Vector2Px
    {
        /// <summary>
        /// x component
        /// </summary>
        public int x;

        /// <summary>
        /// y component	
        /// </summary>
        public int y;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rX"></param>
        /// <param name="rY"></param>
        public Vector2Px(int rX, int rY)
        {
            x = rX;
            y = rY;
        }

        /// <summary>
        /// Returns a string representation of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("[{0}, {1}]", x, y);
        }

        /// <summary>
        /// Automatic conversion from Vector2Px to Vector3
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector2Int(Vector2Px rValue)
        {
            return new Vector2Int(rValue.x, rValue.y);
        }

        /// <summary>
        /// Automatic conversion from Vector3 to Vector2Px
        /// </summary>
        /// <param name="rValue"></param>
        /// <returns></returns>
        public static implicit operator Vector2Px(Vector2Int rValue)
        {
            return new Vector2Px(rValue.x, rValue.y);
        }
    }

    [System.Serializable]
    public struct PixelColor
    {
        public float _r;
        public float _g;
        public float _b;
        public float _a;

        public PixelColor(float r, float g, float b, float a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public override string ToString()
        {
            return String.Format("Color({0}, {1}, {2}, {3})", _r, _g, _b, _a);
        }

        public static implicit operator Color(PixelColor rValue)
        {
            return new Color(rValue._r, rValue._g, rValue._b, rValue._a);
        }

        public static implicit operator PixelColor(Color rValue)
        {
            return new PixelColor(rValue.r, rValue.g, rValue.b, rValue.a);
        }
    }
}
