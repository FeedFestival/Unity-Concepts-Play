using UnityEngine;
using Game.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Game.Shared.Classes
{
    public interface IPointBase
    {
        Vector2Px gridCoord { get; set; }
        Vector2Px gridPosition { get; set; }
        Vector2Px imagePosition { get; set; }
        VoronoiEdge edge { get; set; }
    }

    public interface IPoint: IPointBase
    {
        Color color { get; set; }
        Dictionary<int, Dictionary<int, Pixel>> pixels { get; set; }

        void updatePoint(Vector2Px imagePos, Vector2Px cellSize, Color color);
    }

    public interface IContinentPoint : IPointBase
    {
        float Elevation { get; set; }
        int PlateMovementDirection { get; set; }
        Vector2Int[] gradientSquarePixelCoords { get; set; }
        Dictionary<int, Dictionary<int, IRegionPoint>> RegionPoints { get; set; }

        void updatePoint(Vector2Px imagePos, Vector2Px cellSize, int plateMovementDirection);
    }

    public interface IRegionPoint : IPointBase
    {
        Dictionary<int, Dictionary<int, IPoint>> Points { get; set; }

        void updatePoint(Vector2Px imagePos, Vector2Px cellSize);
    }

    public class ContinentPoint : PointBase, IPointBase, IContinentPoint
    {
        public float Elevation { get; set; }
        public int PlateMovementDirection { get; set; }
        public Vector2Int[] gradientSquarePixelCoords { get; set; }
        public Dictionary<int, Dictionary<int, IRegionPoint>> RegionPoints { get; set; }

        public ContinentPoint() { }

        public ContinentPoint(int i, Vector2Px gridCoord, Vector2Px imagePos, Vector2Px cellSize, int plateMovementDirection, float elevation)
        {
            base.index = i;
            base.gridCoord = gridCoord;
            base.gridPosition = new Vector2Px(base.gridCoord.x * cellSize.x, base.gridCoord.y * cellSize.y);
            base.imagePosition = imagePos;

            base.edge = VoronoiEdge.Middle;

            RegionPoints = new Dictionary<int, Dictionary<int, IRegionPoint>>();
            PlateMovementDirection = plateMovementDirection;
            Elevation = elevation;
        }

        public void updatePoint(Vector2Px imagePos, Vector2Px cellSize, int plateMovementDirection)
        {
            gridPosition = new Vector2Px(gridCoord.x * cellSize.x, gridCoord.y * cellSize.y);
            imagePosition = imagePos;

            PlateMovementDirection = plateMovementDirection;
        }
    }

    public class RegionPoint : PointBase, IPointBase, IRegionPoint
    {
        public Dictionary<int, Dictionary<int, IPoint>> Points { get; set; }

        public RegionPoint() { }

        public RegionPoint(int i, Vector2Px gridCoord, Vector2Px imagePos, Vector2Px cellSize)
        {
            base.index = i;
            base.gridCoord = gridCoord;
            base.gridPosition = new Vector2Px(base.gridCoord.x * cellSize.x, base.gridCoord.y * cellSize.y);
            base.imagePosition = imagePos;

            base.edge = VoronoiEdge.Middle;

            Points = new Dictionary<int, Dictionary<int, IPoint>>();
        }

        public void updatePoint(Vector2Px imagePos, Vector2Px cellSize)
        {
            gridPosition = new Vector2Px(gridCoord.x * cellSize.x, gridCoord.y * cellSize.y);
            imagePosition = imagePos;
        }
    }

    //[Serializable]
    public class Point : PointBase, IPoint
    {
        public Color color { get; set; }
        public Dictionary<int, Dictionary<int, Pixel>> pixels { get; set; }

        public Point() { }

        public Point(int i, Vector2Px gridCoord, Vector2Px imagePos, Vector2Px cellSize, Color color)
        {
            base.index = i;
            this.gridCoord = gridCoord;
            base.gridPosition = new Vector2Px(base.gridCoord.x * cellSize.x, base.gridCoord.y * cellSize.y);
            base.imagePosition = imagePos;

            this.color = color;
            base.edge = VoronoiEdge.Middle;

            pixels = new Dictionary<int, Dictionary<int, Pixel>>();
        }

        public void updatePoint(Vector2Px imagePos, Vector2Px cellSize, Color color)
        {
            base.gridPosition = new Vector2Px(base.gridCoord.x * cellSize.x, base.gridCoord.y * cellSize.y);
            base.imagePosition = imagePos;

            this.color = color;
        }
    }

    public class PointBase: IPointBase
    {
        public int index;
        public Vector2Px gridCoord { get; set; }
        public Vector2Px gridPosition { get; set; }
        public Vector2Px imagePosition { get; set; }
        public VoronoiEdge edge { get; set; }
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
" + "color: " + color.ToString() + @"
";
        }
    }
}