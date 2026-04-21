using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class CubeEffector : BaseEffector
    {
        [SerializeField] private CubeEffect src;
        private List<CubeEffect> effects = new List<CubeEffect>();
        
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
            var height = (note.octave * 12 + (int)note.pitch - 36) * 0.1f;
            var width = 10f;
            var x = (TimelineManager.Instance.Time * 1.0f + setting.deltaX) % width - width * 0.5f;

            var pos = new Vector3(
                x,
                height * 2 - 2,
                5
            );
            var timeScale = 10f;
            var scale = Mathf.Pow(note.duration, 0.5f) + 0.5f;
            scale *= setting.scaleRatio;
            this.GenerateEffect(pos, setting.color,
                note.duration * 0.25f * timeScale, scale);
        }
        private void GenerateEffect(Vector3 position, Color color, float lifeTime, float scale = 1f)
        {
            var efct = Instantiate(src, transform, false);
            efct.transform.localPosition = position;
            efct.SetColor(color);
            efct.SetLifeTime(lifeTime);
            //efct.transform.localScale = Vector3.one * scale;
            effects.Add(efct);
        }
    }
}