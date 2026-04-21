using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class LineEffector : BaseEffector
    {
        [SerializeField] private LineEffect src;
        private List<LineEffect> effects = new List<LineEffect>();
        private int currentBar = 0;
        private Vector3 currentPos = Vector3.zero;
        private float lastEffect = 0;   // 4小節を1とする長さ
        
        void Update()
        {
            var delta = Time.deltaTime;
            for (var i = effects.Count - 1; i >= 0; i--)
            {
                var e = effects[i];
                if (e.Step(delta))
                {
                    e.Dispose();
                    effects.RemoveAt(i);
                }
            }
        }

        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            var pos = ComputePosition(note);
            var timeScale = 10f;
            var scale = Mathf.Pow(note.duration, 0.5f) + 0.5f;
            
            // 次の4小節に行ったか、1小節の空きがあったら新規生成
            if (currentBar > note.bar // ループ対応
                || currentBar / 4 < note.bar / 4 ||
                (note.bar % 4 + note.beat * 0.25f) * 0.25f - lastEffect >= 0.25f)
            {
                // 新規エフェクト
                this.GenerateEffect(pos, setting.color,
                    note.duration * 0.25f * timeScale, scale);
                currentBar = note.bar;
            }
            else if (this.effects.Count > 0)
            {
                // エフェクト更新
                this.effects[^1].AddPoint(pos);
            }
            this.lastEffect = (note.bar % 4 + note.beat * 0.25f) * 0.25f;
            this.currentPos = pos;
            this.effects[^1].SetMarker();
        }

        public override void ContinuePlay(PlayerSettings p, Note n, int index, float time, Note next = null)
        {
            if (next == null)
            {
                return;
            }

            var pos = ComputePosition(next);

            var dist = (next.bar % 4 + next.beat * 0.25f) * 0.25f - lastEffect;
            if (currentBar / 4 < next.bar / 4 || dist >= 0.25f)
            {
                return;
            }
            
            this.effects[^1].AddPoint(Vector3.Lerp(currentPos, pos, time));
        }

        private Vector3 ComputePosition(Note note)
        {
            var height = (note.octave * 12 + (int)note.pitch - 36) * 0.055f;
            var width = 10f;
            var v = (note.bar % 4 + note.beat * 0.25f) * 0.25f; // 4小節を1とするスケール
            v = v < 0.5f ? v * 2 - 0.5f : (1 - v) * 2 - 0.5f; // -1 → 1 → -1　と折り返す
            var x = v * width;
            var offsetY = (note.bar % 2) * 0.5f;    // 折り返しで重なるのを回避

            var pos = new Vector3(
                x,
                height * 2 - 2 + offsetY,
                4
            );
            return pos;
        }

        private void GenerateEffect(Vector3 position, Color color, float lifeTime, float scale = 1f)
        {
            var efct = Instantiate(src, transform, false);
            efct.AddPoint(position);
            efct.SetColor(color);
            efct.SetLifeTime(lifeTime);
            effects.Add(efct);
        }
    }
}