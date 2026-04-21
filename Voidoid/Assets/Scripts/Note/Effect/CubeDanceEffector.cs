using System.Collections.Generic;
using UnityEngine;
using Voidoid.Note.Player;

namespace Voidoid.Note.Effect
{
    public class CubeDanceEffector : BaseEffector
    {
        [SerializeField] private InteractiveMaterialController controller;
        private int count = 0;
        private int offset = 0;

        public override void StartPlay(PlayerSettings setting, Note note, int index)
        {
            // 4音ごとにインデックスが2前に戻る: 0123, 2345, 4567, 6789, ...
            count++;
            if (count > 4)
            {
                //offset += 16 - 4;
                //offset %= 16;
                //count = 0;
            }
            //controller.EmitEffect(Mathf.FloorToInt(note.bar * 8 + note.beat * 2 + offset) % 16);
            controller.EmitEffect(count % 16);
        }
    }
}