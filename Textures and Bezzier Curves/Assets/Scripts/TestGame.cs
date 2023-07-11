using DentedPixel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGame : MonoBehaviour
{
    [Header("Refs")]
    public BezierCurve BezierCurve;
    public LineRenderer LineRenderer;

    public GameObject _walker;

    [Header("Debug")]
    [SerializeField]
    private Transform _debug;
    [SerializeField]
    private Transform _originalParentT;
    [SerializeField]
    private Transform _calculatedParentT;
    [SerializeField]
    private GameObject _bzPointPrefab;
    [SerializeField]
    private GameObject _originalPoint;
    [SerializeField]
    private GameObject _calculatedPoint;

    enum DebugPoint { BzPoint, OriginalPoint, CalculatedPoint }

    [Header("Run Settings")]
    [SerializeField]
    private float _time;
    [SerializeField]
    private float _tFragments = 100;
    [SerializeField]
    private float _pointSpacing = 0.1f;
    [SerializeField]
    private float _speed = 0.1f;
    [SerializeField]
    private float _pointsCount = 20;

    private BezierPoint[] _bezierPoints;
    private BezierPoint[] _currentBezierPointPath;
    private int _index;
    private Vector3[] _evenPoints;
    private List<float> _bezierDistances;

    private int? _moveTwid;
    private bool _isRunningForward;
    private bool _isRunningBackward;

    private float _startTime;

    private const float MAX_Y = 5f;

    private void Start()
    {
        _bezierDistances = new List<float>();

        createBezier();

        var distSum = 0f;
        _bezierDistances.ForEach(bd => distSum += bd);
        _time = 0.75f * distSum;

        //if (_debug) {
        //  Debug.Log("BezierCurve.pointCount: " + BezierCurve.pointCount);
        // }

        _bezierPoints = BezierCurve.GetAnchorPoints();

        calculateBezierHandles();

        drawPath();
    }

    private void createBezier()
    {
        BezierCurve.Clear();
        foreach (Transform bzPointGo in BezierCurve.transform)
        {
            Destroy(bzPointGo.gameObject);
        }

        generateDebugPoint(DebugPoint.BzPoint, Vector3.zero, 0);
        BezierCurve.AddPointAt(Vector3.zero);

        var previousP = Vector2.zero;
        for (int i = 0; i < _pointsCount; i++)
        {
            previousP = createRandomBezierPoint(previousP, i);
        }
    }

    private void drawPath()
    {
        var points = getPoints();
        drawLine(points);
        _evenPoints = getEvenlyDistributedPoints(points);

        if (_debug)
        {
            showOriginalPoints(points);
            showCalculatedPoints(_evenPoints);

            // move the debug points a bit in front
            _originalParentT.position = new Vector3(0, 0, -0.3f);
            _calculatedParentT.position = new Vector3(0, 0, -0.5f);
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.W))
        {
            if (_isRunningForward) { return; }

            _startTime = Time.time;
            _isRunningForward = true;
            _isRunningBackward = false;
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            if (_isRunningBackward) { return; }

            _startTime = Time.time;
            _isRunningForward = false;
            _isRunningBackward = true;
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            runHazardlyOnOriginalBezierPath();
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            runOnEvenBezierPath();
        }
    }

    private void FixedUpdate()
    {
        if (_isRunningForward)
        {
            runForward();
        }
        if (_isRunningBackward)
        {
            runBackward();
        }
    }

    private Vector3[] getPoints()
    {
        var count = (_bezierPoints.Length - 1) * (int)_tFragments;
        var positions = new Vector3[count];
        var pI = 0;

        for (int i = 0; i < _bezierPoints.Length; i++)
        {
            int nextI = i + 1;
            if (_bezierPoints.Length == nextI) { break; }

            for (int p = 0; p < (int)_tFragments; p++)
            {
                var t = p / _tFragments;
                var point = BezierCurve.GetPoint(_bezierPoints[i], _bezierPoints[nextI], t);
                positions[pI] = point;
                pI++;
            }
        }

        return positions;
    }

    private void drawLine(Vector3[] positions)
    {
        LineRenderer.positionCount = positions.Length;
        LineRenderer.SetPositions(positions);
    }

    private void showOriginalPoints(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            generateDebugPoint(DebugPoint.OriginalPoint, points[i], i);
        }
    }

    private void showCalculatedPoints(Vector3[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            generateDebugPoint(DebugPoint.CalculatedPoint, points[i], i);
        }
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

    private void generateDebugPoint(DebugPoint debugPoint, Vector3 point, int i)
    {
        var prefab = _bzPointPrefab;
        Transform parent = null;
        switch (debugPoint)
        {
            case DebugPoint.OriginalPoint:
                prefab = _originalPoint;
                parent = _originalParentT;
                break;
            case DebugPoint.CalculatedPoint:
                prefab = _calculatedPoint;
                parent = _calculatedParentT;
                break;
            case DebugPoint.BzPoint:
            default:
                break;
        }
        var go = Instantiate(prefab, parent);
        go.transform.position = point;
        go.name = "[" + i + "-" + (i + 1) + "]";
    }

    private Vector2 createRandomBezierPoint(Vector2 prevP, int i)
    {
        var yr = Random.Range(0f, 2f) - 1f;
        var x = 1;
        var dir = new Vector2(x, yr).normalized;
        // Debug.Log("[" + i + "]       dir: " + dir);
        var distanceToNextPoint = getDistanceToNextPoint(prevP.y, dir);
        _bezierDistances.Add(distanceToNextPoint);
        // Debug.Log("distanceToNextPoint: " + distanceToNextPoint);
        // Debug.Log("prevP: " + prevP);
        var pointOffset = dir * distanceToNextPoint;
        // Debug.Log("pointOffset: " + pointOffset);

        if (pointOffset.x > 4)
        {
            pointOffset = new Vector2(Random.Range(2.9f, 3.9f), pointOffset.y);
            // Debug.Log("RECALCULATING? pointOffset: " + pointOffset);
        }

        var p = prevP + pointOffset;
        // Debug.Log("p: " + p);

        generateDebugPoint(DebugPoint.BzPoint, p, i);

        var bzPoint = BezierCurve.AddPointAt(p);
        bzPoint.handle1 = new Vector3(1, 0, 0);
        return p;
    }

    private void calculateBezierHandles()
    {
        if (_bezierPoints.Length < 3)
        {
            return;
        }

        for (int i = 1; i < _bezierPoints.Length - 1; i++)
        {
            // Debug.Log("handle [" + (i - 1) + "," + i + "," + (i + 1) + "]");

            var a = _bezierPoints[i - 1].position;
            var b = _bezierPoints[i].position;
            var c = _bezierPoints[i + 1].position;
            var dirAB = a - b;
            dirAB.Normalize();
            // Debug.Log("dirAB: " + dirAB);
            var dirCB = c - b;
            dirCB.Normalize();
            // Debug.Log("dirCB: " + dirCB);
            var inverseDirCB = new Vector3(dirCB.x * -1, dirCB.y * -1, 0);
            // Debug.Log("inverseDir: " + inverseDirCB);


            var averageDirection = (dirAB + inverseDirCB).normalized;
            var middlePoint = (Vector3.zero + averageDirection).normalized;

            var distAB = Vector2.Distance(a, b);
            // Debug.Log("distAB: " + distAB);
            var distBC = Vector2.Distance(b, c);
            // Debug.Log("distBC: " + distBC);
            var averageDistanceABC = (distAB + distBC) / 2f;
            // Debug.Log("averageDistanceABC: " + averageDistanceABC);


            _bezierPoints[i].handle1 = middlePoint * (averageDistanceABC * Random.Range(0.2f, 0.75f));
        }
    }

    private float getDistanceToNextPoint(float prevY, Vector2 dir)
    {
        bool isNewPointPositive = dir.y > 0;
        float maxCurrentY = isNewPointPositive
            ? MAX_Y - prevY
            : Mathf.Abs(-MAX_Y - prevY);
        float distance = Random.Range(maxCurrentY < 3f ? 1f : 3f, maxCurrentY);
        return distance < 2f ? 2.2f : distance;
        //return distance;
    }

    private void runForward()
    {
        float journeyDuration = Time.time - _startTime;
        float fractionOfJourney = journeyDuration / _speed;

        int nextI = _index + 1;
        if (nextI == _evenPoints.Length)
        {
            _isRunningForward = false;
            return;
        }
        if (nextI == _evenPoints.Length - 2)
        {
            // create new path and link it to this one
            // get the last point, use it a an offset of a start
            // generate the path from there
            // make the first node of that new path sync with the handle of the last node from the current path
            // make functionality that jumps from one to another
            // after some time destroy the last path and clear memory
        }

        _walker.transform.position = Vector3.Lerp(_evenPoints[_index], _evenPoints[nextI], fractionOfJourney);

        if (fractionOfJourney >= 0.99f)
        {
            // Debug.Log("fractionOfJourney: " + fractionOfJourney);
            _startTime = Time.time;
            _index++;
        }
    }

    private void runBackward()
    {
        float journeyDuration = Time.time - _startTime;
        float fractionOfJourney = journeyDuration / _speed;

        int nextI = _index - 1;
        if (nextI == -1)
        {
            _isRunningBackward = false;
            return;
        }

        _walker.transform.position = Vector3.Lerp(_evenPoints[_index], _evenPoints[nextI], fractionOfJourney);

        if (fractionOfJourney >= 0.99f)
        {
            // Debug.Log("fractionOfJourney: " + fractionOfJourney);
            _startTime = Time.time;
            _index--;
        }
    }

    private void runHazardlyOnOriginalBezierPath()
    {
        _index = 0;
        continueWalkingOnThePath();
    }

    private void continueWalkingOnThePath()
    {
        int nextIndex = _index + 1;
        // Debug.Log("nextIndex: " + nextIndex);
        bool reachedEnd = _bezierPoints.Length == nextIndex;
        // Debug.Log("reachedEnd: " + reachedEnd);

        if (reachedEnd) { return; }

        _currentBezierPointPath = new BezierPoint[2] { _bezierPoints[_index], _bezierPoints[nextIndex] };

        if (_moveTwid.HasValue)
        {
            LeanTween.cancel(_moveTwid.Value, callOnComplete: true);
            _moveTwid = null;
        }

        _moveTwid = LeanTween.value(0, 1f, _time / _bezierPoints.Length).id;

        LeanTween.descr(_moveTwid.Value)
            .setOnUpdate((float value) =>
            {
                var point = BezierCurve.GetPoint(_currentBezierPointPath[0], _currentBezierPointPath[1], value);
                _walker.transform.position = point;
            })
            .setOnComplete(() =>
            {
                _index++;
                continueWalkingOnThePath();
            });
    }

    private void runOnEvenBezierPath()
    {
        LTSpline spline = new LTSpline(_evenPoints);
        LeanTween.moveSpline(_walker, spline, _time)
            .setDelay(.3f)
            .setEase(LeanTweenType.linear);
    }
}
