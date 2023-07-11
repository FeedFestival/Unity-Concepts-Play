using System.Collections.Generic;
using UnityEngine;

public class CircleCreator : MonoBehaviour
{
    [Header("Refs")]
    public BezierCurve BezierCurve;
    public LineRenderer LineRenderer;
    [SerializeField]
    private float _circleSize = 1f;
    [SerializeField]
    private int _fragments = 100;
    [SerializeField]
    private float _pointSpacing = 0.1f;

    [SerializeField, Header("Settings")]
    private PathCreatorSObject _pathCreatorSettings;
    [SerializeField]
    private bool _isClosedLoop;
    [SerializeField, Space(50)]
    private bool _debug;
    private BezierPathDebug _bezierPathDebug;

    internal Vector3[] EvenPoints;


    private const float HANDLE_LENGTH_PROPORTION = 0.55228f;

    private void Start()
    {
        if (_debug)
        {
            _bezierPathDebug = gameObject.AddComponent<BezierPathDebug>();
        }

        CircleCreator.ClearBezierCurve(BezierCurve);
        BezierCurve.close = _isClosedLoop;

        var handleLength = HANDLE_LENGTH_PROPORTION * _circleSize;
        var initialPointAndHandle = new Vector3[2]
        {
            new Vector3(0, _circleSize, 0),
            new Vector3(-handleLength, 0, 0),
        };
        var points = new List<Vector3[]>()
        {
            initialPointAndHandle,
            new Vector3[2]
            {
                new Vector3(_circleSize, 0, 0),
                new Vector3(0, handleLength, 0),
            },
            new Vector3[2]
            {
                new Vector3(0, -_circleSize, 0),
                new Vector3(handleLength, 0, 0),
            },
            new Vector3[2]
            {
                new Vector3(-_circleSize, 0, 0),
                new Vector3(0, -handleLength, 0),
            },
            initialPointAndHandle
        };

        foreach (var p in points)
        {
            var bzPoint = BezierCurve.AddPointAt(p[0]);
            bzPoint.handle1 = p[1];
        }

        var bezierPoints = BezierCurve.GetAnchorPoints();

        drawPath(bezierPoints);
    }

    private void drawPath(BezierPoint[] bezierPoints)
    {
        var points = GetPoints(bezierPoints, _fragments);
        DrawLine(ref LineRenderer, points);
        EvenPoints = GetEvenlyDistributedPoints(points, _pointSpacing, _isClosedLoop);

        _bezierPathDebug?.ShowPoints(_pathCreatorSettings, BezierPathDebug.DebugPoint.OriginalPoint, points);
        _bezierPathDebug?.ShowPoints(_pathCreatorSettings, BezierPathDebug.DebugPoint.CalculatedPoint, EvenPoints);
    }

    public static Vector3[] GetPoints(BezierPoint[] bezierPoints, int fragments)
    {
        var count = (bezierPoints.Length - 1) * fragments;
        var positions = new Vector3[count];
        var pI = 0;

        for (int i = 0; i < bezierPoints.Length; i++)
        {
            int nextI = i + 1;
            if (bezierPoints.Length == nextI) { break; }

            for (int p = 0; p < fragments; p++)
            {
                var t = p / (float)fragments;
                var point = BezierCurve.GetPoint(bezierPoints[i], bezierPoints[nextI], t);
                positions[pI] = point;
                pI++;
            }
        }

        return positions;
    }

    public static void DrawLine(ref LineRenderer lineRenderer, Vector3[] positions)
    {
        lineRenderer.positionCount = positions.Length;
        lineRenderer.SetPositions(positions);
    }

    public static Vector3[] GetEvenlyDistributedPoints(Vector3[] points, float pointSpacing, bool _isClosedLoop = false)
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
                if (d < pointSpacing)
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

        if (_isClosedLoop)
        {
            evenlyDistributedPoints.Add(points[0]);
        }

        return evenlyDistributedPoints.ToArray();
    }

    public static void ClearBezierCurve(BezierCurve bezierCurve)
    {
        bezierCurve.Clear();
        foreach (Transform t in bezierCurve.transform)
        {
            Destroy(t.gameObject);
        }
    }
}
