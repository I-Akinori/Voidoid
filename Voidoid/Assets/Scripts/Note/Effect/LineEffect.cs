using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.Note.Effect
{
    [RequireComponent(typeof(LineRenderer))]
    public class LineEffect : MonoBehaviour
    {
        public float LifeTime => lifeTime;
        [SerializeField] private float lifeTime = 5;
        [SerializeField] private Color color = Color.red;
        
        private List<Vector3> positions = new List<Vector3>();
        private List<GameObject> markers = new List<GameObject>();
        private LineRenderer lineRenderer;
        

        private float time;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            time = 0;
            InitLineRenderer();
        }

        public void SetColor(Color col)
        {
            this.color = col;
            this.lineRenderer.material.color = this.color;
        }

        public void AddPoint(Vector3 point)
        {
            time = 0;　// 節点追加時はタイマーリセット (延命)
            this.positions.Add(point);
            UpdateLineRenderer();
        }

        // 今の位置にマーカーを置く
        public void SetMarker()
        {
            if (this.positions.Count <= 0)
            {
                return;
            }
            var pos = this.positions[^1];
            var obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.localPosition = pos;
            obj.transform.SetParent(this.transform, false);
            obj.transform.localScale = Vector3.one * 0.25f;   
            obj.GetComponent<Renderer>().material = lineRenderer.material;
            obj.GetComponent<Renderer>().material.color = color;
            obj.layer = LayerMask.NameToLayer("ProjectorView");
            markers.Add(obj);
        }
        public void SetLifeTime(float lifeT)
        {
            this.lifeTime = lifeT;
        }

        public void Dispose()
        {
            Destroy(gameObject);
            foreach (var marker in markers)
            {
                Destroy(marker);
            }

            markers.Clear();
        }

        public bool Step(float t)
        {
            time += t;
            lineRenderer.endColor = new Color(this.color.r, this.color.g, this.color.b, color.a * (lifeTime - time) / lifeTime);
            for (var i = 0; i < markers.Count; i++)
            {
                var marker = markers[i];
                marker.GetComponent<Renderer>().material.color 
                    = new Color(this.color.r, this.color.g, this.color.b, color.a * (lifeTime - time) / lifeTime * ((float)i / markers.Count));
            }
            // 寿命が来たかどうかを返す
            return time >= lifeTime;
        }
        void InitLineRenderer()
        {
            this.lineRenderer = this.gameObject.GetComponent<LineRenderer>();
            this.lineRenderer.material.color = this.color;
            this.lineRenderer.useWorldSpace = false;
        }
        void UpdateLineRenderer()
        {
            lineRenderer.endWidth = 0.05f;
            lineRenderer.startWidth = 0.0f;
            lineRenderer.endColor = this.color;
            lineRenderer.startColor = new Color(this.color.r, this.color.g, this.color.b, 0.0f);
            lineRenderer.positionCount = this.positions.Count;
            lineRenderer.SetPositions(this.positions.ToArray());
        }
    }
}