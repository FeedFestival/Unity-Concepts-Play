using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    public class VoronoiCell
    {
        private List<Vector2Int> points = new List<Vector2Int>();

        public void AddPoint(Vector2Int point)
        {
            points.Add(point);
        }

        public Vector2Int GetCentroid()
        {
            int totalX = 0;
            int totalY = 0;

            foreach (Vector2Int point in points)
            {
                totalX += point.x;
                totalY += point.y;
            }

            return new Vector2Int(totalX / points.Count, totalY / points.Count);
        }
    }

    public static class TectonicsPlateService
    {
        public static Dictionary<int, Dictionary<int, Point>> ApplyLloydRelaxation(Vector2Int imgSize, Vector2Int cellSize, Dictionary<int, Dictionary<int, Point>> pointsDictionary, int iterations)
        {
            // Copy the input points to a new Dictionary which will be transformed by the algorithm
            Dictionary<int, Dictionary<int, Point>> relaxedPoints = new Dictionary<int, Dictionary<int, Point>>(pointsDictionary);

            // Create an array to hold the Voronoi cell data
            VoronoiCell[,] cells = new VoronoiCell[imgSize.x, imgSize.y]; // Assume width and height are defined elsewhere

            for (int iteration = 0; iteration < iterations; iteration++)
            {
                // Make a deep copy of relaxedPoints for this iteration
                var copiedRelaxedPoints = new Dictionary<int, Dictionary<int, Point>>(relaxedPoints.Count);
                foreach (var pair in relaxedPoints)
                {
                    copiedRelaxedPoints[pair.Key] = new Dictionary<int, Point>(pair.Value);
                }

                // Reset Voronoi cells
                for (int x = 0; x < imgSize.x; x++)
                {
                    for (int y = 0; y < imgSize.y; y++)
                    {
                        cells[x, y] = new VoronoiCell();
                    }
                }

                // For each point, determine which cell it belongs to and update the cell data
                foreach (var outerDict in copiedRelaxedPoints)
                {
                    foreach (var pointPair in outerDict.Value)
                    {
                        var point = pointPair.Value;
                        int closestCellX = point.gridPosition.x / cellSize.x; // Assuming cellSize is defined elsewhere
                        int closestCellY = point.gridPosition.y / cellSize.y;

                        cells[closestCellX, closestCellY].AddPoint(point.gridPosition);
                    }
                }

                // Create a new dictionary to hold the modified points for this iteration
                Dictionary<int, Dictionary<int, Point>> modifiedPoints = new Dictionary<int, Dictionary<int, Point>>();

                // Update each point to be the centroid of its cell
                foreach (var outerDict in copiedRelaxedPoints)
                {
                    foreach (var pointPair in outerDict.Value)
                    {
                        var point = pointPair.Value;
                        Vector2Int cellPosition = new Vector2Int(point.gridPosition.x / cellSize.x, point.gridPosition.y / cellSize.y);
                        Vector2Int newCentroid = cells[cellPosition.x, cellPosition.y].GetCentroid();

                        var newPoint = new Point(
                            point.index,
                            point.gridCoord,
                            newCentroid,
                            cellSize,
                            point.color,
                            point.edge
                        );

                        // Add modified point to the copy
                        if (!modifiedPoints.ContainsKey(outerDict.Key))
                        {
                            modifiedPoints[outerDict.Key] = new Dictionary<int, Point>();
                        }
                        modifiedPoints[outerDict.Key][pointPair.Key] = newPoint;
                    }
                }

                // Replace the original points with the modified points
                relaxedPoints = modifiedPoints;
            }

            return relaxedPoints;
        }

        //public static Dictionary<int, Dictionary<int, Point>> ApplyLloydRelaxation(Vector2Int imgSize, Vector2Int cellSize, Dictionary<int, Dictionary<int, Point>> pointsDictionary, int iterations)
        //{
        //    // Copy the input points to a new Dictionary which will be transformed by the algorithm
        //    Dictionary<int, Dictionary<int, Point>> relaxedPoints = new Dictionary<int, Dictionary<int, Point>>(pointsDictionary);

        //    // Create an array to hold the Voronoi cell data
        //    VoronoiCell[,] cells = new VoronoiCell[imgSize.x, imgSize.y]; // Assume width and height are defined elsewhere

        //    for (int iteration = 0; iteration < iterations; iteration++)
        //    {
        //        // Reset Voronoi cells
        //        for (int x = 0; x < imgSize.x; x++)
        //        {
        //            for (int y = 0; y < imgSize.y; y++)
        //            {
        //                cells[x, y] = new VoronoiCell();
        //            }
        //        }

        //        // For each point, determine which cell it belongs to and update the cell data
        //        foreach (var outerDict in relaxedPoints)
        //        {
        //            foreach (var pointPair in outerDict.Value)
        //            {
        //                var point = pointPair.Value;
        //                int closestCellX = point.gridPosition.x / cellSize.x; // Assuming cellSize is defined elsewhere
        //                int closestCellY = point.gridPosition.y / cellSize.y;

        //                cells[closestCellX, closestCellY].AddPoint(point.gridPosition);
        //            }
        //        }

        //        // Update each point to be the centroid of its cell
        //        foreach (var outerDict in relaxedPoints)
        //        {
        //            foreach (var pointPair in outerDict.Value)
        //            {
        //                var point = pointPair.Value;
        //                Vector2Int cellPosition = new Vector2Int(point.gridPosition.x / cellSize.x, point.gridPosition.y / cellSize.y);
        //                Vector2Int newCentroid = cells[cellPosition.x, cellPosition.y].GetCentroid();

        //                var newPoint = new Point(
        //                    point.index,
        //                    point.gridCoord,
        //                    newCentroid,
        //                    cellSize,
        //                    point.color,
        //                    point.edge
        //                );

        //                // Modify the point directly in the dictionary
        //                outerDict.Value[pointPair.Key] = newPoint;  // Assuming that Point has a constructor that accepts a Vector2Int and a type.
        //            }
        //        }
        //    }

        //    return relaxedPoints;
        //}
    }
}