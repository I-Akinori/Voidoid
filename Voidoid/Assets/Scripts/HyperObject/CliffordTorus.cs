using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class CliffordTorus : BaseHyperObject
    {
        private Mesh mesh;
        [SerializeField, Range(0, 128)] private int thetaRes = 8;
        [SerializeField, Range(0, 128)] private int phiRes = 64;
        private static Material material;

        protected override void SetPoints()
        {
            points4d.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                var theta = i * 2 * Mathf.PI / thetaRes;
                for (var j = 0; j < phiRes; j++)
                {
                    var phi = j * 2 * Mathf.PI / phiRes;
                    var sqrt2Inv = 1f / Mathf.Sqrt(2);
                    points4d.Add(new Vector4(
                        Mathf.Sin(theta),
                        Mathf.Cos(theta),
                        Mathf.Cos(phi),
                        Mathf.Sin(phi)
                    ) * sqrt2Inv); 
                }
            }
        }
        protected override void SetEdges()
        {
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    var i0 = i * phiRes + j;
                    var i1 = i * phiRes + (j < phiRes - 1 ? j + 1 : 0);
                    var i2 = (i < thetaRes - 1 ? i + 1 : 0) * phiRes + j;
                    //var col = Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1);
                    edges.Add(new Vector2Int(i0, i1));
                    edges.Add(new Vector2Int(i0, i2));
                    //Debug.DrawLine(points3d[i0], points3d[i1], col);
                    //Debug.DrawLine(points3d[i0], points3d[i2], Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1));
                }
            }
        }
        protected override void SetColors()
        {
            colors.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    var col = Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1);
                    colors.Add(col);
                }
            }
        }
        private void DrawFace()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            var positions = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var uvs = new List<Vector2>();
            var a = 0.5f; 
            var col = new Color(1, 0, 0, a);
            
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    var i0 = i * phiRes + j;
                    var i1 = i * phiRes + (j < phiRes - 1 ? j + 1 : 0);
                    var i2 = (i < thetaRes - 1 ? i + 1 : 0) * phiRes + j;
                    var i3 = (i < thetaRes - 1 ? i + 1 : 0) * phiRes + (j < phiRes - 1 ? j + 1 : 0);
                    //var col = Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1);
                    AddFace(points3d[i0], points3d[i1], points3d[i3], points3d[i2], ref positions, ref triangles);
                    var u0 = (float)i / thetaRes;
                    var u1 = (float)(i + 1) / thetaRes;
                    var v0 = (float)j / phiRes;
                    var v1 = (float)(j + 1) / phiRes;
                    uvs.Add(new Vector2(u0, v0));
                    uvs.Add(new Vector2(u0, v1));
                    uvs.Add(new Vector2(u1, v1));
                    uvs.Add(new Vector2(u1, v0));
                }
            }

            if (this.mesh == null)
            {
                this.mesh = new Mesh();
            }
            this.mesh.vertices = positions.ToArray();
            this.mesh.triangles = triangles.ToArray();
            this.mesh.uv = uvs.ToArray();
            this.mesh.colors = colors.ToArray();
            this.mesh.RecalculateNormals();
            Graphics.DrawMesh(this.mesh, Vector3.zero, Quaternion.identity, material, 0);
        }

        private void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, ref List<Vector3> ps, ref List<int> ts)
        {
            var index = ps.Count;
            ps.Add(v1); ps.Add(v2); ps.Add(v3); ps.Add(v4);
            ts.Add(index); ts.Add(index + 1); ts.Add(index + 2);
            ts.Add(index); ts.Add(index + 2); ts.Add(index + 3);
        }
    }
}