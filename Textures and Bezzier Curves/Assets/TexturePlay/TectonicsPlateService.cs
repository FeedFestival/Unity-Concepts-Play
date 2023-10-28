using Game.Shared.Classes;
using Game.Shared.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TexturePlay
{
    public static class TectonicsPlateService
    {
        private static readonly int[] _gradients = new int[] { 0, 22, 45, 67, 90, 112, 135, 157, 180, 202, 225, 247, 270, 292, 315, 337 };

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
                    var point = points[nearestPoint.x][nearestPoint.y] as IPoint; // TODO: modify this into IPoint
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
                //point.edge = VoronoiEdge.Left;
                //point.edge = VoronoiEdge.Middle;
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
                    //point.edge = VoronoiEdge.InnerLeft;
                    //point.edge = VoronoiEdge.Middle;
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
                        //point.edge = VoronoiEdge.MiddleLeft;
                        //point.edge = VoronoiEdge.Middle;
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

            foreach (var rxKvp in continentPoint.RegionPoints)
            {
                foreach (var ryKvp in rxKvp.Value)
                {
                    foreach (var subColXKvp in ryKvp.Value.Points)
                    {
                        foreach (var subRowKvp in subColXKvp.Value)
                        {
                            var rPoint = subRowKvp.Value;
                            TectonicsPlateService.setMinMax(ref sx, ref sy, ref bx, ref by, ref rPoint);
                        }
                    }
                }
            }

            return new Vector2Int[2] { new Vector2Int(sx, sy), new Vector2Int(bx, by) };
        }

        //internal static void setMinMax(ref int sx, ref int sy, ref int bx, ref int by, ref Dictionary<int, Dictionary<int, Pixel>> pixels)
        internal static void setMinMax(ref int sx, ref int sy, ref int bx, ref int by, ref IPoint point)
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

        public static Texture2D EqualizeGrayscale(Texture2D original)
        {
            // Make a copy of the original image to avoid modifying it directly
            Texture2D equalized = new Texture2D(original.width, original.height);

            // Compute the histogram
            int[] histogram = new int[256];
            for (int y = 0; y < original.height; y++)
            {
                for (int x = 0; x < original.width; x++)
                {
                    int grayscale = (int)(original.GetPixel(x, y).grayscale * 255);
                    histogram[grayscale]++;
                }
            }

            // Compute the CDF
            int[] cdf = new int[256];
            cdf[0] = histogram[0];
            for (int i = 1; i < 256; i++)
            {
                cdf[i] = cdf[i - 1] + histogram[i];
            }

            // Normalize the CDF
            int cdfMin = cdf.Min();
            int cdfMax = cdf.Max();
            for (int i = 0; i < 256; i++)
            {
                cdf[i] = (int)(((cdf[i] - cdfMin) / (float)(cdfMax - cdfMin)) * 255);
            }

            // Replace the intensities
            for (int y = 0; y < original.height; y++)
            {
                for (int x = 0; x < original.width; x++)
                {
                    int grayscale = (int)(original.GetPixel(x, y).grayscale * 255);
                    int equalizedGrayscale = cdf[grayscale];
                    equalized.SetPixel(x, y, new Color(equalizedGrayscale / 255f, equalizedGrayscale / 255f, equalizedGrayscale / 255f));
                }
            }

            // Apply changes to the texture
            equalized.Apply();

            return equalized;
        }

        internal static Texture2D AverageOutTexture(Texture2D source, int iterations = 1, int neighborDepth = 1)
        {
            // Clone the source texture
            Texture2D result = new Texture2D(source.width, source.height);
            result.SetPixels(source.GetPixels());
            result.Apply();

            // Get the pixels from the texture
            Color[] pixels = result.GetPixels();
            Color[] modifiedPixels = new Color[pixels.Length];

            for (int i = 0; i < iterations; i++)
            {
                for (int y = 0; y < source.height; y++)
                {
                    for (int x = 0; x < source.width; x++)
                    {
                        // Calculate the index in the linear pixel array
                        int index = GetIndexAtCoord(x, y, source.width);

                        ColorHSV color = pixels[index];

                        if (color.v < 0.1f || color.v > 0.9f)
                        {
                            // Set the new pixel value to the average brightness of its neighbors
                            float avgBrightness = GetAverageNeighborBrightness(x, y, source.width, source.height, pixels, neighborDepth);
                            modifiedPixels[index] = new Color(
                                avgBrightness,
                                avgBrightness,
                                avgBrightness,
                                pixels[index].a // Preserve original alpha
                            );
                        }
                        else
                        {
                            modifiedPixels[index] = color; // Keep the original color
                        }
                    }
                }
            }

            // Set the modified pixels on the texture and apply the changes
            result.SetPixels(modifiedPixels);
            result.Apply();

            return result;
        }

        private static float GetAverageNeighborBrightness(int x, int y, int width, int height, Color[] pixels, int neighborDepth = 1)
        {
            float totalBrightness = 0.0f;
            int count = 0;

            for (int i = -neighborDepth; i <= neighborDepth; i++)
            {
                for (int j = -neighborDepth; j <= neighborDepth; j++)
                {
                    // Ignore the pixel itself
                    if (i == 0 && j == 0) continue;

                    // Calculate the neighbor's coordinates
                    int neighborX = x + i;
                    int neighborY = y + j;

                    // Skip if the neighbor is out of bounds
                    if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height) continue;

                    // Calculate the index in the linear pixel array
                    int index = neighborY * width + neighborX;

                    // Add the grayscale value of the neighbor pixel to the total
                    totalBrightness += pixels[index].grayscale;
                    count++;
                }
            }

            // Calculate and return the average brightness
            return totalBrightness / count;
        }

        // Define the kernel for the Gaussian blur
        private static readonly float[] blurKernel = new float[]
        {
        1.0f / 273,  4.0f / 273,  7.0f / 273,  4.0f / 273,  1.0f / 273,
        4.0f / 273, 16.0f / 273, 26.0f / 273, 16.0f / 273,  4.0f / 273,
        7.0f / 273, 26.0f / 273, 41.0f / 273, 26.0f / 273,  7.0f / 273,
        4.0f / 273, 16.0f / 273, 26.0f / 273, 16.0f / 273,  4.0f / 273,
        1.0f / 273,  4.0f / 273,  7.0f / 273,  4.0f / 273,  1.0f / 273
        };

        internal static Texture2D ApplyGaussianBlur(Texture2D source, int iterations = 1)
        {
            // Clone the source texture
            Texture2D result = new Texture2D(source.width, source.height);
            result.SetPixels(source.GetPixels());
            result.Apply();

            // Get the pixels from the texture
            Color[] pixels = result.GetPixels();
            Color[] modifiedPixels = new Color[pixels.Length];

            for (int i = 0; i < iterations; i++)
            {
                for (int y = 0; y < source.height; y++)
                {
                    for (int x = 0; x < source.width; x++)
                    {
                        // Calculate the index in the linear pixel array
                        int index = GetIndexAtCoord(x, y, source.width);

                        // Apply the convolution filter to the current pixel
                        ColorHSV color = pixels[index];

                        if (color.v < 0.2f || color.v > 0.8f)
                        {
                            float brightness = ApplyKernel(x, y, source.width, source.height, pixels, blurKernel);
                            modifiedPixels[index] = new Color(
                                brightness,
                                brightness,
                                brightness,
                                pixels[index].a // Preserve original alpha
                            );
                        }
                        else
                        {
                            modifiedPixels[index] = color; // Keep the original color
                        }
                    }
                }
            }

            // Set the modified pixels on the texture and apply the changes
            result.SetPixels(modifiedPixels);
            result.Apply();

            return result;
        }

        private static float ApplyKernel(int x, int y, int width, int height, Color[] pixels, float[] kernel)
        {
            float totalBrightness = 0.0f;

            for (int i = -2; i <= 2; i++)
            {
                for (int j = -2; j <= 2; j++)
                {
                    // Calculate the neighbor's coordinates
                    int neighborX = Mathf.Clamp(x + i, 0, width - 1);
                    int neighborY = Mathf.Clamp(y + j, 0, height - 1);

                    // Calculate the index in the linear pixel array
                    int index = neighborY * width + neighborX;

                    // Apply the kernel to the brightness of the neighbor pixel
                    totalBrightness += pixels[index].grayscale * kernel[(i + 2) + (j + 2) * 5];
                }
            }

            return totalBrightness;
        }

        internal static float GetRandomTectonicElevation()
        {
            return UnityEngine.Random.Range(0.2f, 0.7f);
        }

        internal static int GetRandomDegrees()
        {
            var randomIndex = UnityEngine.Random.Range(0, _gradients.Length);
            return _gradients[randomIndex];
        }

        internal static int GetIndexAtCoord(int x, int y, int width)
        {
            return y * width + x;
        }

        internal static void GetCoordAtIndex(int index, int width, out int x, out int y)
        {
            y = index / width;
            x = index % width;
        }
    }
}