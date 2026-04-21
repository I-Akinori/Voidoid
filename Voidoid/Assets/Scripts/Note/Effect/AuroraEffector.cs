using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class AuroraEffector : BaseEffector
    {
        [SerializeField] private string targetPort = "A";
        [SerializeField] private GameObject obj;
        [SerializeField] private AnimationCurve curve;
        
        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            var h = Mathf.Lerp(0.2f, 0.7f, Mathf.Clamp01(
                 ((note.octave - 3) * 12f + (int)note.pitch) / 18f
                )
            );
            obj.GetComponent<MeshRenderer>().material.SetColor("_Color" + targetPort, setting.color);
            obj.GetComponent<MeshRenderer>().material.SetFloat("_Height" + targetPort, h);
        }

        public override void ContinuePlay(PlayerSettings setting, Note note, int index, float time, Note next = null)
        {
            obj.GetComponent<MeshRenderer>().material.SetFloat("_Value" + targetPort, curve.Evaluate(time));
        }

        public override void EndPlay(int index)
        {
            
        }
    }
}