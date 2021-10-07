using UnityEngine;

public class Grid : MonoBehaviour
{
    private static Grid _grid;
    public static Grid _ { get { return _grid; } }
    private void Awake() { _grid = this; }

    public GridSettings GridSettings;

    public void ShowGridLine(Vector3 point)
    {
        Vector3 s = new Vector3(0, 0, -100);
        Vector3 e = new Vector3(0, 0, 100);
        GameObject go = Instantiate(GridSettings.LinePrefab, Vector3.zero, Quaternion.identity);
        LineRenderer lr = go.GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[] {
            s + new Vector3(point.x, point.y, 0),
            e + new Vector3(point.x, point.y, 0)
        });
    }

    public Point GetPoint()
    {
        GameObject go = Instantiate(GridSettings.PointPrefab, Vector3.zero, Quaternion.identity);
        return go.GetComponent<Point>();
    }
}
