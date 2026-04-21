using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Voidoid.Note.Player
{
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField, Range(0, 5f)] private float barLength = 0.5f;   // 1小節の秒数
        [SerializeField] private List<PlayerSettings> settings = new List<PlayerSettings>();

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            foreach (var setting in settings)
            {
                if (setting.Index >= setting.data.score.Count)
                {
                    continue;
                }
                var note = setting.data.score[setting.Index];
                Note next = null;
                if (setting.data.score.Count > setting.Index + 1)
                {
                    next = setting.data.score[setting.Index + 1];
                }
                
                if (setting.IsPlaying)
                {
                    var height = (note.octave * 12 + (int)note.pitch - 36) * 0.1f;
                    if (setting.isVisible)
                    {
                        // 継続エフェクト
                        //Debug.DrawRay(Vector3.up * height + Vector3.forward * setting.deltaZ + this.transform.position, Vector3.right * 10, setting.color);
                        setting.effector.ContinuePlay(
                                setting, note, setting.Index,
                                (TimelineManager.Instance.Time - setting.PlayStartTime) / (note.duration * 0.25f * this.barLength), next);
                    }
                    
                    if (TimelineManager.Instance.Time - setting.PlayStartTime >= note.duration * 0.25f * this.barLength)
                    {
                        setting.effector.EndPlay(setting.Index);
                        setting.FinishPlay();
                        setting.UpdateIndex();
                        if (setting.Index >= setting.data.score.Count)
                        {
                            continue;
                        }
                        note = setting.data.score[setting.Index];
                    }
                }
                
                if (!setting.IsPlaying && TimelineManager.Instance.Time >= (note.bar + note.beat * 0.25f) * barLength)
                {
                    setting.effector.StartPlay(setting, note, setting.Index);
                    setting.StartPlay();
                }
            }
        }

        public void ResetIndex()
        {
            foreach (var setting in settings)
            {
                setting.ResetIndex();
            }
        }
    }
}
