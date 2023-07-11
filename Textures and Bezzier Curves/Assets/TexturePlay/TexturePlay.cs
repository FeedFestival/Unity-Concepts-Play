using Game.Shared.PropertyDrawers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Shared.Enums;
using Game.Shared.Classes;

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
        private Vector2Int _gridProvinceSize;
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridRegionSize;
        [SerializeField]
        [ReadOnly]
        private Vector2Int _gridContinentSize;
        [SerializeField]
        private int _provinceDivider = 40;
        [SerializeField]
        private int _regionDivider = 100;
        [SerializeField]
        private int _continentDivider = 6;
        [SerializeField]
        private int _sizeMultiplierDivider = 1;
        [SerializeField]
        private TextAsset _pointsTextAsset;
        [SerializeField]
        private TextAsset _continentPointsTextAsset;

        [Header("Regions")]
        [SerializeField]
        private RawImage _provinceImage;

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
            _gridProvinceSize = (SIZE / _provinceDivider) / _sizeMultiplierDivider;
            _gridRegionSize = (_gridProvinceSize / _regionDivider) / _sizeMultiplierDivider;
            _gridContinentSize = (_gridRegionSize / _continentDivider) / _sizeMultiplierDivider;

            Random.InitState(1);

            var rt = _provinceImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;

            rt = _regionsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;

            //
            rt = _continentsPointsPreviewImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt = _continentsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;

            buildImage();
        }

        private void buildImage()
        {
            var points = buildPoints(imgSize: _imgSize, gridSize: _gridProvinceSize);
            drawTexture(_imgSize, _gridProvinceSize, ref points, _provinceImage);

            var regionPoints = buildPoints(imgSize: _imgSize, gridSize: _gridRegionSize, blackAndWhiteColor: false);
            colorSmallerPointsWithBiggerPoints(bgSizePoints: regionPoints, gridBgSize: _gridRegionSize, gridSmSize: _gridProvinceSize, smSizePoints: ref points);
            drawTexture(_imgSize, _gridProvinceSize, ref points, _regionsImage);

            var continentPoints = buildPoints(imgSize: _imgSize, gridSize: _gridContinentSize, blackAndWhiteColor: false);
            colorSmallerPointsWithBiggerPoints(bgSizePoints: continentPoints, gridBgSize: _gridContinentSize, gridSmSize: _gridRegionSize, smSizePoints: ref regionPoints, modifyPixels: false);
            colorSmallerPointsWithBiggerPoints(bgSizePoints: regionPoints, gridBgSize: _gridRegionSize, gridSmSize: _gridProvinceSize, smSizePoints: ref points);
            drawTexture(_imgSize, _gridProvinceSize, ref points, _continentsImage);
        }

        private Dictionary<int, Dictionary<int, Point>> buildPoints(Vector2Int imgSize, Vector2Int gridSize, bool blackAndWhiteColor = true)
        {
            var cellSize = new Vector2Int(imgSize.x / gridSize.x, imgSize.y / gridSize.y);
            var points = getSeamlessPoints(imgSize, cellSize, gridSize, blackAndWhiteColor);
            return points;
        }

        private Dictionary<int, Dictionary<int, Point>> getSeamlessPoints(Vector2Int imgSize, Vector2Int cellSize, Vector2Int gridSize, bool blackAndWhiteColor = true)
        {
            var points = generatePoints(gridSize, cellSize, blackAndWhiteColor);

            var edgesPoints = points.GetEdgesPoints();
            modifySideEdge(cellSize, VoronoiEdge.Right, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.InnerRight, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.MiddleRight, ref edgesPoints, ref points);

            TectonicsPlateService.AssignPixelsAndColor(imgSize, gridSize, cellSize, ref points);

            _plateTectonicsDebug?.showDebugPoints(edgesPoints);

            return points;
        }

        private void colorSmallerPointsWithBiggerPoints(
            Dictionary<int, Dictionary<int, Point>> bgSizePoints,
            Vector2Int gridBgSize,
            Vector2Int gridSmSize,
            ref Dictionary<int, Dictionary<int, Point>> smSizePoints,
            bool modifyPixels = true
        )
        {
            for (int x = 0; x < gridSmSize.x; x++)
            {
                for (int y = 0; y < gridSmSize.y; y++)
                {
                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    var nearestPoint = new Vector2Int();  // Initialize the closest point.

                    for (int bx = 0; bx < gridBgSize.x; bx++)
                    {
                        for (int by = 0; by < gridBgSize.y; by++)
                        {
                            float distance = Vector2Int.Distance(smSizePoints[x][y].imagePosition, bgSizePoints[bx][by].imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = bgSizePoints[bx][by].gridCoord;  // Update the nearest point.
                            }
                        }
                    }

                    smSizePoints[x][y].color = bgSizePoints[nearestPoint.x][nearestPoint.y].color;

                    if (smSizePoints[x][y].hasDeadPixels)
                    {
                        TectonicsPlateService.ModifyAllPixels(ref smSizePoints[x][y].pixels, Color.black);
                    }

                    if (modifyPixels == false) { continue; }

                    TectonicsPlateService.ModifyAllPixels(ref smSizePoints[x][y].pixels, bgSizePoints[nearestPoint.x][nearestPoint.y].color);
                }
            }
        }

        private void modifySideEdge(Vector2Int cellSize, VoronoiEdge edge, ref Dictionary<VoronoiEdge, List<Point>> toModifyEdge, ref Dictionary<int, Dictionary<int, Point>> points)
        {
            var modified = new List<Vector2Int>();
            foreach (var sidePoint in toModifyEdge[edge])
            {
                var oppositeCoord = TectonicsPlateService.GetVoronoiEdgeOppositeCoord(edge, sidePoint.gridCoord);
                var oppositePoint = points[oppositeCoord.x][oppositeCoord.y];
                var imagePos = TectonicsPlateService.GetVoronoiEdgeNewImagePos(edge, oppositePoint, sidePoint, cellSize);

                sidePoint.updatePoint(imagePos, cellSize, oppositePoint.color);

                modified.Add(sidePoint.gridCoord);
            }
            int i = 0;
            foreach (var mod in modified)
            {
                toModifyEdge[edge][i] = points[mod.x][mod.y];
                i++;
            }
        }

        private void drawTexture(Vector2Int imgSize, Vector2Int gridSize, ref Dictionary<int, Dictionary<int, Point>> points, RawImage rawImage)
        {
            var texture = new Texture2D(imgSize.x, imgSize.y);
            texture.filterMode = FilterMode.Point;
            paintPixels(gridSize, ref points, ref texture);
            rawImage.texture = texture;
        }

        private void paintPixels(Vector2Int gridSize, ref Dictionary<int, Dictionary<int, Point>> points, ref Texture2D texture)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    foreach (var pixelCols in points[x][y].pixels)
                    {
                        foreach (var pixel in pixelCols.Value)
                        {
                            texture.SetPixel(pixel.Value.coord.x, pixel.Value.coord.y, pixel.Value.color);
                        }
                    }
                }
            }
            texture.Apply();
        }

        private Dictionary<int, Dictionary<int, Point>> generatePoints(Vector2Int gridSize, Vector2Int cellSize, bool blackAndWhiteColor = true)
        {
            var points = new Dictionary<int, Dictionary<int, Point>>();
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
                    TectonicsPlateService.SetPointOnEdge(gridSize, ref point);
                    points.AddPoint(point);
                    i++;
                }
            }
            return points;
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
