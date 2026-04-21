using System;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class BaseEffector : MonoBehaviour
    {

        public virtual void StartPlay(PlayerSettings p, Note n, int index)
        {
        }

        public virtual void ContinuePlay(PlayerSettings p, Note n, int index, float time, Note next = null)
        {
            
        }

        public virtual void EndPlay(int index)
        {
            
        }
    }
}