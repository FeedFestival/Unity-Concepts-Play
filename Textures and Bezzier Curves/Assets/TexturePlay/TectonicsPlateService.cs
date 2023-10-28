using Game.Shared.Classes;
using Game.Shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    public static class TectonicsPlateService
    {
        private static readonly int[] _gradients = new int[] { 0, 45, 90, 135, 180, 225, 270, 315 };

        internal static void AssignPixelsAndColor<T>(Vector2Int imgSize, Vector2Int gridSize, Vector2Int cellSize, ref Dictionary<int, Dictionary<int, T>> points)
        {
            for (int x = 0; x < imgSize.x; x++)  // Iterate over the width of the image.
            {
                for (int y = 0; y < imgSize.y; y++)  // Iterate over the height of the image.
                {
                    int gridX = x / cellSize.x;  // Determine the grid cell's Y index that contains the current pixel.
                    int gridY = y / cellSize.y;  // Determine the grid cell's X index that contains the current pixel.

                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    var nearestPoint = new Vector2Int();  // Initialize the closest point.

                    for (int a = -1; a < 2; a++)  // Iterate over the neighboring cells in X direction.
                    {
                        for (int b = -1; b < 2; b++)  // Iterate over the neighboring cells in Y direction.
                        {
                            int X = gridX + a;  // Calculate the X index of the neighboring cell.
                            int Y = gridY + b;  // Calculate the Y index of the neighboring cell.
                            if (X < 0 || Y < 0 || X >= gridSize.x || Y >= gridSize.y) continue;  // Skip if the neighbor cell is out of the grid bounds.

                            float distance = Vector2Int.Distance(new Vector2Int(x, y), (points[X][Y] as IPointBase).imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = new Vector2Int(X, Y);  // Update the nearest point.
                            }
                        }
                    }
                    var point = points[nearestPoint.x][nearestPoint.y] as IRegionPoint; // TODO: modify this into IPoint
                    var color = point.color;
                    var pixel = new Pixel(x, y, color);
                    point.pixels.AddPixel(pixel);
                }
            }
        }

        internal static void ModifyAllPixels(ref IPoint point, Color color)
        {
            foreach (var pixelsCols in point.pixels)
            {
                foreach (var pixel in pixelsCols.Value)
                {
                    pixel.Value.color = color;
                }
            }
        }

        internal static Vector2Int GetVoronoiEdgeOppositeCoord(VoronoiEdge edge, Vector2Int coord)
        {
            switch (edge)
            {
                case VoronoiEdge.Right:
                    return new Vector2Int(0, coord.y);
                case VoronoiEdge.InnerRight:
                    return new Vector2Int(1, coord.y);
                case VoronoiEdge.MiddleRight:
                    return new Vector2Int(2, coord.y);
                default:
                    return new Vector2Int(coord.x, 1);
            }
        }

        internal static Vector2Int GetVoronoiEdgeNewImagePos(VoronoiEdge edge, IPointBase oppositePoint, IPointBase sidePoint, Vector2Int cellSize)
        {
            Vector2Int point;
            switch (edge)
            {
                case VoronoiEdge.Right:
                    return new Vector2Int(
                        (sidePoint.gridPosition.x + cellSize.x) - oppositePoint.imagePosition.x,
                        oppositePoint.imagePosition.y
                    );
                case VoronoiEdge.InnerRight:
                    point = new Vector2Int((sidePoint.gridPosition.x + oppositePoint.imagePosition.x) - cellSize.x, oppositePoint.imagePosition.y);
                    return new Vector2Int(
                        (sidePoint.gridPosition.x + cellSize.x) - (point.x - sidePoint.gridPosition.x),
                        point.y
                    );
                case VoronoiEdge.MiddleRight:
                    point = new Vector2Int((sidePoint.gridPosition.x + oppositePoint.imagePosition.x) - (cellSize.x * 2), oppositePoint.imagePosition.y);
                    return new Vector2Int(
                        (sidePoint.gridPosition.x + cellSize.x) - (point.x - sidePoint.gridPosition.x),
                        point.y
                    );
                default:
                    point = new Vector2Int(oppositePoint.imagePosition.x, (sidePoint.gridPosition.y + oppositePoint.imagePosition.y) - cellSize.y);
                    return new Vector2Int(
                        point.x,
                        (sidePoint.gridPosition.y + cellSize.y) - (point.y - sidePoint.gridPosition.y)
                    );
            }
        }

        internal static void SetPointOnEdge(Vector2Int gridSize, ref IPointBase point)
        {
            if (point.gridCoord.x == 0)
            {
                point.edge = VoronoiEdge.Left;
            }
            else if (point.gridCoord.x == gridSize.x - 1)
            {
                point.edge = VoronoiEdge.Right;
            }
            else if (point.gridCoord.y == gridSize.y - 1)
            {
                point.edge = VoronoiEdge.Middle;
            }
            else if (point.gridCoord.y == 0)
            {
                point.edge = VoronoiEdge.Middle;
            }
            else
            {
                if (point.gridCoord.x == 1)
                {
                    point.edge = VoronoiEdge.InnerLeft;
                }
                else if (point.gridCoord.x == gridSize.x - 2)
                {
                    point.edge = VoronoiEdge.InnerRight;
                }
                else if (point.gridCoord.y == gridSize.y - 2)
                {
                    point.edge = VoronoiEdge.Middle;
                }
                else if (point.gridCoord.y == 1)
                {
                    point.edge = VoronoiEdge.Middle;
                }
                else
                {
                    if (point.gridCoord.x == 2)
                    {
                        point.edge = VoronoiEdge.MiddleLeft;
                    }
                    else if (point.gridCoord.x == gridSize.x - 3)
                    {
                        point.edge = VoronoiEdge.MiddleRight;
                    }
                    else if (point.gridCoord.y == gridSize.y - 3)
                    {
                        point.edge = VoronoiEdge.Middle;
                    }
                    else if (point.gridCoord.y == 2)
                    {
                        point.edge = VoronoiEdge.Middle;
                    }
                    else
                    {
                        point.edge = VoronoiEdge.Middle;
                    }
                }
            }
        }

        internal static Vector2Int[] GetGradientSquarePixelCoords(IContinentPoint continentPoint)
        {
            int sx = int.MaxValue;
            int sy = int.MaxValue;
            int bx = int.MinValue;
            int by = int.MinValue;

            foreach (var subColXKvp in continentPoint.SubPoints)
            {
                foreach (var subRowKvp in subColXKvp.Value)
                {
                    var rPoint = subRowKvp.Value;
                    TectonicsPlateService.setMinMax(ref sx, ref sy, ref bx, ref by, ref rPoint);
                }
            }

            return new Vector2Int[2] { new Vector2Int(sx, sy), new Vector2Int(bx, by) };
        }

        //internal static void setMinMax(ref int sx, ref int sy, ref int bx, ref int by, ref Dictionary<int, Dictionary<int, Pixel>> pixels)
        internal static void setMinMax(ref int sx, ref int sy, ref int bx, ref int by, ref IRegionPoint point)
        {
            foreach (var pixelColKvp in point.pixels)
            {
                var x = pixelColKvp.Key;
                if (x < sx)
                {
                    sx = x;
                }
                if (x > bx)
                {
                    bx = x;
                }
                foreach (var pixelKvp in pixelColKvp.Value)
                {
                    var y = pixelKvp.Key;
                    if (y < sy)
                    {
                        sy = y;
                    }
                    if (y > by)
                    {
                        by = y;
                    }
                }
            }
        }

        internal static int GetRandomDegrees()
        {
            var randomIndex = UnityEngine.Random.Range(0, _gradients.Length);
            return _gradients[randomIndex];
        }
    }
}