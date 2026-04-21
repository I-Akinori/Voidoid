using UnityEngine;

namespace Voidoid.HyperObject
{
    public class Icositetrachoron : BaseHyperObject
    {
        protected override void SetPoints()
        {
            points4d.Clear();

            points4d.Add(new Vector4(0, 0, 1, 1));
            points4d.Add(new Vector4(0, 0, 1, -1));
            points4d.Add(new Vector4(0, 0, -1, 1));
            points4d.Add(new Vector4(0, 0, -1, -1));
            
            points4d.Add(new Vector4(0, 1, 0, 1));
            points4d.Add(new Vector4(0, 1, 0, -1));
            points4d.Add(new Vector4(0, -1, 0, 1));
            points4d.Add(new Vector4(0, -1, 0, -1));
            
            points4d.Add(new Vector4(0, 1, 1, 0));
            points4d.Add(new Vector4(0, 1, -1, 0));
            points4d.Add(new Vector4(0, -1, 1, 0));
            points4d.Add(new Vector4(0, -1, -1, 0));
            
            points4d.Add(new Vector4(1, 0, 0,1));
            points4d.Add(new Vector4(1, 0, 0,-1));
            points4d.Add(new Vector4(-1, 0, 0, 1));
            points4d.Add(new Vector4(-1, 0, 0, -1));
            
            points4d.Add(new Vector4(1, 0, 1, 0));
            points4d.Add(new Vector4(1, 0, -1, 0));
            points4d.Add(new Vector4(-1, 0, 1, 0));
            points4d.Add(new Vector4(-1, 0,  -1, 0));
            
            points4d.Add(new Vector4(1, 1, 0, 0));
            points4d.Add(new Vector4(1, -1, 0, 0));
            points4d.Add(new Vector4(-1, 1, 0, 0));
            points4d.Add(new Vector4(-1, -1, 0, 0));
        }

        protected override void SetEdges()
        {
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(4 + i / 2, 12 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(8 + i / 2, 16 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2));
            }

            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(0 + i / 2, 12 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(8 + i / 2, 20 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2));
            }

            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(0 + i / 2, 16 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(4 + i / 2, 20 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2));
            }

            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(0 + i / 2, 4 + (i % 4 < 2 ? 0 : 1) + (i % 2) * 2));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(16 + i / 2, 20 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1));
            }
            
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(0 + i / 2, 8 + (i % 8 < 4 ? 0 : 1) + (i % 2) * 2));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(12 + i / 2, 20 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1));
            }
            
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(4 + i / 2, 8 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1));
            }
            for (var i = 0; i < 8; i++)
            {
                edges.Add(new Vector2Int(12 + i / 2, 16 + (i % 8 < 4 ? 0 : 2) + (i % 2) * 1));
            }
        }
    }
}