using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class OutlineEffector : BaseEffector
    {
        [SerializeField] private Material material;
        
        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            var h = Mathf.Lerp(0.7f, 0.2f, Mathf.Clamp01(
                 ((note.octave - 3) * 12f + (int)note.pitch) / 18f
                )
            );
            material.SetColor("_Color", setting.color);
            material.SetFloat("_Height", h);
        }

        public override void ContinuePlay(PlayerSettings setting, Note note, int index, float time, Note next = null)
        {
            material.SetFloat("_Value", 1f - time);
        }

        public override void EndPlay(int index)
        {
            
        }
    }
}