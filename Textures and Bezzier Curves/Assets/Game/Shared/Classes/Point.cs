using UnityEngine;
using Game.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Game.Shared.Classes
{
    //[Serializable]
    public class Point
    {
        public int index;
        public Vector2Px gridCoord;
        public Vector2Px gridPosition;
        public Vector2Px imagePosition;
        public VoronoiEdge edge;
        public Color color;
        public Dictionary<int, Dictionary<int, Pixel>> pixels;
        public bool hasDeadPixels;

        public Point(int i, Vector2Px gridCoord, Vector2Px imagePos, Vector2Px cellSize, Color color)
        {
            index = i;
            this.gridCoord = gridCoord;
            gridPosition = new Vector2Px(this.gridCoord.x * cellSize.x, this.gridCoord.y * cellSize.y);
            imagePosition = imagePos;

            this.color = color;
            edge = VoronoiEdge.Middle;

            pixels = new Dictionary<int, Dictionary<int, Pixel>>();
        }

        public void updatePoint(Vector2Px imagePos, Vector2Px cellSize, Color color)
        {
            gridPosition = new Vector2Px(gridCoord.x * cellSize.x, gridCoord.y * cellSize.y);
            imagePosition = imagePos;

            this.color = color;
        }
    }

    //[Serializable]
    public class Pixel
    {
        public Vector2Int coord;
        public Color color;

        public Pixel(int x, int y, Color color)
        {
            this.coord = new Vector2Px(x, y);
            this.color = color;
        }

        public override string ToString()
        {
            return "coord: " + coord.ToString() + @",
"                + "color: " + color.ToString() + @"
";
        }
    }
}