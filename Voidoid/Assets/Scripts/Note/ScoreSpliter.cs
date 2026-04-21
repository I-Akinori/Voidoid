using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Voidoid.Note
{
    public class ScoreSpliter : MonoBehaviour
    {
        [SerializeField] private ScoreData input = null;
        [SerializeField] private List<ScoreData> outputs = null;
        [SerializeField] private int split0 = 40;
        [SerializeField] private int split1 = 56;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            for (var i = 0; i < outputs.Count; i++)
            {
                outputs[i].score.Clear();
            }
            for (var i = 0; i < input.score.Count; i++)
            {
                var score = input.score[i];
                if (score.bar < split0)
                {
                    outputs[0].score.Add(score);
                } else if (score.bar < split1)
                {
                    outputs[1].score.Add(score);
                }
                else
                {
                    outputs[2].score.Add(score);
                }
            }
            for (var i = 0; i < outputs.Count; i++)
            {
                //EditorUtility.SetDirty(outputs[i]);
            }
            
            // ディスクへ保存
            //AssetDatabase.SaveAssets();
            //AssetDatabase.Refresh();
        }
    }
}