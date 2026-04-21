using System;
using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class IndicatorEffector : BaseEffector
    {
        [SerializeField] private Material srcMat;
        [SerializeField] private List<GameObject> obj;
        [SerializeField] private AnimationCurve curve;
        private Vector3 origScale;
        private List<Material> materials = new List<Material>();
        private List<Vector2Int> list = new List<Vector2Int>()
        {
            new Vector2Int(0, 1),
            new Vector2Int(2, 3),
            new Vector2Int(4, 5),
            new Vector2Int(6, 7),
            new Vector2Int(0, 1),
            new Vector2Int(6, 7),
            new Vector2Int(4, 5),
            new Vector2Int(2, 3),
            
            new Vector2Int(0, 2),
            new Vector2Int(6, 4),
            new Vector2Int(7, 5),
            new Vector2Int(1, 3),
            new Vector2Int(0, 2),
            new Vector2Int(1, 3),
            new Vector2Int(7, 5),
            new Vector2Int(6, 4),
            
            new Vector2Int(0, 6),
            new Vector2Int(2, 4),
            new Vector2Int(3, 5),
            new Vector2Int(1, 7),
            new Vector2Int(0, 6),
            new Vector2Int(2, 4),
            new Vector2Int(3, 5),
            new Vector2Int(1, 7),
            
            new Vector2Int(3, 7),
            new Vector2Int(2, 6),
            new Vector2Int(1, 5),
            new Vector2Int(0, 4),
            new Vector2Int(3, 7),
            new Vector2Int(2, 6),
            new Vector2Int(1, 5),
            new Vector2Int(0, 4),
        };

        private void Start()
        {
            foreach (var obj in obj)
            {
                var mat = new Material(srcMat);
                obj.GetComponent<Renderer>().material = mat; 
                materials.Add(mat);
                // 全て同じサイズであることを前提に
                origScale = obj.transform.localScale;
            }
        }

        private void Dispose()
        {
            foreach (var mat in materials)
            {
                DestroyImmediate(mat);
            }
        }

        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            var idX = list[index].x;
            var idY = list[index].y;
            materials[idX].SetColor("_Color", setting.color);
            materials[idY].SetColor("_Color", setting.color);
            obj[idX].transform.localScale = origScale;
            obj[idY].transform.localScale = origScale;
        }

        public override void ContinuePlay(PlayerSettings setting, Note note, int index, float time, Note next = null)
        {
            var idX = list[index].x;
            var idY = list[index].y;
            materials[idX].SetColor("_Color", Color.Lerp(setting.color, Color.white, time));
            materials[idY].SetColor("_Color", Color.Lerp(setting.color, Color.white, time));
            obj[idX].transform.localScale = origScale * (1f + 0.5f * curve.Evaluate(time));
            obj[idY].transform.localScale = origScale * (1f + 0.5f * curve.Evaluate(time));
        }

        public override void EndPlay(int index)
        {
            var idX = list[index].x;
            var idY = list[index].y;
            materials[idX].SetColor("_Color", Color.white);
            materials[idY].SetColor("_Color", Color.white);
            obj[idX].transform.localScale = origScale;
            obj[idY].transform.localScale = origScale;
            
        }
    }
}