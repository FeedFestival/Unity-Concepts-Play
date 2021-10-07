using System;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private static Grid _grid;
    public static Grid _ { get { return _grid; } }
    private void Awake() { _grid = this; }

    public GridSettings GridSettings;
    GameObject _emptyGameObject;
    Vector2 _toPos;
    int? _backgroundTweenId;
    bool _forwardMove = false;

    void Start()
    {
        InitBackgroundSkybox();
        MoveBackground();
    }

    public void ShowGridLine(Point point)
    {
        DrawGridLine(point, LineCoord.X);
        DrawGridLine(point, LineCoord.Y);
        DrawGridLine(point, LineCoord.Z);
    }

    public Point GetPoint()
    {
        GameObject go = Instantiate(GridSettings.PointPrefab, Vector3.zero, Quaternion.identity);
        return go.GetComponent<Point>();
    }

    public void ConnectPoints(Point p1, Point p2)
    {
        GameObject go = Instantiate(GridSettings.LinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = go.GetComponent<LineRenderer>();

        lr.SetPositions(new Vector3[] {
            p1.transform.position,
            p2.transform.position
        });
    }

    void DrawGridLine(Point point, LineCoord lineCoord)
    {
        Vector3 s;
        Vector3 e;
        SetLineSegment(lineCoord, out s, out e);

        GameObject go = Instantiate(GridSettings.GridLinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = go.GetComponent<LineRenderer>();

        var addStart = s + GetAddVector(point, lineCoord);
        var addEnd = e + GetAddVector(point, lineCoord);
        var midX = addStart + (addEnd - addStart) / 2;

        lr.SetPositions(new Vector3[] {
            addStart,
            midX,
            addEnd
        });

        go.transform.SetParent(point.transform);
    }

    void SetLineSegment(LineCoord lineCoord, out Vector3 s, out Vector3 e)
    {
        if (lineCoord == LineCoord.X)
        {
            s = new Vector3(-GridSettings.LineSegmentLenght, 0, 0);
            e = new Vector3(GridSettings.LineSegmentLenght, 0, 0);
        }
        else if (lineCoord == LineCoord.Y)
        {
            s = new Vector3(0, -GridSettings.LineSegmentLenght, 0);
            e = new Vector3(0, GridSettings.LineSegmentLenght, 0);
        }
        else
        {
            s = new Vector3(0, 0, -GridSettings.LineSegmentLenght);
            e = new Vector3(0, 0, GridSettings.LineSegmentLenght);
        }
    }

    Vector3 GetAddVector(Point point, LineCoord lineCoord)
    {
        if (lineCoord == LineCoord.X)
        {
            return new Vector3(0, point.Y, point.Z);
        }
        else if (lineCoord == LineCoord.Y)
        {
            return new Vector3(point.X, 0, point.Z);
        }
        else
        {
            return new Vector3(point.X, point.Y, 0);
        }
    }

    void InitBackgroundSkybox()
    {
        _emptyGameObject = new GameObject();
        _emptyGameObject.transform.SetParent(transform);
        _emptyGameObject.name = "_backgroundMover";
        _emptyGameObject.transform.position = Vector3.zero;
    }

    void MoveBackground()
    {
        _forwardMove = !_forwardMove;
        if (_forwardMove)
        {
            _toPos = Vector2.one * 6;
        }
        else
        {
            _toPos = Vector2.zero;
        }
        _backgroundTweenId = LeanTween.move(_emptyGameObject, _toPos, 1020f).id;
        LeanTween.descr(_backgroundTweenId.Value).setEase(LeanTweenType.linear);
        LeanTween.descr(_backgroundTweenId.Value).setOnUpdate((Vector2 newPos) =>
        {
            RenderSettings.skybox.SetTextureOffset("_MainTex", newPos);
        });
        LeanTween.descr(_backgroundTweenId.Value).setOnComplete(_ =>
        {
            LeanTween.cancel(_backgroundTweenId.Value);
            _backgroundTweenId = null;
            MoveBackground();
        });
    }
}

public enum LineCoord
{
    X, Y, Z
}