using Unity.VisualScripting;
using UnityEngine;

namespace Voidoid.Note
{
    public class TestScoreGenerator : MonoBehaviour
    {
        [SerializeField] private ScoreData data = null;

        private int index = 0;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            data.score.Clear();
            for (var i = index; i < 16; i++)
            {
                var id = i % 4;
                Note n;
                switch (id)
                {
                    case 1:
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1f;
                        n.octave = 4;
                        n.pitch = Pitch.C;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1.5f;
                        n.octave = 3;
                        n.pitch = Pitch.G;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2f;
                        n.octave = 3;
                        n.pitch = Pitch.E;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2.5f;
                        n.octave = 3;
                        n.pitch = Pitch.G;
                        data.score.Add(n);
                        break;
                    case 3:
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1f;
                        n.octave = 4;
                        n.pitch = Pitch.G;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1.5f;
                        n.octave = 4;
                        n.pitch = Pitch.E;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2f;
                        n.octave = 4;
                        n.pitch = Pitch.D;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2.5f;
                        n.octave = 4;
                        n.pitch = Pitch.E;
                        data.score.Add(n);
                        break;
                    default:
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1f;
                        n.octave = 4;
                        n.pitch = Pitch.D;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 1.5f;
                        n.octave = 3;
                        n.pitch = Pitch.A;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2f;
                        n.octave = 3;
                        n.pitch = Pitch.G;
                        data.score.Add(n);
                        
                        n = new Note();
                        n.duration = 0.5f;
                        n.bar = 40 + i;
                        n.beat = 2.5f;
                        n.octave = 3;
                        n.pitch = Pitch.A;
                        data.score.Add(n);
                        break;
                }
                var note = new Note();
                note.duration = 0.5f;
                note.bar = 40 + i;
                note.beat = 0;
                note.octave = 4;
                if (i % 8 < 4)
                {
                    note.pitch = Pitch.C;
                }
                else
                {
                    note.pitch = Pitch.D;
                }
                //data.score.Add(note);
            }
        }
        
    }
}