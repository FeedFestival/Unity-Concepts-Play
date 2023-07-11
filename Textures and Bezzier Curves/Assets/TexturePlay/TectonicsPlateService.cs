using Game.Shared.Classes;
using Game.Shared.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    public static class TectonicsPlateService
    {
        internal static void AssignPixelsAndColor(Vector2Int imgSize, Vector2Int gridSize, Vector2Int cellSize, ref Dictionary<int, Dictionary<int, Point>> points)
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

                            float distance = Vector2Int.Distance(new Vector2Int(x, y), points[X][Y].imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = new Vector2Int(X, Y);  // Update the nearest point.
                            }
                        }
                    }

                    var color = points[nearestPoint.x][nearestPoint.y].hasDeadPixels
                        ? Color.black
                        : points[nearestPoint.x][nearestPoint.y].color;

                    var pixel = new Pixel(x, y, color);

                    points[nearestPoint.x][nearestPoint.y].pixels.AddPixel(pixel);
                }
            }
        }

        internal static void ModifyAllPixels(ref Dictionary<int, Dictionary<int, Pixel>> pixels, Color color)
        {
            foreach (var pixelsCols in pixels)
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
                case VoronoiEdge.Top:
                    return new Vector2Int(coord.x, 0);
                case VoronoiEdge.Right:
                    return new Vector2Int(0, coord.y);
                case VoronoiEdge.InnerRight:
                    return new Vector2Int(1, coord.y);
                case VoronoiEdge.MiddleRight:
                    return new Vector2Int(2, coord.y);
                case VoronoiEdge.MiddleTop:
                    return new Vector2Int(coord.x, 2);
                case VoronoiEdge.InnerTop:
                default:
                    return new Vector2Int(coord.x, 1);
            }
        }

        internal static Vector2Int GetVoronoiEdgeNewImagePos(VoronoiEdge edge, Point oppositePoint, Point sidePoint, Vector2Int cellSize)
        {
            Vector2Int point;
            switch (edge)
            {
                case VoronoiEdge.Top:
                    return new Vector2Int(oppositePoint.imagePosition.x, (sidePoint.gridPosition.y + cellSize.y) - oppositePoint.imagePosition.y);
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
                case VoronoiEdge.MiddleTop:
                    point = new Vector2Int(oppositePoint.imagePosition.x, (sidePoint.gridPosition.y + oppositePoint.imagePosition.y) - (cellSize.y * 2));
                    return new Vector2Int(
                        point.x,
                        (sidePoint.gridPosition.y + cellSize.y) - (point.y - sidePoint.gridPosition.y)
                    );
                case VoronoiEdge.InnerTop:
                default:
                    point = new Vector2Int(oppositePoint.imagePosition.x, (sidePoint.gridPosition.y + oppositePoint.imagePosition.y) - cellSize.y);
                    return new Vector2Int(
                        point.x,
                        (sidePoint.gridPosition.y + cellSize.y) - (point.y - sidePoint.gridPosition.y)
                    );
            }
        }

        private static bool IsHiddenPixel(VoronoiEdge edge)
        {
            return edge == VoronoiEdge.Left
                || edge == VoronoiEdge.Top
                || edge == VoronoiEdge.Bottom
                //|| edge == VoronoiEdge.Right
                ;
        }

    }
}