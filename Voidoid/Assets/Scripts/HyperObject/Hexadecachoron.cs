using UnityEngine;

namespace Voidoid.HyperObject
{
    public class Hexadecachoron : BaseHyperObject
    {
        protected override void SetPoints()
        {
            points4d.Clear();
            points4d.Add(new Vector4(1, 0, 0,0));
            points4d.Add(new Vector4(-1, 0, 0, 0));
            points4d.Add(new Vector4(0, 1, 0, 0));
            points4d.Add(new Vector4(0, -1, 0, 0));
            points4d.Add(new Vector4(0, 0, 1, 0));
            points4d.Add(new Vector4(0, 0, -1, 0));
            points4d.Add(new Vector4(0, 0, 0, 1));
            points4d.Add(new Vector4(0, 0, 0, -1));
        }

        protected override void SetEdges()
        {
            edges.Clear();
            edges.Add(new Vector2Int(0, 2));
            edges.Add(new Vector2Int(0, 3));
            edges.Add(new Vector2Int(1, 2));
            edges.Add(new Vector2Int(1, 3));
            
            edges.Add(new Vector2Int(0, 4));
            edges.Add(new Vector2Int(0, 5));
            edges.Add(new Vector2Int(1, 4));
            edges.Add(new Vector2Int(1, 5));
            
            edges.Add(new Vector2Int(0, 6));
            edges.Add(new Vector2Int(0, 7));
            edges.Add(new Vector2Int(1, 6));
            edges.Add(new Vector2Int(1, 7));
            
            edges.Add(new Vector2Int(2, 4));
            edges.Add(new Vector2Int(2, 5));
            edges.Add(new Vector2Int(3, 4));
            edges.Add(new Vector2Int(3, 5));
            
            edges.Add(new Vector2Int(2, 6));
            edges.Add(new Vector2Int(2, 7));
            edges.Add(new Vector2Int(3, 6));
            edges.Add(new Vector2Int(3, 7));
            
            edges.Add(new Vector2Int(4, 6));
            edges.Add(new Vector2Int(4, 7));
            edges.Add(new Vector2Int(5, 6));
            edges.Add(new Vector2Int(5, 7));
        }
    }
}