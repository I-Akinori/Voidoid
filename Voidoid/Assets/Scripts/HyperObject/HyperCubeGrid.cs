using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class HyperCubeGrid : MonoBehaviour
    {
        [SerializeField] private List<Vector4> points4d;
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private float moveTime = 0f;
        private bool expanding = true;
        private Dictionary<int, float> voxelData = new();
        private List<int> keys = new(); 
        private int axis1 = 0;
        private int axis2 = 3;
        private int axis3 = 1;
        private int axis4 = 2;
        private Mesh mesh = null;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (var i = 0; i < Mathf.Pow(8, 4); i++)
            {
                points4d.Add(
                    new Vector4(
                        ((i >> 0) % 8) / 7f - 0.5f,
                        ((i >> 3) % 8) / 7f - 0.5f,
                        ((i >> 6) % 8) / 7f - 0.5f,
                        ((i >> 9) % 8) / 7f - 0.5f)
                );
            }

            InitVoxelData();
        }

        void InitVoxelData()
        {
            for (var i = 0; i < Mathf.Pow(8, 4); i++)
            {
                if ((i >> 0) % 8 == 7 
                    || (i >> 3) % 8 == 7
                    || (i >> 6) % 8 == 7 
                    || (i >> 9) % 8 == 7)
                {
                    continue;
                }

                var sum = ((i >> 0) + (i >> 3) + (i >> 6) + (i >> 9));
                if (sum % 2 == 0)
                {
                    //voxelData[i] = sum % 4 == 0 ? 0.5f : 0.25f;
                }
                var voxelPos = (points4d[i] + points4d[i + 1 + 8 + 64 + 512]) * 0.5f;
                if (voxelPos.magnitude < 0.5f)
                {
                    //voxelData[i] = 0.5f - voxelPos.magnitude;
                    //keys.Add(i); keys.Add(i); keys.Add(i); keys.Add(i); 
                    //keys.Add(i); keys.Add(i); keys.Add(i); keys.Add(i);
                }
                if (voxelPos.magnitude < 0.05f)
                {
                    voxelData[i] = 0.25f;
                    keys.Add(i); keys.Add(i); keys.Add(i); keys.Add(i); 
                    keys.Add(i); keys.Add(i); keys.Add(i); keys.Add(i);
                    keys.Add(i); keys.Add(i); keys.Add(i); keys.Add(i);
                    return;
                }
            }
        }
        void UpdateVoxelData()
        {
            var key  = keys.Last();

            var newKey = key;
            var X = (key >> 0) % 8;
            var Y = (key >> 3) % 8;
            var Z = (key >> 6) % 8;
            var W = (key >> 9) % 8;

            var i = Random.Range(0, 4);
            var x = (key >> i * 3) % 8;
            if (x % 8 == 0)
            {
                newKey += (1 << i * 3);
            } else if (x % 8 == 6)
            {
                newKey -= (1 << i * 3);
            }
            else
            {
                newKey += (1 << i * 3) * (Random.value < 0.5f ? -1 : 1);
            }

            for (var j = 0; j < keys.Count; j++)
            {
                voxelData.Remove(keys[j]);
            }
            keys.Add(newKey);
            keys.RemoveAt(0);
            
            for (var j = 0; j < keys.Count; j++)
            {
                voxelData[keys[j]] = Mathf.Pow(0.8f, (keys.Count - 1) - j) * 0.25f;
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
                var dt = Time.deltaTime * 0.25f;
                time += dt * 0.25f;
                moveTime += dt * 4;
                if (moveTime > Mathf.PI * 0.1f)
                {
                    UpdateVoxelData();
                    moveTime = 0;
                }
                if (time < Mathf.PI * 2)
                {
                    Rotation4d(points4d, dt, axis1, axis2);
                    Rotation4d(points4d, dt * 2, axis3, axis4);
                }
                else
                {
                    time = 0;
                    axis1 = Random.Range(0, 4);
                    axis2 = Random.Range(0, 4);
                    if (axis1 == axis2)
                    {
                        axis2 = (axis1 + 1) % 4;
                    }

                    axis3 = Random.Range(0, 4);
                    axis4 = Random.Range(0, 4);
                    if (axis3 == axis4)
                    {
                        axis4 = (axis3 + 1) % 4;
                    }
                }
            }

            DrawEdge();
            DrawFace();
            DrawTrail();
        }

        private void ExpandDimension()
        {
            points4d.Clear();
            var sx = Mathf.Clamp01(Time.time - 0);
            var sy = Mathf.Clamp01(Time.time - 1);
            var sz = Mathf.Clamp01(Time.time - 2);
            var sw = Mathf.Clamp01(Time.time - 3);
            for (var i = 0; i < Mathf.Pow(8, 4); i++)
            {
                points4d.Add(
                    new Vector4(
                        (((i >> 0) % 8) / 7f - 0.5f) * sx,
                        (((i >> 3) % 8) / 7f - 0.5f) * sy,
                        (((i >> 6) % 8) / 7f - 0.5f) * sz,
                        (((i >> 9) % 8) / 7f - 0.5f) * sw)
                );
            }

            if (sw > 0.999f)
            {
                expanding = false;
            }
        }

        private void Rotation4d(List<Vector4> points, float angle, int ax1, int ax2)
        {
            var matR = Matrix4x4.identity;
            matR[ax1, ax1] = Mathf.Cos(angle);
            matR[ax1, ax2] = -Mathf.Sin(angle);
            matR[ax2, ax1] = Mathf.Sin(angle);
            matR[ax2, ax2] = Mathf.Cos(angle);
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = matR * points[i];
            }
        }

        private List<Vector3> ConvertTo3D(List<Vector4> points, float d)
        {
            return points.Select(p =>
            {
                var parse = 1f / (d - p.w);
                return new Vector3(p.x, p.y, p.z) * parse;
            }).ToList();
        }

        private void DrawLine(Vector3 start, Vector3 delta, Color color)
        {
            Debug.DrawLine(start, start + delta, color);
        }

        private void DrawEdge()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            var o = points3d[0];
            var dx = points3d[1 * 7] - o;
            var dy = points3d[8 * 7] - o;
            var dz = points3d[64 * 7] - o;
            var dw = points3d[512 * 7] - o;
            var col = Color.gray1;

            DrawLine(o, dx, col);
            DrawLine(o + dy * 0.5f, dx, col);
            DrawLine(o + dy, dx, col);
            DrawLine(o + dz * 0.5f, dx, col);
            DrawLine(o + dz, dx, col);
            DrawLine(o + dw * 0.5f, dx, col);
            DrawLine(o + dw, dx, col);
            DrawLine(o + dy * 0.5f + dz, dx, col);
            DrawLine(o + dy + dz * 0.5f, dx, col);
            DrawLine(o + dy + dz, dx, col);
            DrawLine(o + dz * 0.5f + dw, dx, col);
            DrawLine(o + dz + dw * 0.5f, dx, col);
            DrawLine(o + dz + dw, dx, col);
            DrawLine(o + dw * 0.5f + dy, dx, col);
            DrawLine(o + dw + dy * 0.5f, dx, col);
            DrawLine(o + dw + dy, dx, col);
            DrawLine(o + dy * 0.5f + dz + dw, dx, col);
            DrawLine(o + dy + dz * 0.5f + dw, dx, col);
            DrawLine(o + dy + dz + dw * 0.5f, dx, col);
            DrawLine(o + dy * 0.5f + dz  * 0.5f + dw, dx, col);
            DrawLine(o + dy + dz * 0.5f + dw * 0.5f, dx, col);
            DrawLine(o + dy * 0.5f + dz + dw * 0.5f, dx, col);
            DrawLine(o + dy + dz + dw, dx, col);
            
            DrawLine(o, dy, col);
            DrawLine(o + dx * 0.5f, dy, col);
            DrawLine(o + dx, dy, col);
            DrawLine(o + dz * 0.5f, dy, col);
            DrawLine(o + dz, dy, col);
            DrawLine(o + dw * 0.5f, dy, col);
            DrawLine(o + dw, dy, col);
            DrawLine(o + dx * 0.5f + dz, dy, col);
            DrawLine(o + dx + dz * 0.5f, dy, col);
            DrawLine(o + dx + dz, dy, col);
            DrawLine(o + dz * 0.5f + dw, dy, col);
            DrawLine(o + dz + dw * 0.5f, dy, col);
            DrawLine(o + dz + dw, dy, col);
            DrawLine(o + dw * 0.5f + dx, dy, col);
            DrawLine(o + dw + dx * 0.5f, dy, col);
            DrawLine(o + dw + dx, dy, col);
            DrawLine(o + dx * 0.5f + dz + dw, dy, col);
            DrawLine(o + dx + dz * 0.5f + dw, dy, col);
            DrawLine(o + dx + dz + dw * 0.5f, dy, col);
            DrawLine(o + dx * 0.5f + dz  * 0.5f + dw, dy, col);
            DrawLine(o + dx + dz * 0.5f + dw * 0.5f, dy, col);
            DrawLine(o + dx * 0.5f + dz + dw * 0.5f, dy, col);
            DrawLine(o + dx + dz + dw, dy, col);
            
            DrawLine(o, dz, col);
            DrawLine(o + dx * 0.5f, dz, col);
            DrawLine(o + dx, dz, col);
            DrawLine(o + dy * 0.5f, dz, col);
            DrawLine(o + dy, dz, col);
            DrawLine(o + dw * 0.5f, dz, col);
            DrawLine(o + dw, dz, col);
            DrawLine(o + dx * 0.5f + dy, dz, col);
            DrawLine(o + dx + dy * 0.5f, dz, col);
            DrawLine(o + dx + dy, dz, col);
            DrawLine(o + dy * 0.5f + dw, dz, col);
            DrawLine(o + dy + dw * 0.5f, dz, col);
            DrawLine(o + dy + dw, dz, col);
            DrawLine(o + dw * 0.5f + dx, dz, col);
            DrawLine(o + dw + dx * 0.5f, dz, col);
            DrawLine(o + dw + dx, dz, col);
            DrawLine(o + dx * 0.5f + dy + dw, dz, col);
            DrawLine(o + dx + dy * 0.5f + dw, dz, col);
            DrawLine(o + dx + dy + dw * 0.5f, dz, col);
            DrawLine(o + dx * 0.5f + dy  * 0.5f + dw, dz, col);
            DrawLine(o + dx + dy * 0.5f + dw * 0.5f, dz, col);
            DrawLine(o + dx * 0.5f + dy + dw * 0.5f, dz, col);
            DrawLine(o + dx + dy + dw, dz, col);
            
            DrawLine(o, dw, col);
            DrawLine(o + dx * 0.5f, dw, col);
            DrawLine(o + dx, dw, col);
            DrawLine(o + dy * 0.5f, dw, col);
            DrawLine(o + dy, dw, col);
            DrawLine(o + dz * 0.5f, dw, col);
            DrawLine(o + dz, dw, col);
            DrawLine(o + dx * 0.5f + dy, dw, col);
            DrawLine(o + dx + dy * 0.5f, dw, col);
            DrawLine(o + dx + dy, dw, col);
            DrawLine(o + dy * 0.5f + dz, dw, col);
            DrawLine(o + dy + dz * 0.5f, dw, col);
            DrawLine(o + dy + dz, dw, col);
            DrawLine(o + dz * 0.5f + dx, dw, col);
            DrawLine(o + dz + dx * 0.5f, dw, col);
            DrawLine(o + dz + dx, dw, col);
            DrawLine(o + dx * 0.5f + dy + dz, dw, col);
            DrawLine(o + dx + dy * 0.5f + dz, dw, col);
            DrawLine(o + dx + dy + dz * 0.5f, dw, col);
            DrawLine(o + dx * 0.5f + dy  * 0.5f + dz, dw, col);
            DrawLine(o + dx + dy * 0.5f + dz * 0.5f, dw, col);
            DrawLine(o + dx * 0.5f + dy + dz * 0.5f, dw, col);
            DrawLine(o + dx + dy + dz, dw, col);
            return;
            for (var i = 0; i < 8; i++)
            {
                for (var j = 0; j < 8; j++)
                {
                    for (var k = 0; k < 8; k++)
                    {
                        for (var l = 0; l < 8; l++)
                        {
                            var index = i * 512 + j * 64 + k * 8 + l;
                            if (l < 8 - 1)
                            {
                                Debug.DrawLine(points3d[index], points3d[index + 1], Color.gray1);
                            }
                            if (k < 8 - 1)
                            {
                                Debug.DrawLine(points3d[index], points3d[index + 8], Color.gray1);
                            }
                            if (j < 8 - 1)
                            {
                                Debug.DrawLine(points3d[index], points3d[index + 64], Color.gray1);
                            }
                            if (i < 8 - 1)
                            {
                                Debug.DrawLine(points3d[index], points3d[index + 512], Color.gray1);
                            }
                        }
                    }
                }
            }
        }

        private void DrawTrail()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            for (var i = 0; i < keys.Count - 1; i++)
            {
                var v = Mathf.Abs(keys[i] - keys[i + 1]);
                var col = Color.gray;
                if (v == 1)
                {
                    col = Color.red;
                }
                if (v == 8)
                {
                    col = Color.green;
                }
                if (v == 64)
                {
                    col = Color.cyan;
                }
                if (v == 512)
                {
                    col = Color.purple;
                }
                Debug.DrawLine(
                    (points3d[keys[i]] + points3d[keys[i] + 1 + 8 + 64 + 512]) * 0.5f
                    , (points3d[keys[i + 1]] + points3d[keys[i + 1] + 1 + 8 + 64 + 512]) * 0.5f, col);
            }
        }
        private void DrawFace()
        {
            var points3d = this.ConvertTo3D(points4d, 3);
            var positions = new List<Vector3>();
            var triangles = new List<int>();
            var colors = new List<Color>();
            var a = alpha;
            var col = new Color(1, 0, 0, a);

            foreach (var key in voxelData.Keys)
            {
                AddVoxel(key, voxelData[key], points3d, ref positions, ref triangles, ref colors);
            }
            
            var mesh = new Mesh();
            mesh.vertices = positions.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.colors = colors.ToArray();
            mesh.RecalculateNormals();
            Destroy(this.mesh);
            this.mesh = mesh;
            Graphics.DrawMesh(mesh, Vector3.zero, Quaternion.identity, material, 0);
        }

        private void AddVoxel(int key, float val, List<Vector3> ps, ref List<Vector3> ps2, ref List<int> ts, ref List<Color> cs)
        {
            var X = (key >> 0) % 8;
            var Y = (key >> 3) % 8;
            var Z = (key >> 6) % 8;
            var W = (key >> 9) % 8;
            if (X == 7 || Y == 7 || Z == 7 || W == 7)
            {
                return;
            }
            
            var p = new int[]
            {
                key, key + 1, key + 8, key + 1 + 8,
                key + 64, key + 1 + 64, key + 8 + 64, key + 1 + 8 + 64,
                key + 512, key + 1 + 512, key + 8 + 512, key + 1 + 8 + 512,
                key + 64 + 512, key + 1 + 64 + 512, key + 8 + 64 + 512, key + 1 + 8 + 64 + 512
            };
            var c = (ps[p[0]] + ps[p[^1]]) * 0.5f;
            for (var i = 0; i < 4; i++)
            {
                var d = i * 4;
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[1 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[3 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[2 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }
            for (var i = 0; i < 4; i++)
            {
                var d = (i % 2) * 2 + (i > 1 ? 6 : 0);
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[1 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[5 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[4 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }

            for (var i = 0; i < 4; i++)
            {
                var d = i * 2;
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[1 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[9 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[8 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }

            for (var i = 0; i < 4; i++)
            {
                var d = (i % 2) * 1 + (i > 1 ? 8 : 0);
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[2 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[6 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[4 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }
            for (var i = 0; i < 4; i++)
            {
                var d = (i % 2) * 1 + (i > 1 ? 4 : 0);
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[2 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[10 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[8 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }

            for (var i = 0; i < 4; i++)
            {
                var d = i * 1;
                var v0 = Vector3.Lerp(c, ps[p[0 + d]], val);
                var v1 = Vector3.Lerp(c, ps[p[4 + d]], val);
                var v2 = Vector3.Lerp(c, ps[p[12 + d]], val);
                var v3 = Vector3.Lerp(c, ps[p[8 + d]], val);
                AddFace(v0, v1, v2, v3, Color.white, ref ps2, ref ts, ref cs);
            }
        }

        private void AddFace(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color col, ref List<Vector3> ps,
            ref List<int> ts, ref List<Color> cs)
        {
            var index = ps.Count;
            ps.Add(v1);
            ps.Add(v2);
            ps.Add(v3);
            ps.Add(v4);
            ts.Add(index);
            ts.Add(index + 1);
            ts.Add(index + 2);
            ts.Add(index);
            ts.Add(index + 2);
            ts.Add(index + 3);
            cs.Add(col);
            cs.Add(col);
            cs.Add(col);
            cs.Add(col);
        }
    }
}