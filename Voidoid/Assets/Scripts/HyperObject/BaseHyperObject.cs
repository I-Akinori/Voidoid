using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public abstract class BaseHyperObject : MonoBehaviour
    {
        protected bool active = true;
        protected List<Vector4> points4d = new List<Vector4>();
        protected List<Vector3> points3d = new List<Vector3>();
        protected List<Color> colors = new List<Color>();
        protected List<Vector2Int> edges = new List<Vector2Int>();
        private static Material lineMaterial;
        [SerializeField] private Color color = Color.gray;
        [SerializeField] private bool useColor = false;
        [SerializeField, Range(0, 1)] protected float alpha = 1;
        [SerializeField, Range(0, 1)] protected float normalizeScaler = 1;
        [SerializeField] private Camera targetCamera;
        private int axis1 = 0;
        private int axis2 = 3;
        private int axis3 = 1;
        private int axis4 = 2;
        public bool Active { get => active; set => active = value; }
        public float Alpha { get => alpha; set => alpha = value; }
        public float Scaler { get => normalizeScaler; set => normalizeScaler = value; }
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        protected virtual void Start()
        {
            SetPoints();
            SetEdges();
            SetColors();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (!active)
            {
                return;
            }
            var dt = Time.deltaTime;
            Rotation4d(ref points4d, dt * 0.1f, axis1, axis2);
            Rotation4d(ref points4d, dt * 0.2f, axis3, axis4);
            this.points3d = this.ConvertTo3D(points4d, 3);
        }
        protected virtual void OnRenderObject()
        {
            if (!active)
            {
                return;
            }
            if (Camera.current != this.targetCamera)
            {
                return;
            }
            if (!lineMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                lineMaterial.SetInt("_ZWrite", 0);
                lineMaterial.SetInt("_ZTest", (int)UnityEngine.Rendering.CompareFunction.Always);
            }

            lineMaterial.SetPass(0);

            GL.Begin(GL.QUADS);
            if (!useColor)
            {
                var c = color;
                c.a = alpha;
                GL.Color(c);
            }
            for (var i = 0; i < this.edges.Count; i++)
            {
                var e = this.edges[i];
                if (useColor)
                {
                    var ca = this.colors[e.x];
                    var cb = this.colors[e.y];
                    ca.a *= alpha;
                    cb.a *= alpha;
                    SetThickLineToGL(
                        this.transform.TransformPoint(this.points3d[e.x]), ca, 
                        this.transform.TransformPoint(this.points3d[e.y]), cb, 0.01f);
                }
                else
                {
                    SetThickLineToGL(
                        this.transform.TransformPoint(this.points3d[e.x]), 
                        this.transform.TransformPoint(this.points3d[e.y]), 0.01f);
                }
            }

            GL.End();
        }
        private void SetThickLineToGL(Vector3 a, Vector3 b, float width)
        {
            var dir = (b - a).normalized;

            // カメラに対して垂直な方向を作る
            var normal = Vector3.Cross(dir, Camera.main.transform.forward).normalized;
            var offset = normal * (width * 0.5f);
            var v0 = a - offset;
            var v1 = a + offset;
            var v2 = b + offset;
            var v3 = b - offset;
            GL.Vertex(v0);
            GL.Vertex(v1);
            GL.Vertex(v2);
            GL.Vertex(v3);
        }
        private void SetThickLineToGL(Vector3 a, Color ca, Vector3 b, Color cb, float width)
        {
            var dir = (b - a).normalized;

            // カメラに対して垂直な方向を作る
            var normal = Vector3.Cross(dir, Camera.main.transform.forward).normalized;
            var offset = normal * (width * 0.5f);
            var v0 = a - offset;
            var v1 = a + offset;
            var v2 = b + offset;
            var v3 = b - offset;
            GL.Color(ca);
            GL.Vertex(v0);
            GL.Color(ca);
            GL.Vertex(v1);
            GL.Color(cb);
            GL.Vertex(v2);
            GL.Color(cb);
            GL.Vertex(v3);
        }

        protected abstract void SetPoints();

        protected abstract void SetEdges();

        protected virtual void SetColors()
        {
            colors.Clear();
            foreach (var p in points4d)
            {
                var u = Mathf.Atan2(p.y, p.x);
                var v = Mathf.Atan2(p.w, p.z);
                var hue = (u / (2 * Mathf.PI) + 1f) % 1f;
                var value = (v / (2 * Mathf.PI) + 1f) * 0.5f;
                var c = Color.HSVToRGB(hue, 1f, value);
                c.a = alpha;
                colors.Add(c);
            }
        }

        protected void Rotation4d(ref List<Vector4> points, float angle, int ax1, int ax2)
        {
            var matR = Matrix4x4.identity;
            matR[ax1, ax1] = Mathf.Cos(angle);
            matR[ax1, ax2] = - Mathf.Sin(angle);
            matR[ax2, ax1] = Mathf.Sin(angle);
            matR[ax2, ax2] = Mathf.Cos(angle);
            for (var i = 0; i < points.Count; i++)
            {
                points[i] = matR * points[i];
            }
        }
        protected List<Vector3> ConvertTo3D(List<Vector4> points, float d)
        {
            var reduced = new List<Vector3>();
            points.ForEach(p =>
            {
                var parse = 1f / (d - p.w);
                reduced.Add(new Vector3(p.x, p.y, p.z) * (parse * normalizeScaler));
            });
            return reduced;
        }
    }
}