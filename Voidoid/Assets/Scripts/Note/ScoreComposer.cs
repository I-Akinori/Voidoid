using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Voidoid.Note
{
    public class ScoreComposer : MonoBehaviour
    {
        [SerializeField] private List<ScoreData> inputs = null;
        [SerializeField] private ScoreData output = null;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            output.score.Clear();
            for (var i = 0; i < inputs.Count; i++)
            {
                var input = inputs[i];
                for (var j = 0; j < input.score.Count; j++)
                {
                    output.score.Add(input.score[j].Clone);
                }
            }
        }
    }
}