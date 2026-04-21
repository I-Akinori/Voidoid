using UnityEngine;

namespace Voidoid.Note
{
    public class ScoreViewer : MonoBehaviour
    {
        [SerializeField] private ScoreData data = null;
        [SerializeField, Range(0, 1f)] private float barInterval = 0.5f;
        [SerializeField, Range(0, 1f)] private float pitchInterval = 0.5f;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            DrawGrid();
            DrawScore();
            DrawTimeline();
        }

        void DrawGrid()
        {
            for (var i = 0; i < 72; i++)
            {
                var start = new Vector3(i * barInterval, 0, 0);
                var end = new Vector3(i * barInterval, 10, 0);
                Debug.DrawLine(start, end, Color.gray);
            }
            for (var i = 0; i < 72; i++)
            {
                var height = i * 0.1f * pitchInterval;
                var start = new Vector3(0, height, 0);
                var end = new Vector3(72 * barInterval, height, 0);
                Debug.DrawLine(start, end, Color.gray);
            }
        }

        void DrawScore()
        {
            for (var i = 0; i < data.score.Count; i++)
            {
                var note = data.score[i];
                var height = (note.octave * 12 + (int)note.pitch) * 0.1f * pitchInterval;
                var start = new Vector3((note.bar + note.beat * 0.25f) * barInterval, height, 0);
                var end = start + Vector3.right * (note.duration * 0.25f * barInterval);
                Debug.DrawLine(start, end, Color.red);
            }
        }
        void DrawTimeline()
        {
            for (var i = 0; i < 64; i++)
            {
                var start = new Vector3(i * barInterval, -1, 0);
                var end = new Vector3(i * barInterval, 1, 0);
                Debug.DrawLine(start, end, Color.gray);
            }
        }
    }

    
}