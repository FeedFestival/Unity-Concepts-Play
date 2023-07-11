using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoundPathCreator : MonoBehaviour
{
    [SerializeField]
    private BezierCurve _roundCurve;
    [SerializeField]
    private LineRenderer _lineRenderer;
    [SerializeField]
    private float _lineResolution = 100;
    [SerializeField]
    private float _pointSpacing = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        var points = GET_POINTS();
        CreatePath(points);

        var bezierPoints = _roundCurve.GetAnchorPoints();
        drawLinePathWithEvenPoints(bezierPoints.ToList());
    }

    public void CreatePath(List<Vector3> cornersPosition)
    {
        // clear points
        _roundCurve.Clear();
        foreach (Transform bzPointGo in _roundCurve.transform)
        {
            Destroy(bzPointGo.gameObject);
        }

        // create path
        for (int i = 0; i < cornersPosition.Count; i++)
        {
            var prevI = (i == 0) ? cornersPosition.Count - 1 : i - 1;
            var nextI = (i == cornersPosition.Count - 1) ? 0 : i + 1;
            createBezierPoint(cornersPosition[prevI], cornersPosition[i], cornersPosition[nextI]);
            Debug.Log("-----------------" + i + "-----------------");
        }
    }

    private Vector3 createBezierPoint(Vector3 prevP, Vector3 cornerPos, Vector3 nextPos)
    {
        var reduceBy = 2.5f;

        var prevDir = (cornerPos - prevP).normalized;
        var distance = Vector3.Distance(cornerPos, prevP);
        var reducedDistance = distance / reduceBy;
        var reducedDir = prevDir * reducedDistance;
        Debug.Log("prevDir: " + prevDir);
        Debug.Log("distance: " + distance);
        Debug.Log("reducedDistance: " + reducedDistance);
        Debug.Log("reducedDir: " + reducedDir);

        var nextDir = (cornerPos - nextPos).normalized;
        var nDistance = Vector3.Distance(cornerPos, nextPos);
        var nReducedDistance = nDistance / reduceBy;
        var nReducedDir = nextDir * nReducedDistance;
        Debug.Log("nextDir: " + nextDir);
        Debug.Log("nDistance: " + nDistance);
        Debug.Log("nReducedDistance: " + nReducedDistance);
        Debug.Log("nReducedDir: " + nReducedDir);
        var oppositeDir = -nReducedDir;

        var dir = MiddlePoint(reducedDir, oppositeDir);

        var bzPoint = _roundCurve.AddPointAt(cornerPos);
        bzPoint.handle2 = dir;
        return cornerPos;
    }

    public static Vector3 MiddlePoint(Vector3 vectorA, Vector3 vectorB)
    {
        return (vectorA + vectorB) / 2.0f;
    }

    private void drawLinePathWithEvenPoints(List<BezierPoint> bezierPoints)
    {
        var points = getPoints(bezierPoints);
        //var evenPoints = getEvenlyDistributedPoints(points);
        drawLine(points);
    }

    private Vector3[] getPoints(List<BezierPoint> bezierPoints)
    {
        bezierPoints.Add(bezierPoints[0]);
        var count = (bezierPoints.Count - 1) * (int)_lineResolution;
        //var count = (bezierPoints.Length) * (int)_lineResolution;
        var positions = new Vector3[count];
        var pI = 0;

        for (int i = 0; i < bezierPoints.Count; i++)
        {
            int nextI = i + 1;
            if (bezierPoints.Count == nextI) { break; }

            for (int p = 0; p < (int)_lineResolution; p++)
            {
                var t = p / _lineResolution;
                var point = BezierCurve.GetPoint(bezierPoints[i], bezierPoints[nextI], t);
                positions[pI] = point;
                pI++;
            }
        }

        return positions;
    }

    private void drawLine(Vector3[] positions)
    {
        _lineRenderer.positionCount = positions.Length;
        _lineRenderer.SetPositions(positions);
    }

    private Vector3[] getEvenlyDistributedPoints(Vector3[] points)
    {
        var evenlyDistributedPoints = new List<Vector3>();

        evenlyDistributedPoints.Add(points[0]);

        for (int i = 0; i < points.Length - 1; i++)
        {
            for (int j = i + 1; j < points.Length; j++)
            {
                var a = (Vector2)points[i];
                var b = (Vector2)points[j];
                float d = Vector2.Distance(a, b);
                if (d < _pointSpacing)
                {
                    continue;
                }
                else
                {
                    evenlyDistributedPoints.Add(points[j]);
                    i = j;
                }
            }
        }

        return evenlyDistributedPoints.ToArray();
    }

    //public static void CreateSVG()
    //{
    //    // Create a new SVG document and set its viewBox and preserveAspectRatio settings.
    //    var sceneInfo = new SceneInfo();
    //    sceneInfo.Scene = new Scene();
    //    sceneInfo.Scene.Root = new SceneNode();
    //    sceneInfo.PreserveViewport = true;

    //    // Create a new shape containing the path.
    //    var shape = new Shape()
    //    {
    //        Contours = new BezierContour[] { new BezierContour() { Segments = PathSegmentBuilder.BuildPath(path.ToArray(), true) } },
    //        Fill = new SolidFill() { Color = Color.red },
    //        PathProps = new PathProperties()
    //        {
    //            Stroke = new Stroke() { Color = Color.black, HalfThickness = 0.05f },
    //            Head = LineEnd.Round,
    //            Tail = LineEnd.Round,
    //            Corner = LineCorner.Round,
    //            Join = LineJoin.Round,
    //        }
    //    };

    //    sceneInfo.Scene.Root.Shapes = new List<Shape> { shape };

    //    // Create an SVG document from the scene.
    //    var svg = SVGExporter.Export(sceneInfo.Scene);

    //    // Output the SVG data to a file (you can use any path you want).
    //    System.IO.File.WriteAllText(Application.dataPath + "/Polygon.svg", svg);
    //}

    public static List<Vector3> GET_POINTS()
    {
        return new List<Vector3>()
        {
            new Vector3(6.602531f, 0f,9f),
            new Vector3(7.468559f, 0f,8.5f),
            new Vector3(7.468559f, 0f,7.5f),
            new Vector3(8.334587f, 0f,7f),
            new Vector3(8.334587f, 0f,6f),
            new Vector3(9.200615f, 0f,5.5f),
            new Vector3(10.06664f, 0f,6f),
            new Vector3(10.93266f, 0f,5.5f),
            new Vector3(11.79868f, 0f,6f),
            new Vector3(12.66471f, 0f,5.5f),
            new Vector3(13.53074f, 0f,6f),
            new Vector3(13.53074f, 0f,7f),
            new Vector3(14.39677f, 0f,7.5f),
            new Vector3(14.39677f, 0f,8.5f),
            new Vector3(15.26279f, 0f,9f),
            new Vector3(15.26279f, 0f,10f),
            new Vector3(14.39677f, 0f,10.5f),
            new Vector3(14.39677f, 0f,11.5f),
            new Vector3(13.53074f, 0f,12f),
            new Vector3(13.53074f, 0f,13f),
            new Vector3(12.66471f, 0f,13.5f),
            new Vector3(11.79868f, 0f,13f),
            new Vector3(10.93266f, 0f,13.5f),
            new Vector3(10.06664f, 0f,13f),
            new Vector3(9.200615f, 0f,13.5f),
            new Vector3(8.334587f, 0f,13f),
            new Vector3(8.334587f, 0f,12f),
            new Vector3(7.468559f, 0f,11.5f),
            new Vector3(7.468559f, 0f,10.5f),
            new Vector3(6.602531f, 0f,10f),
        };
    }
}
