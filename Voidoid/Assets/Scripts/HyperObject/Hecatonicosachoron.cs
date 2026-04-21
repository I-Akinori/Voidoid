using System.Collections.Generic;
using UnityEngine;

namespace Voidoid.HyperObject
{
    public class Hecatonicosachoron : BaseHyperObject
    {
        protected override void SetPoints()
        {
            points4d = new List<Vector4>();
            var sqr5 = Mathf.Sqrt(5);
            var phi = (1 + sqr5) * 0.5f;
            var phi2 = phi * phi;
            var phiInv = 1.0f / phi;
            var phiInv2 = 1.0f / phi2;

            // normalize
            // A: 24 points
            Shuffle24(ref points4d, 2f);
            // B: 64 points
            Shuffle64(ref points4d, phi, phiInv2);
            // C: 64 points
            Shuffle64(ref points4d, 1f, sqr5);
            // D: 64 points
            Shuffle64(ref points4d, phiInv, phi2);
            // E: 96 points
            Shuffle96(ref points4d, phiInv, phi, sqr5);
            // F: 96 points
            Shuffle96(ref points4d, phiInv2, 1f, phi2);
            // G: 192 points
            Shuffle192(ref points4d, phiInv, 1f, phi, 2f);
        }
        
        protected override void SetEdges()
        {
            this.edges.Clear();
            var eps = 1e-2f;
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
                    var d2 = (points4d[i] - points4d[j]).sqrMagnitude;
                    if (Mathf.Abs(d2 - minDist2) < 0.1f)
                    {
                        edges.Add(new Vector2Int(i, j));
                    }
                }
            }
        }

        private void Shuffle24(ref List<Vector4> points, float a)
        {
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, 0, a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, a * (i < 2 ? 1 : -1), 0,a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    0, a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1), 0));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), 0, 0,a * (i % 2 == 0 ? 1 : -1)));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), 0, a * (i % 2 == 0 ? 1 : -1), 0));
            }
            for (var i = 0; i < 4; i++)
            {
                points.Add(new Vector4(
                    a * (i < 2 ? 1 : -1), a * (i % 2 == 0 ? 1 : -1), 0, 0));
            }
        }

        private void Shuffle64(ref List<Vector4> points, float a, float b)
        {
            for (var k = 0; k < 4; k++)
            {
                for (var i = 0; i < 16; i++)
                {
                    points.Add(new Vector4(
                        (((i >> 0) & 1) == 0 ? -1 : 1) * (k == 0 ? b : a), 
                        (((i >> 1) & 1) == 0 ? -1 : 1) * (k == 1 ? b : a), 
                        (((i >> 2) & 1) == 0 ? -1 : 1) * (k == 2 ? b : a), 
                        (((i >> 3) & 1) == 0 ? -1 : 1) * (k == 3 ? b : a)
                    ));
                }
            }
        }
        private void Shuffle96(ref List<Vector4> points, float a, float b, float c)
        {
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                    ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0, 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0, 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    0, 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
            for (var i = 0; i < 8; i++)
            {
                points.Add(new Vector4( 
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    0
                ));
            }
        }

        private void Shuffle192(ref List<Vector4> points, float a, float b, float c, float d)
        {
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    b * (((i >> 1) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    c * (((i >> 2) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1), 
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1) 
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1)
                ));
            }
            for (var i = 0; i < 16; i++)
            {
                points.Add(new Vector4(
                    d * (((i >> 3) & 1) == 0 ? -1 : 1),
                    c * (((i >> 2) & 1) == 0 ? -1 : 1),
                    b * (((i >> 1) & 1) == 0 ? -1 : 1),
                    a * (((i >> 0) & 1) == 0 ? -1 : 1)
                ));
            }
        }

    }
}