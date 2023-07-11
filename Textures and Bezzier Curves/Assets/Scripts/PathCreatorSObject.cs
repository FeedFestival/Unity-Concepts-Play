using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PathCreatorSObject", order = 1)]
public class PathCreatorSObject : ScriptableObject
{
    public GameObject BezierPointPrefab;
    public GameObject OriginalPointPrefab;
    public GameObject CalculatedPointPrefab;
}
