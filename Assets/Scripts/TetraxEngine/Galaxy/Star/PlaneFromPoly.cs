using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TetraxEngine.Galaxy.Star
{
    public class PlaneFromPoly : MonoBehaviour {
     
        public Material mat;

        public delegate void MouseAction();
        private MouseAction onCellClick;
     
        public void SetPolyMesh (Vector3[] poly, MouseAction callback)
        {
            if (poly == null || poly.Length < 3) {
                Debug.Log ("Define 2D polygon in 'poly' in the the Inspector");
                return;
            }

            onCellClick = callback;
         
            var mf = gameObject.AddComponent<MeshFilter>();
            mf.mesh = new Mesh();
         
            Renderer rend = gameObject.AddComponent<MeshRenderer>();
            rend.material = mat;
 
            var center = FindCenter(poly);
                 
            var vertices = new Vector3[poly.Length+1];
            vertices[0] = Vector3.zero;

            for (var i = 0; i < poly.Length; i++)
            {
                poly[i].z = 0.0f;
                vertices[i+1] = poly[i] - center;
            }
         
            mf.mesh.vertices = vertices;
         
            var triangles = new int[poly.Length*3*2];
         
            for (var i = 0; i < poly.Length-1; i++)
            {
                triangles[i*3] = i+2;
                triangles[i*3+1] = 0;
                triangles[i*3+2] = i + 1;
            }
         
            triangles[(poly.Length-1)*3] = 1;
            triangles[(poly.Length-1)*3+1] = 0;
            triangles[(poly.Length-1)*3+2] = poly.Length;
         
            mf.mesh.triangles = triangles;
            mf.mesh.uv = BuildUVs(vertices);
         
            mf.mesh.RecalculateBounds();
            mf.mesh.RecalculateNormals();
         
            mf.mesh.SetIndices(mf.mesh.GetIndices(0).Concat(mf.mesh.GetIndices(0).Reverse()).ToArray(), MeshTopology.Triangles, 0);

            gameObject.AddComponent<MeshCollider>();
        }
     
        private static Vector3 FindCenter(IReadOnlyCollection<Vector3> poly) {
            var center = poly.Aggregate(Vector3.zero, (current, v3) => current + v3);
            return center / poly.Count;
        }

        private static Vector2[] BuildUVs(IList<Vector3> vertices)
        {
         
            var xMin = Mathf.Infinity;
            var yMin = Mathf.Infinity;
            var xMax = -Mathf.Infinity;
            var yMax = -Mathf.Infinity;
         
            foreach (var v3 in vertices) {
                if (v3.x < xMin)
                    xMin = v3.x;
                if (v3.y < yMin)
                    yMin = v3.y;
                if (v3.x > xMax)
                    xMax = v3.x;
                if (v3.y > yMax)
                    yMax = v3.y;
            }
         
            var xRange = xMax - xMin;
            var yRange = yMax - yMin;
             
            var uvs = new Vector2[vertices.Count];
            for (var i = 0; i < vertices.Count; i++) {
                uvs[i].x = (vertices[i].x - xMin) / xRange;
                uvs[i].y = (vertices[i].y - yMin) / yRange;
             
            }
            return uvs;
        }
     
        private void OnMouseUp()
        {
            onCellClick();
        }
    }
}