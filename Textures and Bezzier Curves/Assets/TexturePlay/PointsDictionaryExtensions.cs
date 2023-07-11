using Game.Shared.Classes;
using Game.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace TexturePlay
{
    public static class PointsDictionaryExtensions
    {
        public static Dictionary<VoronoiEdge, List<Point>> GetEdgesPoints(this Dictionary<int, Dictionary<int, Point>> dictionary)
        {
            var voronoiEdges = new Dictionary<VoronoiEdge, List<Point>>()
            {
                { VoronoiEdge.Left, new List<Point>() },
                { VoronoiEdge.Top, new List<Point>() },
                { VoronoiEdge.Bottom, new List<Point>() },
                { VoronoiEdge.Middle, new List<Point>() },
                { VoronoiEdge.Right, new List<Point>() },
                { VoronoiEdge.InnerLeft, new List<Point>() },
                { VoronoiEdge.InnerRight, new List<Point>() },
                { VoronoiEdge.InnerBottom, new List<Point>() },
                { VoronoiEdge.InnerTop, new List<Point>() },
                { VoronoiEdge.MiddleLeft, new List<Point>() },
                { VoronoiEdge.MiddleRight, new List<Point>() },
                { VoronoiEdge.MiddleTop, new List<Point>() },
                { VoronoiEdge.MiddleBottom, new List<Point>() },
            };
            for (int x = 0; x < dictionary.Count; x++)
            {
                for (int y = 0; y < dictionary.ElementAt(x).Value.Count; y++)
                {
                    voronoiEdges[dictionary[x][y].edge].Add(dictionary[x][y]);
                }
            }
            return voronoiEdges;
        }

        public static int PointsCount(this Dictionary<int, Dictionary<int, Point>> dictionary)
        {
            int count = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                count += dictionary.ElementAt(i).Value.Count;
            }
            return count;
        }

        public static void AddPoint(this Dictionary<int, Dictionary<int, Point>> dictionary, Point value)
        {
            if (!dictionary.ContainsKey(value.gridCoord.x))
            {
                dictionary.Add(value.gridCoord.x, new Dictionary<int, Point>());
            }
            dictionary[value.gridCoord.x].Add(value.gridCoord.y, value);
        }

        public static void AddPoint(this Dictionary<int, Dictionary<int, Point>> dictionary, int key1, int key2, Point value)
        {
            if (!dictionary.ContainsKey(key1))
            {
                dictionary.Add(key1, new Dictionary<int, Point>());
            }
            dictionary[key1].Add(key2, value);
        }

        public static void AddPixel(this Dictionary<int, Dictionary<int, Pixel>> dictionary, Pixel value)
        {
            if (!dictionary.ContainsKey(value.coord.x))
            {
                dictionary.Add(value.coord.x, new Dictionary<int, Pixel>());
            }
            dictionary[value.coord.x].Add(value.coord.y, value);
        }

        public static bool ContainsPixel(this Dictionary<int, Dictionary<int, Pixel>> dictionary, Pixel value)
        {
            if (dictionary.ContainsKey(value.coord.x))
            {
                if (dictionary[value.coord.x].ContainsKey(value.coord.y))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}