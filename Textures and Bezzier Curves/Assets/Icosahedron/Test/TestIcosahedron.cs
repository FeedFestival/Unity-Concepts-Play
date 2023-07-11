using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Icosahedron
{
    public class TestIcosahedron : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _meshFilter;

        [SerializeField]
        private MeshFilter _meshFilter2x;

        void Start()
        {
            var icosahedron = new Icosahedron();

            icosahedron.CreateIcosahedron(ref _meshFilter);

            var vertices = icosahedron.GetVertices();
            var triangles = icosahedron.GetTriangles();
            List<Vector3> vertices2x;
            List<int[]> triangles2x;
            icosahedron.MultiplyIcosahedron2(ref _meshFilter2x, ref vertices, ref triangles, out vertices2x, out triangles2x);

            List<Vector3> vertices3x;
            List<int[]> triangles3x;
            icosahedron.MultiplyIcosahedron2(ref _meshFilter2x, ref vertices2x, ref triangles2x, out vertices3x, out triangles3x);

            //var areas = icosahedron.CalculateAreas(ref vertices3x, ref triangles3x);
            //foreach (var area in areas)
            //{
            //    Debug.Log("area: " + area);
            //}

            ////icosahedron.PointsOnSphere(ref vertices3x);

            List<Vector3> vertices4x;
            List<int[]> triangles4x;
            icosahedron.MultiplyIcosahedron2(ref _meshFilter2x, ref vertices3x, ref triangles3x, out vertices4x, out triangles4x);

            var areas = icosahedron.CalculateAreas(ref vertices4x, ref triangles4x);
            foreach (var area in areas)
            {
                Debug.Log("area: " + area);
            }

            //List<Vector3> vertices5x;
            //List<int[]> triangles5x;
            //icosahedron.MultiplyIcosahedron2(ref _meshFilter2x, ref vertices4x, ref triangles4x, out vertices5x, out triangles5x);

            //var areas = icosahedron.CalculateAreas(ref vertices5x, ref triangles5x);
            //foreach (var area in areas)
            //{
            //    Debug.Log("area: " + area);
            //}
        }
    }
}
