using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class Hexacosichoron : BaseHyperObject
    {
        protected override void SetPoints()
        {
            points4d = new List<Vector4>();
            // A: 16 points
            for (var i = 0; i < 16; i++)
            {
                points4d.Add(
                    new Vector4(
                        (((i >> 0) & 1) == 0 ? -1 : 1),
                        (((i >> 1) & 1) == 0 ? -1 : 1),
                        (((i >> 2) & 1) == 0 ? -1 : 1),
                        (((i >> 3) & 1) == 0 ? -1 : 1)) * 0.5f);
            }

            // B: 8 points
            for (var i = 0; i < 8; i++)
            {
                points4d.Add(
                    new Vector4(
                        i / 2 == 3 ? (i % 2 == 0 ? 1 : -1) : 0,
                        i / 2 == 2 ? (i % 2 == 0 ? 1 : -1) : 0,
                        i / 2 == 1 ? (i % 2 == 0 ? 1 : -1) : 0,
                        i / 2 == 0 ? (i % 2 == 0 ? 1 : -1) : 0
                    )
                );
            }

            // C: 96 points
            var phi = (1 + Mathf.Sqrt(5)) * 0.5f;
            var vals = new float[]
            {
                0f, 0.5f, 0.5f * phi, 0.5f / phi
            };
            points4d.Add(new Vector4(vals[0], vals[1], vals[2], vals[3]));
            points4d.Add(new Vector4(vals[0], vals[1], vals[2], -vals[3]));
            points4d.Add(new Vector4(vals[0], vals[1], -vals[2], vals[3]));
            points4d.Add(new Vector4(vals[0], vals[1], -vals[2], -vals[3]));
            points4d.Add(new Vector4(vals[0], -vals[1], vals[2], vals[3]));
            points4d.Add(new Vector4(vals[0], -vals[1], vals[2], -vals[3]));
            points4d.Add(new Vector4(vals[0], -vals[1], -vals[2], vals[3]));
            points4d.Add(new Vector4(vals[0], -vals[1], -vals[2], -vals[3]));

            points4d.Add(new Vector4(vals[0], vals[2], vals[3], vals[1]));
            points4d.Add(new Vector4(vals[0], vals[2], vals[3], -vals[1]));
            points4d.Add(new Vector4(vals[0], vals[2], -vals[3], vals[1]));
            points4d.Add(new Vector4(vals[0], vals[2], -vals[3], -vals[1]));
            points4d.Add(new Vector4(vals[0], -vals[2], vals[3], vals[1]));
            points4d.Add(new Vector4(vals[0], -vals[2], vals[3], -vals[1]));
            points4d.Add(new Vector4(vals[0], -vals[2], -vals[3], vals[1]));
            points4d.Add(new Vector4(vals[0], -vals[2], -vals[3], -vals[1]));

            points4d.Add(new Vector4(vals[0], vals[3], vals[1], vals[2]));
            points4d.Add(new Vector4(vals[0], vals[3], vals[1], -vals[2]));
            points4d.Add(new Vector4(vals[0], vals[3], -vals[1], vals[2]));
            points4d.Add(new Vector4(vals[0], vals[3], -vals[1], -vals[2]));
            points4d.Add(new Vector4(vals[0], -vals[3], vals[1], vals[2]));
            points4d.Add(new Vector4(vals[0], -vals[3], vals[1], -vals[2]));
            points4d.Add(new Vector4(vals[0], -vals[3], -vals[1], vals[2]));
            points4d.Add(new Vector4(vals[0], -vals[3], -vals[1], -vals[2]));

            points4d.Add(new Vector4(vals[1], vals[0], vals[3], vals[2]));
            points4d.Add(new Vector4(vals[1], vals[0], vals[3], -vals[2]));
            points4d.Add(new Vector4(vals[1], vals[0], -vals[3], vals[2]));
            points4d.Add(new Vector4(vals[1], vals[0], -vals[3], -vals[2]));
            points4d.Add(new Vector4(-vals[1], vals[0], vals[3], vals[2]));
            points4d.Add(new Vector4(-vals[1], vals[0], vals[3], -vals[2]));
            points4d.Add(new Vector4(-vals[1], vals[0], -vals[3], vals[2]));
            points4d.Add(new Vector4(-vals[1], vals[0], -vals[3], -vals[2]));

            points4d.Add(new Vector4(vals[2], vals[0], vals[1], vals[3]));
            points4d.Add(new Vector4(vals[2], vals[0], vals[1], -vals[3]));
            points4d.Add(new Vector4(vals[2], vals[0], -vals[1], vals[3]));
            points4d.Add(new Vector4(vals[2], vals[0], -vals[1], -vals[3]));
            points4d.Add(new Vector4(-vals[2], vals[0], vals[1], vals[3]));
            points4d.Add(new Vector4(-vals[2], vals[0], vals[1], -vals[3]));
            points4d.Add(new Vector4(-vals[2], vals[0], -vals[1], vals[3]));
            points4d.Add(new Vector4(-vals[2], vals[0], -vals[1], -vals[3]));

            points4d.Add(new Vector4(vals[3], vals[0], vals[2], vals[1]));
            points4d.Add(new Vector4(vals[3], vals[0], vals[2], -vals[1]));
            points4d.Add(new Vector4(vals[3], vals[0], -vals[2], vals[1]));
            points4d.Add(new Vector4(vals[3], vals[0], -vals[2], -vals[1]));
            points4d.Add(new Vector4(-vals[3], vals[0], vals[2], vals[1]));
            points4d.Add(new Vector4(-vals[3], vals[0], vals[2], -vals[1]));
            points4d.Add(new Vector4(-vals[3], vals[0], -vals[2], vals[1]));
            points4d.Add(new Vector4(-vals[3], vals[0], -vals[2], -vals[1]));

            points4d.Add(new Vector4(vals[1], vals[2], vals[0], vals[3]));
            points4d.Add(new Vector4(vals[1], vals[2], vals[0], -vals[3]));
            points4d.Add(new Vector4(vals[1], -vals[2], vals[0], vals[3]));
            points4d.Add(new Vector4(vals[1], -vals[2], vals[0], -vals[3]));
            points4d.Add(new Vector4(-vals[1], vals[2], vals[0], vals[3]));
            points4d.Add(new Vector4(-vals[1], vals[2], vals[0], -vals[3]));
            points4d.Add(new Vector4(-vals[1], -vals[2], vals[0], vals[3]));
            points4d.Add(new Vector4(-vals[1], -vals[2], vals[0], -vals[3]));

            points4d.Add(new Vector4(vals[2], vals[3], vals[0], vals[1]));
            points4d.Add(new Vector4(vals[2], vals[3], vals[0], -vals[1]));
            points4d.Add(new Vector4(vals[2], -vals[3], vals[0], vals[1]));
            points4d.Add(new Vector4(vals[2], -vals[3], vals[0], -vals[1]));
            points4d.Add(new Vector4(-vals[2], vals[3], vals[0], vals[1]));
            points4d.Add(new Vector4(-vals[2], vals[3], vals[0], -vals[1]));
            points4d.Add(new Vector4(-vals[2], -vals[3], vals[0], vals[1]));
            points4d.Add(new Vector4(-vals[2], -vals[3], vals[0], -vals[1]));

            points4d.Add(new Vector4(vals[3], vals[1], vals[0], vals[2]));
            points4d.Add(new Vector4(vals[3], vals[1], vals[0], -vals[2]));
            points4d.Add(new Vector4(vals[3], -vals[1], vals[0], vals[2]));
            points4d.Add(new Vector4(vals[3], -vals[1], vals[0], -vals[2]));
            points4d.Add(new Vector4(-vals[3], vals[1], vals[0], vals[2]));
            points4d.Add(new Vector4(-vals[3], vals[1], vals[0], -vals[2]));
            points4d.Add(new Vector4(-vals[3], -vals[1], vals[0], vals[2]));
            points4d.Add(new Vector4(-vals[3], -vals[1], vals[0], -vals[2]));

            points4d.Add(new Vector4(vals[3], vals[2], vals[1], vals[0]));
            points4d.Add(new Vector4(vals[3], vals[2], -vals[1], vals[0]));
            points4d.Add(new Vector4(vals[3], -vals[2], vals[1], vals[0]));
            points4d.Add(new Vector4(vals[3], -vals[2], -vals[1], vals[0]));
            points4d.Add(new Vector4(-vals[3], vals[2], vals[1], vals[0]));
            points4d.Add(new Vector4(-vals[3], vals[2], -vals[1], vals[0]));
            points4d.Add(new Vector4(-vals[3], -vals[2], vals[1], vals[0]));
            points4d.Add(new Vector4(-vals[3], -vals[2], -vals[1], vals[0]));

            points4d.Add(new Vector4(vals[2], vals[1], vals[3], vals[0]));
            points4d.Add(new Vector4(vals[2], vals[1], -vals[3], vals[0]));
            points4d.Add(new Vector4(vals[2], -vals[1], vals[3], vals[0]));
            points4d.Add(new Vector4(vals[2], -vals[1], -vals[3], vals[0]));
            points4d.Add(new Vector4(-vals[2], vals[1], vals[3], vals[0]));
            points4d.Add(new Vector4(-vals[2], vals[1], -vals[3], vals[0]));
            points4d.Add(new Vector4(-vals[2], -vals[1], vals[3], vals[0]));
            points4d.Add(new Vector4(-vals[2], -vals[1], -vals[3], vals[0]));

            points4d.Add(new Vector4(vals[1], vals[3], vals[2], vals[0]));
            points4d.Add(new Vector4(vals[1], vals[3], -vals[2], vals[0]));
            points4d.Add(new Vector4(vals[1], -vals[3], vals[2], vals[0]));
            points4d.Add(new Vector4(vals[1], -vals[3], -vals[2], vals[0]));
            points4d.Add(new Vector4(-vals[1], vals[3], vals[2], vals[0]));
            points4d.Add(new Vector4(-vals[1], vals[3], -vals[2], vals[0]));
            points4d.Add(new Vector4(-vals[1], -vals[3], vals[2], vals[0]));
            points4d.Add(new Vector4(-vals[1], -vals[3], -vals[2], vals[0]));
        }

        protected override void SetEdges()
        {
            edges.Clear();
            var minDist2 = float.MaxValue;
            for (var i = 0; i < points4d.Count; i++)
            {
                for (var j = i + 1; j < points4d.Count; j++)
                {
                    float d2 = (points4d[i] - points4d[j]).sqrMagnitude;
                    if (d2 < minDist2)
                    {
                        minDist2 = d2;
                    }
                }
            }
            
            for (var i = 0; i < points4d.Count; i++)
            {
                for (var j = i + 1; j < points4d.Count; j++)
                {
                    float d2 = (points4d[i] - points4d[j]).sqrMagnitude;
                    if (Mathf.Abs(d2 - minDist2) < 0.1f)
                    {
                        edges.Add(new Vector2Int(i, j));
                    }
                }
            }
        }
    }
}