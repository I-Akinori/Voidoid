using UnityEngine;
using UnityEngine.Events;
using Voidoid.Sound;

namespace Voidoid
{
    public class TimelineManager : SingletonMonoBehaviour<TimelineManager>
    {
        public float antiDelay = 0.0f;
        public float Time => (float)SoundManager.Instance.CurrentSongTime - antiDelay;
        public float DeltaTime => (float)SoundManager.Instance.CurrentSongTime - Time;
        public UnityEvent onLoop;

        void Start()
        {
            SoundManager.Instance.OnSongLoop += this.OnLoop;
        }

        public void OnLoop(int loopCount)
        {
            onLoop?.Invoke();
        }

    }
}