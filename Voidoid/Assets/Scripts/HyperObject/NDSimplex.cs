using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class NDSimplex : MonoBehaviour
    {
        [SerializeField] private int dimensions = 6;
        [SerializeField] private List<float[]> pointsNd = new List<float[]>();
        [SerializeField] private Material material;
        [SerializeField, Range(0, 1)] private float alpha;
        private float time = 0f;
        private bool expanding = true;
        private int axis1 = 0;
        private int axis2 = 2;
        private int axis3 = 1;
        private int axis4 = 2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            pointsNd = new List<float[]>();
            for (var i = 0; i < dimensions + 1; i++)
            {
                pointsNd.Add(new float[dimensions]);
            }

            var a = (-1 + (dimensions - 1) * Mathf.Sqrt(dimensions + 1)) / dimensions;
            var b = (-1 - Mathf.Sqrt(dimensions + 1)) / dimensions;

            for (var i = 0; i < dimensions; i++)
            {
                var point = pointsNd[i];
                for (var j = 0; j < dimensions; j++)
                {
                    var pj = (i == j) ? a : b;
                    point[j] = pj;
                }
                pointsNd[i] = point;
            }

            for (var j = 0; j < dimensions; j++)
            {
                pointsNd[dimensions][j] = 1f;
            }
            for (var i = 0; i < dimensions + 1; i++)
            {
                //Debug.Log(pointsNd[i][0] + " " + pointsNd[i][1] + " " + pointsNd[i][2] + " " + pointsNd[i][3]);
                Debug.Log(pointsNd[i][0] + " " + pointsNd[i][1] + " " + pointsNd[i][2]);
            }
        }

        // Update is called once per frame
        void Update()
        {
            var dt = Time.deltaTime;
            time += dt;
            if (time < Mathf.PI * 2)
            {
                RotationNd(ref pointsNd, dt, axis1, axis2);
            }
            else
            {
                time = 0;
                //axis1 = Random.Range(0, dimensions - 1);
                axis1 = 3;
                axis2 = Random.Range(0, dimensions - 1);
                if (axis1 == axis2)
                {
                    axis2 = (axis1 + 1) % dimensions;
                }
            }
            DrawEdge();
            //DrawFace();
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
            for (var i = 0; i < dimensions + 1; i++)
            {
                var vec3d = new Vector3(points3d[i][0], points3d[i][1], points3d[i][2]);
                var col = Color.gray;
                for (var j = i + 1; j < dimensions + 1; j++)
                {
                    var end = new Vector3(points3d[j][0], points3d[j][1], points3d[j][2]);
                    Debug.DrawLine(vec3d, end, col);
                }
            }
        }
    }
}