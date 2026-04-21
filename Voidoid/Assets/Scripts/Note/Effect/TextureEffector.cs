using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class TextureEffector : BaseEffector
    {
        [SerializeField] private Material material;
        [SerializeField] private string targetPort = "A";
        [SerializeField] private Vector3 posOffset = new Vector2(0.5f, 0.5f);
        [SerializeField] private Vector2 heightRange = new Vector2(0, 1);

        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            var h = Mathf.Lerp(heightRange.x, heightRange.y, Mathf.Clamp01(
                    ((note.octave - 3) * 12f + (int)note.pitch) / 18f
                )
            );
            material.SetColor("_Color" + targetPort, setting.color);
            material.SetFloat("_Value" + targetPort, 1f);
            material.SetVector("_Center" + targetPort, new Vector4(posOffset.x, posOffset.y + h, posOffset.z, 1));
        }
        public override void ContinuePlay(PlayerSettings settings, Note note, int index, float time, Note next = null)
        {   
            material.SetFloat("_Value" + targetPort, time);
        }
    }
}