using UnityEngine;

[CreateAssetMenu(fileName = "GridSettings", menuName = "Mathematics/GridSettings", order = 1)]
public class GridSettings : ScriptableObject
{
    public GameObject PointPrefab;
    public GameObject GridLinePrefab;
    public GameObject LinePrefab;
    public float LineSegmentLenght;
}
