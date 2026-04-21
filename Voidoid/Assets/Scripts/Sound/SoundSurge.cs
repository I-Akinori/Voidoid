using UnityEngine;

namespace Voidoid.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundSurge: MonoBehaviour
    {
        [SerializeField] private ImageEffector effector; // 映像側の歪み強度を参照;
        [Range(0, 1)]
        public float speed = 0.5f;

        [Range(0, 1)]
        public float amount = 0.5f;

        private AudioSource audioSource;
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        void Update()
        {
            amount = effector.MaxDistortionIntensity * 0.1f;
            var lfo = Mathf.Sin(TimelineManager.Instance.Time * speed);
            SoundManager.Instance.pitch = 1.0f + lfo * amount;
        }
    }
}