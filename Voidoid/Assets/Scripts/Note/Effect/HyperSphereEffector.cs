using System.Collections.Generic;
using UnityEngine;
using Voidoid.HyperObject;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class HyperSphereEffector : BaseEffector
    {
        [SerializeField] private HyperSphereEffect effect;
        [SerializeField] private float sustain = 2f;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        [SerializeField] private float[] times = new float[8]{0, 0, 0, 0, 0, 0, 0, 0};
        private Color[] colors = new Color[8];

        private void Update()
        {
            var delta = TimelineManager.Instance.DeltaTime;
            for (var i = 0; i < times.Length; i++)
            {
                if (times[i] > 0)
                {
                    times[i] -= delta;
                    UpdateColor(i);
                    if (times[i] <= 0)
                    {
                        times[i] = 0;
                        UpdateColor(i, inactiveColor);
                    }
                }
                else
                {
                    times[i] = 0;
                }
            }
        }
        
        private void UpdateColor(int index)
        {
            var col = Color.Lerp(inactiveColor, colors[index], curve.Evaluate(1 - times[index] / sustain));
            effect.SetColors(col, index);
        }

        private void UpdateColor(int index, Color color)
        {
            effect.SetColors(color, index);
        }
        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            // 通常はC1～C4 / ラスサビだけ高いのでC3～C5
            var offset = note.bar < 64 ? -1 : -3;
            var range = note.bar < 64 ? 36f : 24f;
            var h = Mathf.Clamp01(((note.octave + offset) * 12f + (int)note.pitch) / range);
            var id = Mathf.FloorToInt(h * effect.Resolution);
            if (id >= effect.Resolution - 1)
            {
                id = effect.Resolution - 2;
            }

            times[id] = sustain;
            colors[id] = setting.color;
        }

        public override void ContinuePlay(PlayerSettings setting, Note note, int index, float time, Note next = null)
        {
        }

        public override void EndPlay(int index)
        {
        }
    }
}