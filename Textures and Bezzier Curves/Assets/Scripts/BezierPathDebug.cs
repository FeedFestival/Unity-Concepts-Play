using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BezierPathDebug : MonoBehaviour
{
    Dictionary<DebugPoint, Transform> _parentDict = new Dictionary<DebugPoint, Transform>();

    public enum DebugPoint { OriginalPoint, CalculatedPoint, BzPoint }

    internal void ShowPoints(PathCreatorSObject pathCreatorSettings, DebugPoint pointType, Vector3[] points)
    {
        createParentT(pointType);

        for (int i = 0; i < points.Length; i++)
        {
            generateDebugPoint(pathCreatorSettings, pointType, points[i], i);
        }
    }

    private void createParentT(DebugPoint pointType)
    {
        if (_parentDict.ContainsKey(pointType) == false)
        {
            var go = new GameObject();
            go.name = "___" + pointType.ToString() + "T" + "___";
            _parentDict.Add(pointType, go.transform);
        }
    }

    private void generateDebugPoint(PathCreatorSObject pathCreatorSettings, DebugPoint pointType, Vector3 point, int i)
    {
        var prefab = pathCreatorSettings.BezierPointPrefab;
        Transform parent = _parentDict[pointType];
        switch (pointType)
        {
            case DebugPoint.OriginalPoint:
                prefab = pathCreatorSettings.OriginalPointPrefab;
                break;
            case DebugPoint.CalculatedPoint:
                prefab = pathCreatorSettings.CalculatedPointPrefab;
                break;
            case DebugPoint.BzPoint:
            default:
                break;
        }
        var go = Instantiate(prefab, parent);
        go.transform.position = point;
        go.name = "[" + i + "-" + (i + 1) + "]";
    }
}
