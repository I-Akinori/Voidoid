using System;
using UnityEngine;
using Voidoid.Note.Effect;

namespace Voidoid.Note.Player
{
    [Serializable]
    public class PlayerSettings
    {
        public ScoreData data;
        public BaseEffector effector;
        public Color color;
        public float deltaX;
        public float deltaZ;
        public float scaleRatio;
        public bool isVisible;
        private int index = 0;
        private bool isPlaying = false;
        private float playStartTime = 0f;
        
        
        public int Index => index;
        public bool IsPlaying => isPlaying;
        public float PlayStartTime => playStartTime;

        public void StartPlay()
        {
            isPlaying = true;
            playStartTime = TimelineManager.Instance.Time;
        }

        public void FinishPlay()
        {
            isPlaying = false;
        }

        public void UpdateIndex()
        {
            index++;
        }

        public void ResetIndex()
        {
            index = 0;
            playStartTime = 0;
        }
    }
}