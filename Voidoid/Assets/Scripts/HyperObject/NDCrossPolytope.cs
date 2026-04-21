using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class NDCrossPolytope : MonoBehaviour
    {
        [SerializeField] private int dimensions = 6;
        [SerializeField] private List<float[]> pointsNd  = new List<float[]>();
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private bool expanding = true;
        private int axis1 = 0;
        private int axis2 = 3;
        private int axis3 = 1;
        private int axis4 = 2;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            pointsNd = new List<float[]>();
            for (var i = 0; i < 2 * dimensions; i++)
            {
                pointsNd.Add(new float[dimensions]);
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
                if (time < Mathf.PI * 2)
                {
                    RotationNd(ref pointsNd, dt, axis1, axis2);
                    RotationNd(ref pointsNd, dt * 2, axis3, axis4);
                }
                else
                {
                    time = 0;
                    axis1 = Random.Range(0, dimensions);
                    axis2 = Random.Range(0, dimensions);
                    if (axis1 == axis2)
                    {
                        axis2 = (axis1 + 1) % dimensions;
                    }
                    axis3 = Random.Range(0, dimensions);
                    axis4 = Random.Range(0, dimensions);
                    if (axis3 == axis4)
                    {
                        axis4 = (axis3 + 1) % dimensions;
                    }
                }
            }
            DrawEdge();
            //DrawFace();
        }

        private void ExpandDimension()
        {
            var scales = new float[dimensions];
            for (var i = 0; i < 2 * dimensions; i++)
            {
                var pos = new float[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    scales[j] = Mathf.Clamp01(Time.time - (i / 2));
                    pos[j] = j == (i / 2) ? 1 : 0;
                    pos[j] *= i % 2 == 0 ? 1 : -1;
                    pos[j] *= scales[j];
                }
                pointsNd[i] = pos;
            }

            if (scales[dimensions - 1] > 0.999f)
            {
                expanding = false;
            }
        }

        private void RotationNd(ref List<float[]> points, float angle, int ax1, int ax2)
        {
            var c = Mathf.Cos(angle);
            var s = Mathf.Sin(angle);
            for (var i = 0; i < points.Count; i++)
            {
                var point = points[i];
                ax1 = Mathf.Min(ax1, point.Length - 1);
                ax2 = Mathf.Min(ax2, point.Length - 1);
                var e1 = point[ax1];
                var e2 = point[ax2];
                point[ax1] = e1 * c - e2 * s;
                point[ax2] = e1 * s + e2 * c;
                points[i] = point;
            }
        }
        private List<float[]> ConvertTo3d(List<float[]> points, float d, int dim)
        {
            if (dim < 4)
            {
                return points;
            }
            var reduced = new List<float[]>();
            points.ForEach(p =>
            {
                var parse = 1f / (d - p[^1]);
                var pos = new float[dim - 1];
                for (var j = 0; j < dim - 1; j++)
                {
                    pos[j] = p[j] * parse;
                }
                reduced.Add(pos);
            });
            return ConvertTo3d(reduced, d, dim - 1);
        }


        private void DrawEdge()
        {
            var points3d = this.ConvertTo3d(pointsNd, 3, dimensions);
            var groups = dimensions * (dimensions - 1) / 2;
            var count = 0;
            for (var i = 0; i < dimensions; i++)
            {
                for (var j = i + 1; j < dimensions; j++)
                {
                    var col = Color.HSVToRGB(count++ / (float)groups, 1, 1);
                    for (var k = 0; k < 4; k++)
                    {
                        var sid = i * 2 + k % 2;
                        var eid = j * 2 + k / 2;
                        var s3d = new Vector3(points3d[sid][0], points3d[sid][1], points3d[sid][2]);
                        var e3d = new Vector3(points3d[eid][0], points3d[eid][1], points3d[eid][2]);
                        Debug.DrawLine(s3d, e3d, col);
                    }
                }
            }
        }
    }
}