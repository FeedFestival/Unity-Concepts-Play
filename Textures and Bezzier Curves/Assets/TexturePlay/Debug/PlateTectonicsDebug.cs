using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TexturePlay
{
    public class PlateTectonicsDebug : MonoBehaviour
    {
        [SerializeField]
        private TexturePlayDebug _texturePlayDebugSettings;

        [Header("Transforms")]
        [SerializeField]
        private Transform _pointsTransform;
        [SerializeField]
        private Transform _gridPointsTransform;
        [SerializeField]
        private Transform _movedPointsTransform;

        internal void showDebugPoints(Dictionary<VoronoiEdge, List<Point>> edgesPoints)
        {
            showDebugPoints(edgesPoints[VoronoiEdge.Left], _texturePlayDebugSettings.OuterEdgePoint);
            showDebugPoints(edgesPoints[VoronoiEdge.Bottom], _texturePlayDebugSettings.OuterEdgePoint);
            showDebugPoints(edgesPoints[VoronoiEdge.InnerLeft], _texturePlayDebugSettings.MiddlePoint, 0.65f);
            showDebugPoints(edgesPoints[VoronoiEdge.InnerBottom], _texturePlayDebugSettings.MiddlePoint, 0.65f);
            foreach (var kvp in edgesPoints)
            {
                showDebugGridPoints(kvp.Value, _texturePlayDebugSettings.GridPoint);
            }
            showDebugPoints(edgesPoints[VoronoiEdge.Right], _texturePlayDebugSettings.OuterEdgePoint);
            showDebugPoints(edgesPoints[VoronoiEdge.Top], _texturePlayDebugSettings.OuterEdgePoint);
            showDebugPoints(edgesPoints[VoronoiEdge.InnerRight], _texturePlayDebugSettings.MiddlePoint, 0.65f);
            showDebugPoints(edgesPoints[VoronoiEdge.Middle], _texturePlayDebugSettings.MiddlePoint, 0.5f);
        }

        private void showDebugPoints(List<Point> edgePoints, Material material, float scaleMultiplier = 0.35f)
        {
            foreach (var point in edgePoints)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                go.name = point.edge.ToString() + " (" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
                go.transform.SetParent(_pointsTransform);
                go.transform.localScale = Vector3.one * scaleMultiplier;
                go.transform.position = point.worldPosition;
                var rendererRef = go.GetComponent<MeshRenderer>();
                rendererRef.material = material;
            }
        }

        private void showDebugGridPoints(List<Point> edgePoints, Material material)
        {
            foreach (var point in edgePoints)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = "(" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
                go.transform.SetParent(_gridPointsTransform);
                go.transform.localScale = Vector3.one * 0.4f;
                go.transform.position = point.gridWorldPosition;
                var rendererRef = go.GetComponent<MeshRenderer>();
                rendererRef.material = material;
            }
        }

        private void showDebugMovedPoint(VoronoiEdge edge, Point point, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = edge + " (" + point.index + ")" + point.gridCoord.x + " _ " + point.gridCoord.y;
            go.transform.SetParent(_movedPointsTransform);
            go.transform.localScale = Vector3.one * 0.4f;
            go.transform.position = point.worldPosition;
            var rendererRef = go.GetComponent<MeshRenderer>();
            rendererRef.material = material;
        }
    }
}
