using Game.Shared.Classes;
using Game.Shared.Enums;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    public class PlateTectonicsDebug : MonoBehaviour
    {
        [SerializeField]
        private TexturePlayDebug _texturePlayDebugSettings;

        internal void showDebugTectonics()
        {
            
        }

        //private void showDebugPoints(List<Point> edgePoints, Material material, float scaleMultiplier = 0.35f)
        //{
        //    foreach (var point in edgePoints)
        //    {
        //        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //        go.name = point.edge.ToString() + " (" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
        //        go.transform.SetParent(_pointsTransform);
        //        go.transform.localScale = Vector3.one * scaleMultiplier;
        //        var rendererRef = go.GetComponent<MeshRenderer>();
        //        rendererRef.material = material;
        //    }
        //}

        //private void showDebugGridPoints(List<Point> edgePoints, Material material)
        //{
        //    foreach (var point in edgePoints)
        //    {
        //        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //        go.name = "(" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
        //        go.transform.SetParent(_gridPointsTransform);
        //        go.transform.localScale = Vector3.one * 0.4f;
        //        var rendererRef = go.GetComponent<MeshRenderer>();
        //        rendererRef.material = material;
        //    }
        //}

        //private void showDebugMovedPoint(VoronoiEdge edge, Point point, Material material)
        //{
        //    var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    go.name = edge + " (" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
        //    go.transform.SetParent(_movedPointsTransform);
        //    go.transform.localScale = Vector3.one * 0.4f;
        //    var rendererRef = go.GetComponent<MeshRenderer>();
        //    rendererRef.material = material;
        //}
    }
}
