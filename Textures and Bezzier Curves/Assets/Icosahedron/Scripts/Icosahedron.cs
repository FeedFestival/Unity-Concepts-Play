using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Icosahedron
{
    public class Icosahedron
    {
        private readonly List<Vector3> _vertices;
        private readonly List<int[]> _triangles;

        public Icosahedron()
        {
            var t = (1 + Mathf.Sqrt(5)) / 2;
            Debug.Log("_t: " + t);
            var multiplier = 1 / Mathf.Sqrt(1 + Mathf.Pow(t, 2));
            Debug.Log("multiplier: " + multiplier);

            _vertices = new List<Vector3>()
            {
                multiplier * new Vector3(t, 1, 0),
                multiplier * new Vector3(-t, 1, 0),
                multiplier * new Vector3(t, -1, 0),
                multiplier * new Vector3(-t, -1, 0),
                multiplier * new Vector3(1, 0, t),
                multiplier * new Vector3(1, 0, -t),
                multiplier * new Vector3(-1, 0, t),
                multiplier * new Vector3(-1, 0, -t),
                multiplier * new Vector3(0, t, 1),
                multiplier * new Vector3(0, -t, 1),
                multiplier * new Vector3(0, t, -1),
                multiplier * new Vector3(0, -t, -1),
            };

            _triangles = new List<int[]>()
            {
                new int[3] { 10, 5, 7 },    // 0
                new int[3] { 0, 5, 10 },    // 1
                new int[3] { 0, 10, 8 },    // 2
                new int[3] { 1, 8, 10 },    // 3
                new int[3] { 1, 10, 7 },    // 4
                //
                new int[3] { 1, 6, 8 },     // 5
                new int[3] { 3, 9, 6 },     // 6
                new int[3] { 3, 7, 11 },    // 7
                new int[3] { 2, 4, 9 },     // 8
                new int[3] { 2, 11, 5 },    // 9
                //
                new int[3] { 2, 9, 11 },    // 10
                new int[3] { 3, 11, 9 },    // 11   (3, 9, 11)
                new int[3] { 4, 2, 0 },     // 12
                new int[3] { 5, 0, 2 },     // 13
                new int[3] { 6, 1, 3 },     // 14
                //
                new int[3] { 7, 3, 1 },     // 15
                new int[3] { 8, 6, 4 },     // 16
                new int[3] { 9, 4, 6 },     // 17
                new int[3] { 0, 8, 4 },     // 18
                new int[3] { 11, 7, 5 },    // 19
            };

            //debugAllTriangles();

            debugCurrentTriangle();
        }

        internal List<float> CalculateAreas(ref List<Vector3> vertices, ref List<int[]> triangles)
        {
            var areas = new List<float>();
            foreach (var triangleIndexes in triangles)
            {
                var a = vertices[triangleIndexes[0]];
                var b = vertices[triangleIndexes[1]];
                var c = vertices[triangleIndexes[2]];
                var area = calculateAreaOfTrinagle(a, b, c);
                areas.Add(area);
            }
            return areas;
        }

        internal List<Vector3> GetVertices()
        {
            return _vertices;
        }

        internal List<int[]> GetTriangles()
        {
            return _triangles;
        }

        internal void MultiplyIcosahedron2(ref MeshFilter meshFilter, ref List<Vector3> vertices, ref List<int[]> triangles, out List<Vector3> newVertices, out List<int[]> newTriangles)
        {
            newVertices = new List<Vector3>(vertices);  // Keep old vertices.
            newTriangles = new List<int[]>();

            foreach (var triangle in triangles)
            {
                // Get the vertices of the current triangle.
                var a = vertices[triangle[0]];
                var b = vertices[triangle[1]];
                var c = vertices[triangle[2]];

                // Calculate the midpoints and normalize them.
                var ab = ((a + b) * 0.5f).normalized;
                var bc = ((b + c) * 0.5f).normalized;
                var ca = ((c + a) * 0.5f).normalized;

                // Add the new vertices and get their indices.
                newVertices.AddRange(new[] { ab, bc, ca });
                int abI = newVertices.Count - 3;
                int bcI = newVertices.Count - 2;
                int caI = newVertices.Count - 1;

                // Create the new triangles.
                newTriangles.Add(new[] { triangle[0], abI, caI });
                newTriangles.Add(new[] { triangle[1], bcI, abI });
                newTriangles.Add(new[] { triangle[2], caI, bcI });
                newTriangles.Add(new[] { abI, bcI, caI });
            }

            CreateIcosahedron(ref meshFilter, newVertices, newTriangles);
        }

        internal void MultiplyIcosahedron(ref MeshFilter meshFilter, ref List<Vector3> vertices, ref List<int[]> triangles, out List<Vector3> newVertices, out List<int[]> newTriangles)
        {
            var startIndex = -1;    // this is the legth of the current verteces

            newVertices = new List<Vector3>();
            newTriangles = new List<int[]>();
            foreach (var triangleIndexes in triangles)
            {
                // init
                int aI = startIndex + 1;
                int bI = aI + 1;
                int cI = bI + 1;
                int abI = cI + 1;
                int bcI = abI + 1;
                int caI = bcI + 1;

                // subdivide
                var a = vertices[triangleIndexes[0]];
                var b = vertices[triangleIndexes[1]];
                var c = vertices[triangleIndexes[2]];
                Debug.Log("a: " + a);
                Debug.Log("b: " + b);
                Debug.Log("c: " + c);
                var ab = (a + b) * 0.5f;
                var bc = (b + c) * 0.5f;
                var ca = (c + a) * 0.5f;
                Debug.Log("abm: " + ab);
                Debug.Log("bcm: " + bc);
                Debug.Log("cam: " + ca);

                // distance from center
                var dirAB = ab - Vector3.zero;
                Debug.Log("dirAB: " + dirAB);
                Debug.Log("dirAB.normalized: " + dirAB.normalized);
                ab = dirAB.normalized;
                bc = (bc - Vector3.zero).normalized;
                ca = (ca - Vector3.zero).normalized;

                newVertices.AddRange(new Vector3[6] { a, b, c, ab, bc, ca });

                newTriangles.Add(new int[3] { aI, abI, caI });
                newTriangles.Add(new int[3] { caI, bcI, cI });
                newTriangles.Add(new int[3] { caI, abI, bcI });
                newTriangles.Add(new int[3] { abI, bI, bcI });

                Debug.Log("-----------------");
                //break;
                startIndex = newVertices.Count - 1;
            }

            CreateIcosahedron(ref meshFilter, newVertices, newTriangles);
        }

        public Vector3[] PointsOnSphere(int n)
        {
            List<Vector3> upts = new List<Vector3>();
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2.0f / n;
            float x = 0;
            float y = 0;
            float z = 0;
            float r = 0;
            float phi = 0;

            for (var k = 0; k < n; k++)
            {
                y = k * off - 1 + (off / 2);
                r = Mathf.Sqrt(1 - y * y);
                phi = k * inc;
                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;

                upts.Add(new Vector3(x, y, z));
            }
            Vector3[] pts = upts.ToArray();
            return pts;
        }

        public void PointsOnSphere(ref List<Vector3> vertices)
        {
            float inc = Mathf.PI * (3 - Mathf.Sqrt(5));
            float off = 2.0f / vertices.Count;

            for (var k = 0; k < vertices.Count; k++)
            {
                float x = vertices[k].x;
                float y = vertices[k].y;
                float z = vertices[k].z;
                y = k * off - 1 + (off / 2);
                float r = Mathf.Sqrt(1 - y * y);
                float phi = k * inc;
                x = Mathf.Cos(phi) * r;
                z = Mathf.Sin(phi) * r;

                vertices[k] = new Vector3(x, y, z);
            }
        }

        public void CreateIcosahedron(ref MeshFilter meshFilter)
        {
            CreateIcosahedron(ref meshFilter, _vertices, _triangles);

        }

        public void CreateIcosahedron(ref MeshFilter meshFilter, List<Vector3> vertices, List<int[]> triangles)
        {
            meshFilter.mesh.vertices = vertices.ToArray();
            var allTriangles = new List<int>();
            foreach (var tris in triangles)
            {
                foreach (var t in tris)
                {
                    allTriangles.Add(t);
                }
            }

            meshFilter.mesh.triangles = allTriangles.ToArray();

            meshFilter.mesh.RecalculateBounds();
            meshFilter.mesh.RecalculateNormals();
            meshFilter.mesh.Optimize();
        }

        private void debugAllTriangles()
        {
            int i = 0;
            foreach (var tris in _triangles)
            {
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.name = i + " [" + tris[0] + ", " + tris[1] + ", " + tris[2] + "]";
                go.transform.localScale = Vector3.one * 0.1f;

                // 
                var a = _vertices[tris[0]];
                var b = _vertices[tris[1]];
                var c = _vertices[tris[2]];
                var center = new Vector3(
                   (a.x + b.x + c.x) / 3,
                   (a.y + b.y + c.y) / 3,
                   (a.z + b.z + c.z) / 3
                );

                go.transform.position = center;

                i++;
            }
        }

        private void debugCurrentTriangle()
        {
            var currentTraingle = _triangles[0];
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "[" + currentTraingle[0] + ", " + currentTraingle[1] + ", " + currentTraingle[2] + "]";
            go.transform.localScale = Vector3.one * 0.1f;
            var a = _vertices[currentTraingle[0]];
            var b = _vertices[currentTraingle[1]];
            var c = _vertices[currentTraingle[2]];
            var center = new Vector3(
               (a.x + b.x + c.x) / 3,
               (a.y + b.y + c.y) / 3,
               (a.z + b.z + c.z) / 3
            );
            go.transform.position = center;

            int i = 0;
            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "(" + i + ") A vertex [" + currentTraingle[i] + "]";
            go.transform.localScale = Vector3.one * 0.1f;
            go.transform.position = a;
            i++;

            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "(" + i + ") B vertex [" + currentTraingle[i] + "]";
            go.transform.localScale = Vector3.one * 0.17f;
            go.transform.position = b;
            i++;

            go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.name = "(" + i + ") C vertex [" + currentTraingle[i] + "]";
            go.transform.localScale = Vector3.one * 0.24f;
            go.transform.position = c;
            i++;
        }

        private float calculateAreaOfTrinagle(Vector3 n1, Vector3 n2, Vector3 n3)
        {
            float res = Mathf.Pow(((n2.x * n1.y) - (n3.x * n1.y) - (n1.x * n2.y) + (n3.x * n2.y) + (n1.x * n3.y) - (n2.x * n3.y)), 2.0f);
            res += Mathf.Pow(((n2.x * n1.z) - (n3.x * n1.z) - (n1.x * n2.z) + (n3.x * n2.z) + (n1.x * n3.z) - (n2.x * n3.z)), 2.0f);
            res += Mathf.Pow(((n2.y * n1.z) - (n3.y * n1.z) - (n1.y * n2.z) + (n3.y * n2.z) + (n1.y * n3.z) - (n2.y * n3.z)), 2.0f);
            return Mathf.Sqrt(res) * 0.5f;
        }
    }
}
