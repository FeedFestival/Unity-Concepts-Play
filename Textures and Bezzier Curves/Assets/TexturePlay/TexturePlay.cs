using Game.Shared.PropertyDrawers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TexturePlay
{
    public class TexturePlay : MonoBehaviour
    {
        [Header("Settings Size(7680, 3840) Resolution(768, 384)")]
        [SerializeField]
        [ReadOnly]
        private Vector2Int _imgSize;
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridRegionSize;
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridContinentSize;
        [SerializeField]
        private int _regionDivider = 100;
        [SerializeField]
        private int _regionSliceMultiplier = 1;
        [SerializeField]
        private int _continentDivider = 10;

        [Header("Regions")]
        [SerializeField]
        private RawImage _regionsImage;

        [Header("Continent")]
        [SerializeField]
        private RawImage _continentsImage;
        [SerializeField]
        private RawImage _continentsPointsPreviewImage;

        [Header("Debug")]
        [SerializeField]
        private PlateTectonicsDebug _plateTectonicsDebug;

        private readonly Vector2Int SIZE = new Vector2Int(7680, 3840);
        private readonly int SIZE_RESOLUTION_DIVIDER = 10;

        void Start()
        {
            _imgSize = SIZE / SIZE_RESOLUTION_DIVIDER;
            _gridRegionSize = (SIZE / _regionDivider) / _regionSliceMultiplier;
            _gridContinentSize = _gridRegionSize / _continentDivider;

            Debug.Log("_gridSize: " + _gridRegionSize.ToString());

            var rt = _regionsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;

            //
            rt = _continentsPointsPreviewImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt = _continentsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;

            buildImage();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                buildImage();
            }
        }

        private void buildImage()
        {
            var cellSize = new Vector2Int(_imgSize.x / _gridRegionSize.x, _imgSize.y / _gridRegionSize.y);
            Debug.Log("cellSize: " + cellSize);
            var points = getSeamlessPoints(cellSize, _gridRegionSize);

            drawTexture(_imgSize, _gridRegionSize, cellSize, ref points, _regionsImage);

            //var relaxedPoints = TectonicsPlateService.ApplyLloydRelaxation(_imgSize, cellSize, points, 2);

            var continentCellSize = new Vector2Int(_imgSize.x / _gridContinentSize.x, _imgSize.y / _gridContinentSize.y);
            Debug.Log("continentCellSize: " + continentCellSize);
            var continentPoints = getSeamlessPoints(continentCellSize, _gridContinentSize, blackAndWhiteColor: false);
            var continentRegionPoints = new Dictionary<int, Dictionary<int, Point>>();

            for (int x = 0; x < _gridRegionSize.x; x++)
            {
                for (int y = 0; y < _gridRegionSize.y; y++)
                {
                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    Vector2Int nearestPoint = new Vector2Int();  // Initialize the closest point.

                    for (int cx = 0; cx < _gridContinentSize.x; cx++)
                    {
                        for (int cy = 0; cy < _gridContinentSize.y; cy++)
                        {
                            float distance = Vector2Int.Distance(points[x][y].imagePosition, continentPoints[cx][cy].imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = continentPoints[cx][cy].gridCoord;  // Update the nearest point.
                            }
                        }
                    }

                    var newPoint = new Point(points[x][y], continentPoints[nearestPoint.x][nearestPoint.y].color);

                    continentRegionPoints.AddPoint(newPoint);
                }
            }

            drawTexture(_imgSize, _gridContinentSize, continentCellSize, ref continentPoints, null, _continentsPointsPreviewImage);

            drawTexture(_imgSize, _gridRegionSize, cellSize, ref continentRegionPoints, _continentsImage);
        }

        private Dictionary<int, Dictionary<int, Point>> getSeamlessPoints(Vector2Int cellSize, Vector2Int gridSize, bool blackAndWhiteColor = true)
        {
            var points = generatePoints(gridSize, cellSize, blackAndWhiteColor);

            // --- make the voronoi texture seamless

            var edgesPoints = points.GetEdgesPoints();
            modifySideEdge(cellSize, VoronoiEdge.Right, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.Top, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.InnerRight, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.InnerTop, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.MiddleRight, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.MiddleTop, ref edgesPoints, ref points);

            _plateTectonicsDebug?.showDebugPoints(edgesPoints);

            return points;
        }

        private void modifySideEdge(Vector2Int cellSize, VoronoiEdge edge, ref Dictionary<VoronoiEdge, List<Point>> toModifyEdge, ref Dictionary<int, Dictionary<int, Point>> points)
        {
            var modified = new List<Vector2Int>();
            foreach (var sidePoint in toModifyEdge[edge])
            {
                var oppositeCoord = getVoronoiEdgeOppositeCoord(edge, sidePoint.gridCoord);
                var oppositePoint = points[oppositeCoord.x][oppositeCoord.y];
                var imagePos = getVoronoiEdgeNewImagePos(edge, oppositePoint, sidePoint, cellSize);

                var newRightPoint = new Point(
                    sidePoint.index,
                    sidePoint.gridCoord,
                    imagePos,
                    cellSize,
                    oppositePoint.color,
                    sidePoint.edge
                );

                //showDebugMovedPoint(edge, newRightPoint, _texturePlayDebugSettings.InnerEdgePoint);

                points[sidePoint.gridCoord.x][sidePoint.gridCoord.y] = newRightPoint;
                modified.Add(newRightPoint.gridCoord);
            }
            int i = 0;
            foreach (var mod in modified)
            {
                toModifyEdge[edge][i] = points[mod.x][mod.y];
                i++;
            }
        }

        private Vector2Int getVoronoiEdgeOppositeCoord(VoronoiEdge edge, Vector2Int coord)
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

        private Vector2Int getVoronoiEdgeNewImagePos(VoronoiEdge edge, Point oppositePoint, Point sidePoint, Vector2Int cellSize)
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

        private void drawTexture(Vector2Int imgSize, Vector2Int gridSize, Vector2Int cellSize, ref Dictionary<int, Dictionary<int, Point>> points, RawImage rawImage = null, RawImage pointsPreviewImage = null)
        {
            if (pointsPreviewImage != null)
            {
                var previewTexture = new Texture2D(imgSize.x, imgSize.y);
                previewTexture.filterMode = FilterMode.Point;
                generatePreview(imgSize, gridSize, ref points, ref previewTexture);
                pointsPreviewImage.texture = previewTexture;
            }

            // --- draw texture voronoi

            if (rawImage != null)
            {
                var texture = new Texture2D(imgSize.x, imgSize.y);
                texture.filterMode = FilterMode.Point;
                generateDiagram(imgSize, gridSize, cellSize, ref points, ref texture);
                rawImage.texture = texture;
            }
        }

        private void previewPoints(Vector2Int gridSize, Dictionary<int, Dictionary<int, Point>> pointsPositions, ref Texture2D texture)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    texture.SetPixel(pointsPositions[x][y].imagePosition.x, pointsPositions[x][y].imagePosition.y, Color.black);
                }
            }
            texture.Apply();
        }

        private void generatePreview(Vector2Int imgSize, Vector2Int gridSize, ref Dictionary<int, Dictionary<int, Point>> pointsPositions, ref Texture2D texture)
        {
            for (int x = 0; x < imgSize.x; x++)
            {
                for (int y = 0; y < imgSize.y; y++)
                {
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                }
            }
            previewPoints(gridSize, pointsPositions, ref texture);
        }

        private void generateDiagram(Vector2Int imgSize, Vector2Int gridSize, Vector2Int cellSize, ref Dictionary<int, Dictionary<int, Point>> points, ref Texture2D texture)
        {
            for (int x = 0; x < imgSize.x; x++)  // Iterate over the width of the image.
            {
                for (int y = 0; y < imgSize.y; y++)  // Iterate over the height of the image.
                {
                    int gridX = x / cellSize.x;  // Determine the grid cell's Y index that contains the current pixel.
                    int gridY = y / cellSize.y;  // Determine the grid cell's X index that contains the current pixel.

                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    Vector2Int nearestPoint = new Vector2Int();  // Initialize the closest point.

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

                    var shouldBeHidden = points[nearestPoint.x][nearestPoint.y].edge == VoronoiEdge.Left
                        || points[nearestPoint.x][nearestPoint.y].edge == VoronoiEdge.Top
                        || points[nearestPoint.x][nearestPoint.y].edge == VoronoiEdge.Bottom
                        || points[nearestPoint.x][nearestPoint.y].edge == VoronoiEdge.Right;
                    var color = shouldBeHidden ? Color.black : points[nearestPoint.x][nearestPoint.y].color;
                    texture.SetPixel(x, y, color);  // Set the color of the current pixel to the color of the closest point.
                }
            }
            texture.Apply();
        }

        private Dictionary<int, Dictionary<int, Point>> generatePoints(Vector2Int gridSize, Vector2Int cellSize, bool blackAndWhiteColor = true)
        {
            var pointsCount = gridSize.x * gridSize.y;
            var lastRowCount = pointsCount - gridSize.y;
            var points = new Dictionary<int, Dictionary<int, Point>>();
            var lastGridPoint = Vector2Int.zero;
            int i = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    var point = new Point(
                        i,
                        gridCoord: new Vector2Int(x, y),
                        imagePos: new Vector2Int(x * cellSize.x + Random.Range(0, cellSize.x), y * cellSize.y + Random.Range(0, cellSize.y)),
                        cellSize,
                        color: blackAndWhiteColor ? GenerateRandomBlackAndWhiteColor() : GenerateRandomColor()
                    );

                    var isLastPointTop = determineWhatEdgeOrMiddle(i, pointsCount, lastRowCount, gridSize, ref point);
                    if (isLastPointTop)
                    {
                        var previousPoint = points[lastGridPoint.x][lastGridPoint.y];
                        previousPoint.edge = VoronoiEdge.Top;
                        points[lastGridPoint.x][lastGridPoint.y] = previousPoint;
                    }

                    points.AddPoint(point);

                    // -> for next iteration
                    lastGridPoint = point.gridCoord;
                    i++;
                }
            }
            return points;
        }

        private bool determineWhatEdgeOrMiddle(int i, int pointsCount, int lastRowCount, Vector2Int gridSize, ref Point point)
        {
            if (i == 0) // first point
                point.edge = VoronoiEdge.Bottom;
            else if (i < gridSize.y) // left side
                point.edge = VoronoiEdge.Left;
            else if (i % gridSize.y == 0) // first in column
            {
                point.edge = VoronoiEdge.Bottom;
                return true;
            }
            else if (i == pointsCount - 1) // last point
                point.edge = VoronoiEdge.Top;
            else if (i > lastRowCount) // right side
                point.edge = VoronoiEdge.Right;
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
                    point.edge = VoronoiEdge.InnerTop;
                }
                else if (point.gridCoord.y == 1)
                {
                    point.edge = VoronoiEdge.InnerBottom;
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
                        point.edge = VoronoiEdge.MiddleTop;
                    }
                    else if (point.gridCoord.y == 2)
                    {
                        point.edge = VoronoiEdge.MiddleBottom;
                    }
                    else
                    {
                        point.edge = VoronoiEdge.Middle;
                    }
                }
            }

            return false;
        }

        public static Color GenerateRandomBlackAndWhiteColor()
        {
            return new ColorHSV(
                0, // Random Hue
                0, // Random Saturation
                Random.Range(0f, 1f), // Random Value
                1f // Alpha (transparency) is set to 1 (opaque)
            );
        }

        public static Color GenerateRandomColor()
        {
            return new Color(
                Random.Range(0f, 1f), // Random Red
                Random.Range(0f, 1f), // Random Green
                Random.Range(0f, 1f), // Random Blue
                1f // Alpha (transparency) is set to 1 (opaque)
            );
        }
    }
}
