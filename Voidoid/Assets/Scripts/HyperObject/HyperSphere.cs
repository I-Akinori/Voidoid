using UnityEngine;

namespace Voidoid.HyperObject
{
    public class HyperSphere : BaseHyperObject
    {
        [SerializeField, Range(0, 128)] private int thetaRes = 8;
        [SerializeField, Range(0, 128)] private int phiRes = 64;
        [SerializeField, Range(0, 128)] private int psiRes = 64;
        [SerializeField, Range(0, 10)] private float scale = 1f;
        
        protected override void SetPoints()
        {
            points4d.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                var theta = i * Mathf.PI / thetaRes;
                for (var j = 0; j < phiRes; j++)
                {
                    var phi = j * 2 * Mathf.PI / phiRes;
                    for (var k = 0; k < psiRes; k++)
                    {
                        var psi = k * 4 * Mathf.PI / psiRes;
                        var cosT = Mathf.Cos(theta * 0.5f);
                        var sinT = Mathf.Sin(theta * 0.5f);
                        points4d.Add(new Vector4(
                            cosT * Mathf.Cos((phi + psi) * 0.5f),
                            cosT * Mathf.Sin((phi + psi) * 0.5f),
                            sinT * Mathf.Cos((phi - psi) * 0.5f),
                            sinT * Mathf.Sin((phi - psi) * 0.5f)
                        ) * scale);
                    }   
                }
            }
        }

        protected override void SetEdges()
        {
            edges.Clear();
            //colors.Clear();
            for (var i = 0; i < thetaRes; i++)
            {
                for (var j = 0; j < phiRes; j++)
                {
                    for (var k = 0; k < psiRes; k++)
                    {
                        var index = i * (phiRes * psiRes) + j * psiRes;
                        var i0 = index + k;
                        var i1 = index + (k < psiRes - 1 ? k + 1 : 0);
                        //var col = Color.HSVToRGB((float)i / thetaRes, (float)j / phiRes, 1);
                        
                        edges.Add(new Vector2Int(i0, i1));
                        //colors.Add(col);
                    }   
                }
            }
        }
    }
}