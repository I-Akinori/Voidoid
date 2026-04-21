using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class DebugEffector : BaseEffector
    {
        public override void ContinuePlay(PlayerSettings settings, Note note, int index, float time, Note next = null)
        {   
            Debug.Log(time);
            var height = (note.octave * 12 + (int)note.pitch - 36) * 0.1f;
            Debug.DrawRay(Vector3.up * height + Vector3.forward * settings.deltaZ + this.transform.position, Vector3.right * 10, settings.color);
        }

        public override void EndPlay(int index)
        {
            
        }
    }
}