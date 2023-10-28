using Game.Shared.Classes;
using Game.Shared.Enums;
using System.Collections.Generic;
using System.Linq;

namespace TexturePlay
{
    public static class PointsDictionaryExtensions
    {
        public static Dictionary<VoronoiEdge, List<T>> GetEdgesPoints<T>(this Dictionary<int, Dictionary<int, T>> dictionary) where T : IPointBase
        {
            var voronoiEdges = new Dictionary<VoronoiEdge, List<T>>()
            {
                { VoronoiEdge.Left, new List<T>() },
                { VoronoiEdge.Middle, new List<T>() },
                { VoronoiEdge.Right, new List<T>() },
                { VoronoiEdge.InnerLeft, new List<T>() },
                { VoronoiEdge.InnerRight, new List<T>() },
                { VoronoiEdge.MiddleLeft, new List<T>() },
                { VoronoiEdge.MiddleRight, new List<T>() }
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

        public static int PointsCount(this Dictionary<int, Dictionary<int, IPoint>> dictionary)
        {
            int count = 0;
            for (int i = 0; i < dictionary.Count; i++)
            {
                count += dictionary.ElementAt(i).Value.Count;
            }
            return count;
        }

        public static void AddPoint<T>(this Dictionary<int, Dictionary<int, T>> dictionary, T value) where T : IPointBase
        {
            if (!dictionary.ContainsKey(value.gridCoord.x))
            {
                dictionary.Add(value.gridCoord.x, new Dictionary<int, T>());
            }
            dictionary[value.gridCoord.x].Add(value.gridCoord.y, value);
        }

        public static void AddPixel(this Dictionary<int, Dictionary<int, Pixel>> dictionary, Pixel value)
        {
            if (!dictionary.ContainsKey(value.coord.x))
            {
                dictionary.Add(value.coord.x, new Dictionary<int, Pixel>());
            }
            dictionary[value.coord.x].Add(value.coord.y, value);
        }

        public static bool ContainsPixelCoords(this Dictionary<int, Dictionary<int, Pixel>> dictionary, int x, int y)
        {
            if (dictionary.ContainsKey(x))
            {
                if (dictionary[x].ContainsKey(y))
                {
                    return true;
                }
                return false;
            }
            return false;
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