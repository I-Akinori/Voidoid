using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public struct Vector5
    {
        public Vector5(float x, float y, float z, float w, float v)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.v = v;
        }

        public float GetElement(int i)
        {
            switch (i)
            {
                case 0:
                    return x;
                case 1:
                    return y;
                case 2:
                    return z;
                case 3:
                    return w;
                case 4:
                    return v;
                default:
                    return 0;
            }
        }
        public void SetElement(int i, float value)
        {
            switch (i)
            {
                case 0:
                    this.x = value;
                    return;
                case 1:
                    this.y = value;
                    return;
                case 2:
                    this.z = value;
                    return;
                case 3:
                    this.w = value;
                    return;
                case 4:
                    this.v = value;
                    return;
                default:
                    return;
            }
        }
        public float x, y, z, w, v;
    }
    public struct Vector6
    {
        public Vector6(float x, float y, float z, float w, float v, float u)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
            this.v = v;
            this.u = u;
        }
        public float x, y, z, w, v, u;
    }
    public class ExtraHyperCube : MonoBehaviour
    {
        [SerializeField] private List<Vector5> points5d  = new List<Vector5>();
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private bool expanding = true;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (var i = 0; i < 32; i++)
            {
                points5d.Add(
                    new Vector5(
                        ((i >> 0) & 1) == 0 ? -1f : 1f, 
                        ((i >> 1) & 1) == 0 ? -1f : 1f, 
                        ((i >> 2) & 1) == 0 ? -1f : 1f, 
                        ((i >> 3) & 1) == 0 ? -1f : 1f, 
                        ((i >> 4) & 1) == 0 ? -1f : 1f)
                    );
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (expanding)
            {
                ExpandDimension();
            }
            else
            {
                var dt = Time.deltaTime;
                time += dt;
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 0)
                {
                    Rotation5d(ref points5d, dt, 0, 4);
                }
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 1)
                {
                    Rotation5d(ref points5d, dt, 1, 3);
                }
                if (Mathf.FloorToInt(time / (Mathf.PI * 2)) % 3 == 2)
                {
                    Rotation5d(ref points5d, dt, 3, 4);
                }
            }
            DrawEdge();
            //DrawFace();
        }

        private void ExpandDimension()
        {
            var sx = Mathf.Clamp01(Time.time - 0);
            var sy = Mathf.Clamp01(Time.time - 1);
            var sz = Mathf.Clamp01(Time.time - 2);
            var sw = Mathf.Clamp01(Time.time - 3);
            var sv = Mathf.Clamp01(Time.time - 4);
            for (var i = 0; i < 32; i++)
            {
                points5d[i] = 
                    new Vector5(
                        (((i >> 0) & 1) == 0 ? -1 : 1) * sx, 
                        (((i >> 1) & 1) == 0 ? -1 : 1) * sy, 
                        (((i >> 2) & 1) == 0 ? -1 : 1) * sz, 
                        (((i >> 3) & 1) == 0 ? -1 : 1) * sw,
                            (((i >> 4) & 1) == 0 ? -1 : 1) * sv
                        );
            }

            if (sv > 0.999f)
            {
                expanding = false;
            }
        }

        private void Rotation5d(ref List<Vector5> points, float angle, int ax1, int ax2)
        {
            var c = Mathf.Cos(angle);
            var s = Mathf.Sin(angle);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                var e1 = point.GetElement(ax1);
                var e2 = point.GetElement(ax2);
                point.SetElement(ax1, e1 * c - e2 * s);
                point.SetElement(ax2, e1 * s + e2 * c);
                points[i] = point;
            }
        }
        private List<Vector4> Convert5DTo4D(List<Vector5> points, float d)
        {
            return points.Select(p =>
            {
                var parse = 1f / (d - p.v);
                return new Vector4(p.x, p.y, p.z, p.w) * parse;
            }).ToList();
        }

        private List<Vector3> Convert4DTo3D(List<Vector4> points, float d)
        {
            return points.Select(p =>
            {
                var parse = 1f / (d - p.w);
                return new Vector3(p.x, p.y, p.z) * parse;
            }).ToList();
        }

        private void DrawEdge()
        {
            var points4d = Convert5DTo4D(points5d, 3);
            var points3d = this.Convert4DTo3D(points4d, 3);
            for (var i = 0; i < 32; i++)
            {
                if (i % 2 < 1)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 1], Color.red);
                }
                if (i % 4 < 2)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 2], Color.green);
                }
                if (i % 8 < 4)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 4], Color.cyan);
                }
                if (i % 16 < 8)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 8], Color.purple);
                }
                if (i % 32 < 16)
                {
                    Debug.DrawLine(points3d[i], points3d[i + 16], Color.yellow);
                }
            }
        }
        private void DrawFace()
        {
            var points4d = this.Convert5DTo4D(points5d, 3);
            var points3d = this.Convert4DTo3D(points4d, 3);
            var positions = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var a = alpha; 
            var col = new Color(1, 0, 0, a);
            AddFace(points3d[0], points3d[1], points3d[3], points3d[2], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[12], points3d[13], points3d[15], points3d[14], col, ref positions, ref triangles, ref colors);
            
            col = new Color(0, 1, 0, a);
            AddFace(points3d[0], points3d[8], points3d[12], points3d[4], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[3], points3d[11], points3d[15], points3d[7], col, ref positions, ref triangles, ref colors);
            
            col = new Color(0, 1, 1, a);
            AddFace(points3d[1], points3d[3], points3d[7], points3d[5], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[8], points3d[10], points3d[14], points3d[12], col, ref positions, ref triangles, ref colors);
            
            col = new Color(1, 1, 0, a);
            AddFace(points3d[0], points3d[1], points3d[9], points3d[8], col, ref positions, ref triangles, ref colors);
            AddFace(points3d[6], points3d[7], points3d[15], points3d[14], col, ref positions, ref triangles, ref colors);
            
            var mesh = new Mesh();
            mesh.vertices = positions.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }

        private void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color col, ref List<Vector3> ps, ref List<int> ts, ref List<Color> cs)
        {
            var index = ps.Count;
            ps.Add(v1); ps.Add(v2); ps.Add(v3); ps.Add(v4);
            ts.Add(index); ts.Add(index + 1); ts.Add(index + 2);
            ts.Add(index); ts.Add(index + 2); ts.Add(index + 3);
            cs.Add(col); cs.Add(col); cs.Add(col); cs.Add(col);
        }
    }
}