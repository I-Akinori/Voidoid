using UnityEngine;

namespace Voidoid.HyperObject
{
    public class HyperTorus : BaseHyperObject
    {
        [SerializeField, Range(0, 10)] private float rad0 = 4f;
        [SerializeField, Range(0, 10)] private float rad1 = 2f;
        [SerializeField, Range(0, 10)] private float rad2 = 1f;
        [SerializeField, Range(0, 128)] private int thetaRes = 8;
        [SerializeField, Range(0, 128)] private int phiRes = 64;
        [SerializeField, Range(0, 128)] private int psiRes = 64;

        protected override void SetPoints()
        {
            points4d.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                var theta = i * 2 * Mathf.PI / thetaRes;
                for (var j = 0; j < phiRes; j++)
                {
                    var phi = j * 2 * Mathf.PI / phiRes;
                    for (var k = 0; k < psiRes; k++)
                    {
                        var psi = k * 2 * Mathf.PI / psiRes;
                        points4d.Add(new Vector4(
                            (rad0 + rad1 * Mathf.Cos(theta) + rad2 * Mathf.Cos(phi)) * Mathf.Cos(psi),
                            (rad0 + rad1 * Mathf.Cos(theta) + rad2 * Mathf.Cos(phi)) * Mathf.Sin(psi),
                            (rad1 * Mathf.Sin(theta) + rad2 * Mathf.Sin(phi)) * Mathf.Cos(psi),
                            (rad1 * Mathf.Sin(theta) + rad2 * Mathf.Sin(phi)) * Mathf.Sin(psi)
                        ));
                    }   
                }
            }
        }

        protected override void SetEdges()
        {
            edges.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    for (var k = 0; k < psiRes; k++)
                    {
                        var index = i * (phiRes * psiRes) + j * psiRes;
                        var i0 = index + k;
                        var i1 = index + (k < psiRes - 1 ? k + 1 : 0);
                        edges.Add(new Vector2Int(i0, i1));
                        //var col = Color.HSVToRGB((float)i / thetaRes, 0.5f + 0.5f * Mathf.Cos(Mathf.PI * 2 * j / phiRes), 1);
                    }   
                }
            }
        }
    }
}