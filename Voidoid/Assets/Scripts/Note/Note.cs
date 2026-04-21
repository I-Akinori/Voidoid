using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Voidoid.Note
{
    [Serializable]
    public class Note
    {
        public int bar = 0;         // 小節
        public float beat = 0;      // 拍: 1小節を4とする
        public float duration = 0.25f;  // 音の長さ: beatと同じ単位
        public int octave = 3;
        public Pitch pitch = Pitch.C;

        public Note Clone => new Note()
        {
            bar = this.bar,
            beat = this.beat,
            duration = this.duration,
            octave = this.octave,
            pitch = this.pitch
        };
    }
    public enum Pitch
    {
        C, Cs, D, Ds, E, F, Fs, G, Gs, A, As, B
    }
}
