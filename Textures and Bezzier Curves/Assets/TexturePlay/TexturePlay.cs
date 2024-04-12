using Game.Shared.PropertyDrawers;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Game.Shared.Enums;
using Game.Shared.Classes;
using UniRx;
using System.Linq;

namespace TexturePlay
{
    public class TexturePlay : MonoBehaviour
    {
        [Header("Settings Size(7680, 3840) Resolution(768, 384)")]
        [SerializeField]
        private Vector2Int _imgSize;
        [SerializeField]
        private Vector2Int _gridProvinceSize;
        [SerializeField]
        private Vector2Int _gridRegionSize;
        [SerializeField]
        private Vector2Int _gridContinentSize;
        [SerializeField]
        private int _provinceDivider = 40;
        [SerializeField]
        private int _regionDivider = 100;
        [SerializeField]
        private int _continentDivider = 6;
        [SerializeField]
        private int _sizeMultiplierDivider = 1;

        [Header("Province")]
        [SerializeField]
        private RawImage _provinceImage;

        [Header("Regions")]
        [SerializeField]
        private RawImage _regionsImage;

        [Header("Continent")]
        [SerializeField]
        private RawImage _continentsImage;

        [Header("Continent")]
        [SerializeField]
        private RawImage _testSquareImage;

        [Header("GradientOptions")]
        [SerializeField]
        private GradientSettingsSO _gradientSettings;

        [Header("Debug")]
        [SerializeField]
        private PlateTectonicsDebug _plateTectonicsDebug;

        private readonly Vector2Int SIZE = new Vector2Int(7680, 3840);
        private readonly int SIZE_RESOLUTION_DIVIDER = 5;  // 10

        private Subject<int> _buildProvinces__s = new Subject<int>();
        private readonly System.TimeSpan _timeSpanTimeout = System.TimeSpan.FromMilliseconds(100);
        private int _frameDelay = 30;
        private Subject<(int x, Vector2Int size, int i)> _iterateX__s = new Subject<(int x, Vector2Int size, int i)>();
        private Subject<(int y, int x, Vector2Int size, int i)> _iterateY__s = new Subject<(int y, int x, Vector2Int size, int i)>();

        private Subject<int> _buildTectonicPlates__s = new Subject<int>();

        private Dictionary<int, Dictionary<int, IPoint>> _points;

        private void Awake()
        {
            _imgSize = SIZE / SIZE_RESOLUTION_DIVIDER;
            _gridProvinceSize = (SIZE / _provinceDivider) / _sizeMultiplierDivider;
            _gridRegionSize = (_gridProvinceSize / _regionDivider) / _sizeMultiplierDivider;
            _gridContinentSize = (_gridRegionSize / _continentDivider) / _sizeMultiplierDivider;

            Random.InitState(1);

            var textureSize = Vector3.one * 1.245f;
            var rt = _provinceImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt.localScale = textureSize;

            rt = _regionsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt.localScale = textureSize;

            //
            rt = _continentsImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt.localScale = textureSize;

            rt = _testSquareImage.GetComponent<RectTransform>();
            rt.sizeDelta = _imgSize;
            rt.localScale = textureSize;

            initSubjects();
        }

        struct TectonicPlate
        {
            public int direction;
            public float height;
            public List<Vector2Int> plates;
        }

        private void Start()
        {
            //_buildProvinces__s.OnNext(0);

            var tectonicsList = new List<Vector2Int>();
            for (int x = 0; x < _gridContinentSize.x; x++)
            {
                for (int y = 0; y < _gridContinentSize.y; y++)
                {
                    tectonicsList.Add(new Vector2Int(x, y));
                }
            }

            int continentSize = 9;
            int minContinentSize = 3;
            var tectonicsLength = tectonicsList.Count - (_gridContinentSize.y * 2);
            Debug.Log("tectonicsLength: " + tectonicsLength);
            var tectonicsSizes = new List<int>();
            while (tectonicsLength > 0)
            {
                int newLength = tectonicsLength - continentSize;
                if (newLength < 0)
                {
                    continentSize = newLength;
                }

                tectonicsSizes.Add(continentSize);
                tectonicsLength = newLength;

                //var shouldWeAddItAgain = Mathf.RoundToInt(Random.Range(0, 100)) >= 50;
                //if (shouldWeAddItAgain)
                //{
                //    continentSize++;
                //}

                continentSize--;
                if (continentSize < minContinentSize)
                {
                    continentSize = minContinentSize;
                }
            }

            var tectonicsPlate = new List<TectonicPlate>();
            for (int i = 0; i < tectonicsSizes.Count; i++)
            {
                Debug.Log("tectonicsSizes: " + tectonicsSizes[i]);

                int randomIndex = Mathf.FloorToInt(Random.Range(_gridContinentSize.y + 1, tectonicsList.Count - _gridContinentSize.y + 1));
                var randomTectonic = tectonicsList[randomIndex];
                tectonicsList.RemoveAt(randomIndex);
                Debug.Log("randomTectonic: " + randomTectonic);

                var x = randomTectonic.x;
                var y = randomTectonic.y;
                var possibleNeighbors = new List<Vector2Int>() {
                    new Vector2Int(-1, 0),
                    new Vector2Int(-1, 1),
                    new Vector2Int(0, 1),
                    new Vector2Int(1, 1),
                    new Vector2Int(1, 0),
                    new Vector2Int(1, -1),
                    new Vector2Int(0, -1),
                    new Vector2Int(-1, -1),
                };
                for (int t = 0; t < tectonicsSizes[i]; t++)
                {
                    // find an available neighbor to add
                    var rIndex = Mathf.FloorToInt(Random.Range(0, possibleNeighbors.Count));
                    var randomNeighborCoord = randomTectonic + possibleNeighbors[rIndex];
                    Debug.Log("randomNeighborCoord: " + randomNeighborCoord);
                    var isAvailable = tectonicsList.Find(tct => tct == randomNeighborCoord);
                    Debug.Log("isAvailable: " + isAvailable);

                    //Debug.Log("neighborIndex: [" + x + "][" + y + "]");

                }

                var tectonicPlate = new TectonicPlate()
                {
                    height = 1,
                    direction = 1,
                    plates = new List<Vector2Int>() { tectonicsList[i] }
                };
            }





            // THIS IS HOW WE DO IT !!!
            _buildTectonicPlates__s.OnNext(0);








            //_points = buildPoints(imgSize: _imgSize, gridSize: _gridProvinceSize);

            //var texture = new Texture2D(_imgSize.x, _imgSize.y);
            //texture.filterMode = FilterMode.Point;
            //_provinceImage.texture = texture;

            //_iterateX__s.OnNext((x: 0, _gridProvinceSize, i: 0));
        }

        private void initSubjects()
        {
            _buildTectonicPlates__s
                .Do(_ =>
                {
                    var continentPoints = buildContinentPoints(imgSize: _imgSize, gridSize: _gridContinentSize);
                    var regionPoints = buildRegionPoints(imgSize: _imgSize, gridSize: _gridRegionSize);

                    var continentCellSize = new Vector2Int(_imgSize.x / _gridContinentSize.x, _imgSize.y / _gridContinentSize.y);
                    assignRegionsToContinents(continentCellSize, _gridContinentSize, regionPoints, ref continentPoints);

                    var points = buildPoints(imgSize: _imgSize, gridSize: _gridProvinceSize);
                    var regionCellSize = new Vector2Int(_imgSize.x / _gridRegionSize.x, _imgSize.y / _gridRegionSize.y);
                    assignPointsToRegions(regionCellSize, _gridRegionSize, points, ref regionPoints);

                    var regionsTexture = new Texture2D(_imgSize.x, _imgSize.y);
                    regionsTexture.filterMode = FilterMode.Point;



                    foreach (var cxKvp in continentPoints)
                    {
                        foreach (var cyKvp in cxKvp.Value)
                        {
                            var continentPoint = cyKvp.Value;
                            continentPoint.gradientSquarePixelCoords
                                = TectonicsPlateService.GetGradientSquarePixelCoords(continentPoint);
                            var bottomLeft = continentPoint.gradientSquarePixelCoords[0];
                            var topRight = continentPoint.gradientSquarePixelCoords[1];
                            var size = new Vector2Int(topRight.x - bottomLeft.x, topRight.y - bottomLeft.y);
                            var gradientTexture = _gradientSettings.GetGradientTexture(continentPoint.PlateMovementDirection);

                            var perPixel = new Vector2((float)gradientTexture.width / (float)size.x, (float)gradientTexture.height / (float)size.y);

                            foreach (var rxKvp in continentPoint.RegionPoints)
                            {
                                foreach (var ryKvp in rxKvp.Value)
                                {
                                    var regionPoint = ryKvp.Value;
                                    foreach (var pxKvp in regionPoint.Points)
                                    {
                                        foreach (var pyKvp in pxKvp.Value)
                                        {
                                            var point = pyKvp.Value;

                                            var gradientPos = point.imagePosition - bottomLeft;
                                            var pixelPos = new Vector2Int(Mathf.FloorToInt(gradientPos.x * perPixel.x), Mathf.FloorToInt(gradientPos.y * perPixel.y));
                                            ColorHSV color = gradientTexture.GetPixel(pixelPos.x, pixelPos.y);
                                            color.v = color.v * continentPoint.Elevation;

                                            foreach (var pxxKvp in point.pixels)
                                            {
                                                foreach (var pxyKvp in pxxKvp.Value)
                                                {
                                                    var pixel = pxyKvp.Value;
                                                    regionsTexture.SetPixel(pixel.coord.x, pixel.coord.y, color);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    regionsTexture.Apply();
                    _regionsImage.gameObject.SetActive(true);
                    _regionsImage.texture = regionsTexture;

                    Debug.Log("_regionsImage ->>>>>>>>>");

                    //var testTexture = TectonicsPlateService.AverageOutTexture(_regionsImage.texture as Texture2D, neighborDepth: 4);
                    //testTexture.filterMode = FilterMode.Point;
                    //_testSquareImage.gameObject.SetActive(true);
                    //_testSquareImage.texture = testTexture;

                    //testTexture = TectonicsPlateService.EqualizeGrayscale(regionsTexture); // lighten up

                    //var testTexture = TectonicsPlateService.ApplyGaussianBlur(_regionsImage.texture as Texture2D);
                    //testTexture = TectonicsPlateService.ApplyGaussianBlur(testTexture);

                    //testTexture.filterMode = FilterMode.Point;
                    //_testSquareImage.gameObject.SetActive(true);
                    //_testSquareImage.texture = testTexture;
                })
                .Subscribe();




            _buildProvinces__s
                //.Timeout(_timeSpanTimeout)
                .Do(_ =>
                {
                    _points = buildPoints(imgSize: _imgSize, gridSize: _gridProvinceSize);
                })
                //.Timeout(_timeSpanTimeout)
                .Do(_ =>
                {
                    _provinceImage.gameObject.SetActive(false);
                    //drawTexture(_imgSize, _gridProvinceSize, ref _points, _provinceImage);
                })
                //.Timeout(_timeSpanTimeout)
                .Select(_ =>
                {
                    var regionPoints = buildPoints(imgSize: _imgSize, gridSize: _gridRegionSize, blackAndWhiteColor: false);
                    colorSmallerPointsWithBiggerPoints(bgSizePoints: regionPoints, gridBgSize: _gridRegionSize, gridSmSize: _gridProvinceSize, smSizePoints: ref _points);
                    return regionPoints;
                })
                //.Timeout(_timeSpanTimeout)
                .Select(regionPoints =>
                {
                    //_regionsImage.gameObject.SetActive(true);
                    //drawTexture(_imgSize, _gridProvinceSize, ref _points, _regionsImage);
                    return regionPoints;
                })
                //.Timeout(_timeSpanTimeout)
                .Do(regionPoints =>
                {
                    var continentPoints = buildPoints(imgSize: _imgSize, gridSize: _gridContinentSize, blackAndWhiteColor: false);
                    colorSmallerPointsWithBiggerPoints(bgSizePoints: continentPoints, gridBgSize: _gridContinentSize, gridSmSize: _gridRegionSize, smSizePoints: ref regionPoints, modifyPixels: false);
                    colorSmallerPointsWithBiggerPoints(bgSizePoints: regionPoints, gridBgSize: _gridRegionSize, gridSmSize: _gridProvinceSize, smSizePoints: ref _points);
                })
                //.Timeout(_timeSpanTimeout)
                .Do(_ =>
                {
                    _continentsImage.gameObject.SetActive(true);
                    drawTexture(_imgSize, _gridProvinceSize, ref _points, _continentsImage);
                })
                .Subscribe();

            //_iterateX__s
            //    .Timeout(System.TimeSpan.FromMilliseconds(1000))
            //    .Do((payload) =>
            //    {
            //        if (payload.x == payload.size.x) { return; }
            //        Debug.Log("[" + payload.x + "] ... ");
            //        _iterateY__s.OnNext((y: 0, payload.x, payload.size, payload.i));
            //    })
            //    .Subscribe();

            //_iterateY__s
            //    .Timeout(System.TimeSpan.FromMilliseconds(1000))
            //    .Do((payload) =>
            //    {
            //        Debug.Log("[" + payload.x + "][" + payload.y + "] (" + payload.i + ")");

            //        //var texture = _provinceImage.texture as Texture2D;
            //        //paintPixels(_points[payload.x][payload.y].pixels, ref texture, applyTexture: true);
            //        //_provinceImage.texture = texture;

            //        payload.i++;
            //        payload.y++;
            //        if (payload.y == payload.size.y)
            //        {
            //            payload.x++;
            //            _iterateX__s.OnNext((x: payload.x, payload.size, payload.i));
            //            return;
            //        }
            //        _iterateY__s.OnNext(payload);
            //    })
            //    .Subscribe();
        }

        private Dictionary<int, Dictionary<int, IPoint>> buildPoints(Vector2Int imgSize, Vector2Int gridSize, bool blackAndWhiteColor = true)
        {
            var cellSize = new Vector2Int(imgSize.x / gridSize.x, imgSize.y / gridSize.y);
            var points = generatePoints(gridSize, cellSize, blackAndWhiteColor);

            var edgesPoints = points.GetEdgesPoints();
            modifySideEdge(cellSize, VoronoiEdge.Right, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.InnerRight, ref edgesPoints, ref points);
            modifySideEdge(cellSize, VoronoiEdge.MiddleRight, ref edgesPoints, ref points);

            TectonicsPlateService.AssignPixelsAndColor(imgSize, gridSize, cellSize, ref points);

            return points;
        }

        private Dictionary<int, Dictionary<int, IContinentPoint>> buildContinentPoints(Vector2Int imgSize, Vector2Int gridSize)
        {
            var cellSize = new Vector2Int(imgSize.x / gridSize.x, imgSize.y / gridSize.y);
            var continentPoints = generateContinentPoints(gridSize, cellSize);// as Dictionary<int, Dictionary<int, IContinentPoint>>;

            var edgesPoints = continentPoints.GetEdgesPoints();
            modifyContinentSideEdge(cellSize, VoronoiEdge.Right, ref edgesPoints, ref continentPoints);
            modifyContinentSideEdge(cellSize, VoronoiEdge.InnerRight, ref edgesPoints, ref continentPoints);
            modifyContinentSideEdge(cellSize, VoronoiEdge.MiddleRight, ref edgesPoints, ref continentPoints);

            return continentPoints;
        }

        private Dictionary<int, Dictionary<int, IRegionPoint>> buildRegionPoints(Vector2Int imgSize, Vector2Int gridSize)
        {
            var cellSize = new Vector2Int(imgSize.x / gridSize.x, imgSize.y / gridSize.y);
            var regionPoints = generateRegionPoints(gridSize, cellSize);

            var edgesPoints = regionPoints.GetEdgesPoints();
            modifyRegionSideEdge(cellSize, VoronoiEdge.Right, ref edgesPoints, ref regionPoints);
            modifyRegionSideEdge(cellSize, VoronoiEdge.InnerRight, ref edgesPoints, ref regionPoints);
            modifyRegionSideEdge(cellSize, VoronoiEdge.MiddleRight, ref edgesPoints, ref regionPoints);

            return regionPoints;
        }

        private void assignRegionsToContinents(Vector2Int continentCellSize, Vector2Int gridContinentSize, Dictionary<int, Dictionary<int, IRegionPoint>> regionPoints, ref Dictionary<int, Dictionary<int, IContinentPoint>> continentPoints)
        {
            foreach (var xKvp in regionPoints)
            {
                foreach (var yKvp in xKvp.Value)
                {
                    var point = yKvp.Value;

                    var pX = Mathf.FloorToInt(point.gridPosition.x / continentCellSize.x);
                    var pY = Mathf.FloorToInt(point.gridPosition.y / continentCellSize.y);

                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    var nearestPoint = new Vector2Int();  // Initialize the closest point.

                    // check left and right of the parent too ses who is nearest to the current point
                    for (int bx = pX - 1; bx < pX + 1; bx++)
                    {
                        for (int by = pY - 1; by < pY + 1; by++)
                        {
                            if (bx < 0 || by < 0 || bx >= gridContinentSize.x || by >= gridContinentSize.y) continue;  // Skip if the neighbor cell is out of the grid bounds.

                            float distance = Vector2Int.Distance(point.imagePosition, continentPoints[bx][by].imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = continentPoints[bx][by].gridCoord;  // Update the nearest point.
                            }
                        }
                    }
                    continentPoints[nearestPoint.x][nearestPoint.y].RegionPoints.AddPoint(point);
                }
            }
        }

        private void assignPointsToRegions(Vector2Int regionCellSize, Vector2Int gridRegionSize, Dictionary<int, Dictionary<int, IPoint>> points, ref Dictionary<int, Dictionary<int, IRegionPoint>> regionPoints)
        {
            foreach (var xKvp in points)
            {
                foreach (var yKvp in xKvp.Value)
                {
                    var point = yKvp.Value;

                    var pX = Mathf.FloorToInt(point.gridPosition.x / regionCellSize.x);
                    var pY = Mathf.FloorToInt(point.gridPosition.y / regionCellSize.y);

                    float nearestDistance = Mathf.Infinity;  // Initialize the minimum distance with infinity.
                    var nearestPoint = new Vector2Int();  // Initialize the closest point.

                    // check left and right of the parent too ses who is nearest to the current point
                    for (int bx = pX - 1; bx < pX + 1; bx++)
                    {
                        for (int by = pY - 1; by < pY + 1; by++)
                        {
                            if (bx < 0 || by < 0 || bx >= gridRegionSize.x || by >= gridRegionSize.y) continue;  // Skip if the neighbor cell is out of the grid bounds.

                            float distance = Vector2Int.Distance(point.imagePosition, regionPoints[bx][by].imagePosition);  // Calculate the distance between the current pixel and the point in the neighboring cell.
                            if (distance < nearestDistance)  // If the calculated distance is less than the current minimum distance.
                            {
                                nearestDistance = distance;  // Update the minimum distance.
                                nearestPoint = regionPoints[bx][by].gridCoord;  // Update the nearest point.
                            }
                        }
                    }
                    regionPoints[nearestPoint.x][nearestPoint.y].Points.AddPoint(point);
                }
            }
        }

        private void colorSmallerPointsWithBiggerPoints(
        Dictionary<int, Dictionary<int, IPoint>> bgSizePoints,
        Vector2Int gridBgSize,
        Vector2Int gridSmSize,
        ref Dictionary<int, Dictionary<int, IPoint>> smSizePoints,
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

                    if (modifyPixels == false) { continue; }

                    IPoint smallSizedPoint = smSizePoints[x][y];
                    TectonicsPlateService.ModifyAllPixels(ref smallSizedPoint, bgSizePoints[nearestPoint.x][nearestPoint.y].color);
                }
            }
        }

        private void modifySideEdge(Vector2Int cellSize, VoronoiEdge edge, ref Dictionary<VoronoiEdge, List<IPoint>> toModifyEdge, ref Dictionary<int, Dictionary<int, IPoint>> points)
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

        private void modifyRegionSideEdge(Vector2Int cellSize, VoronoiEdge edge, ref Dictionary<VoronoiEdge, List<IRegionPoint>> toModifyEdge, ref Dictionary<int, Dictionary<int, IRegionPoint>> points)
        {
            var modified = new List<Vector2Int>();
            foreach (IRegionPoint sidePoint in toModifyEdge[edge])
            {
                var oppositeCoord = TectonicsPlateService.GetVoronoiEdgeOppositeCoord(edge, sidePoint.gridCoord);
                IRegionPoint oppositePoint = points[oppositeCoord.x][oppositeCoord.y];
                var imagePos = TectonicsPlateService.GetVoronoiEdgeNewImagePos(edge, oppositePoint, sidePoint, cellSize);

                sidePoint.updatePoint(imagePos, cellSize);

                modified.Add(sidePoint.gridCoord);
            }
            int i = 0;
            foreach (var mod in modified)
            {
                toModifyEdge[edge][i] = points[mod.x][mod.y];
                i++;
            }
        }

        private void modifyContinentSideEdge(Vector2Int cellSize, VoronoiEdge edge, ref Dictionary<VoronoiEdge, List<IContinentPoint>> toModifyEdge, ref Dictionary<int, Dictionary<int, IContinentPoint>> points)
        {
            var modified = new List<Vector2Int>();
            foreach (IContinentPoint sidePoint in toModifyEdge[edge])
            {
                var oppositeCoord = TectonicsPlateService.GetVoronoiEdgeOppositeCoord(edge, sidePoint.gridCoord);
                IContinentPoint oppositePoint = points[oppositeCoord.x][oppositeCoord.y];
                var imagePos = TectonicsPlateService.GetVoronoiEdgeNewImagePos(edge, oppositePoint, sidePoint, cellSize);

                sidePoint.updatePoint(imagePos, cellSize, oppositePoint.PlateMovementDirection);

                modified.Add(sidePoint.gridCoord);
            }
            int i = 0;
            foreach (var mod in modified)
            {
                toModifyEdge[edge][i] = points[mod.x][mod.y];
                i++;
            }
        }

        private void drawTexture(Vector2Int imgSize, Vector2Int gridSize, ref Dictionary<int, Dictionary<int, IPoint>> points, RawImage rawImage)
        {
            var texture = new Texture2D(imgSize.x, imgSize.y);
            texture.filterMode = FilterMode.Point;
            paintGridPixels(gridSize, ref points, ref texture);
            rawImage.texture = texture;
        }

        private void paintGridPixels(Vector2Int gridSize, ref Dictionary<int, Dictionary<int, IPoint>> points, ref Texture2D texture)
        {
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    paintPixels(points[x][y].pixels, ref texture);
                }
            }
            texture.Apply();
        }

        private void paintPixels(Dictionary<int, Dictionary<int, Pixel>> pixels, ref Texture2D texture, bool applyTexture = false)
        {
            foreach (var pixelCols in pixels)
            {
                foreach (var pixel in pixelCols.Value)
                {
                    texture.SetPixel(pixel.Value.coord.x, pixel.Value.coord.y, pixel.Value.color);
                }
            }

            if (applyTexture)
            {
                texture.Apply();
            }
        }

        private Dictionary<int, Dictionary<int, IContinentPoint>> generateContinentPoints(Vector2Int gridSize, Vector2Int cellSize)
        {
            var points = new Dictionary<int, Dictionary<int, IContinentPoint>>();
            int i = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    IPointBase point = new ContinentPoint(
                        i,
                        gridCoord: new Vector2Int(x, y),
                        imagePos: new Vector2Int(x * cellSize.x + Random.Range(0, cellSize.x), y * cellSize.y + Random.Range(0, cellSize.y)),
                        cellSize,
                        plateMovementDirection: TectonicsPlateService.GetRandomDegrees(),
                        elevation: TectonicsPlateService.GetRandomTectonicElevation()
                    );
                    TectonicsPlateService.SetPointOnEdge(gridSize, ref point);
                    points.AddPoint(point as IContinentPoint);
                    i++;
                }
            }
            return points;
        }

        private Dictionary<int, Dictionary<int, IPoint>> generatePoints(Vector2Int gridSize, Vector2Int cellSize, bool blackAndWhiteColor = true)
        {
            var points = new Dictionary<int, Dictionary<int, IPoint>>();
            int i = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    IPointBase point = new Point(
                        i,
                        gridCoord: new Vector2Int(x, y),
                        imagePos: new Vector2Int(x * cellSize.x + Random.Range(0, cellSize.x), y * cellSize.y + Random.Range(0, cellSize.y)),
                        cellSize,
                        color: blackAndWhiteColor ? GenerateRandomBlackAndWhiteColor() : GenerateRandomColor()
                    );
                    TectonicsPlateService.SetPointOnEdge(gridSize, ref point);
                    points.AddPoint(point as IPoint);
                    i++;
                }
            }
            return points;
        }

        private Dictionary<int, Dictionary<int, IRegionPoint>> generateRegionPoints(Vector2Int gridSize, Vector2Int cellSize)
        {
            var points = new Dictionary<int, Dictionary<int, IRegionPoint>>();
            int i = 0;
            for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    IPointBase point = new RegionPoint(
                        i,
                        gridCoord: new Vector2Int(x, y),
                        imagePos: new Vector2Int(x * cellSize.x + Random.Range(0, cellSize.x), y * cellSize.y + Random.Range(0, cellSize.y)),
                        cellSize
                    );
                    TectonicsPlateService.SetPointOnEdge(gridSize, ref point);
                    points.AddPoint(point as IRegionPoint);
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
