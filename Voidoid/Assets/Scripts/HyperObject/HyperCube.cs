using UnityEngine;

namespace Voidoid.HyperObject
{
    public class HyperCube : BaseHyperObject
    {
        protected override void SetPoints()
        {
            points4d.Clear();
            for (var i = 0; i < 16; i++)
            {
                points4d.Add(
                    new Vector4(
                        ((i >> 0) & 1) == 0 ? -1 : 1, 
                        ((i >> 1) & 1) == 0 ? -1 : 1, 
                        ((i >> 2) & 1) == 0 ? -1 : 1, 
                        ((i >> 3) & 1) == 0 ? -1 : 1)
                    );
            }
        }

        protected override void SetEdges()
        {
            edges.Clear();
            for (var i = 0; i < 16; i++)
            {
                if (i % 2 < 1)
                {
                    edges.Add(new Vector2Int(i, i + 1));
                }
                if (i % 4 < 2)
                {
                    edges.Add(new Vector2Int(i, i + 2));
                }
                if (i % 8 < 4)
                {
                    edges.Add(new Vector2Int(i, i + 4));
                }
                if (i % 16 < 8)
                {
                    edges.Add(new Vector2Int(i, i + 8));
                }
            }
        }
    }
}