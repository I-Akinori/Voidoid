using UnityEngine;
namespace Voidoid.Sound
{

    [RequireComponent(typeof(AudioLowPassFilter), typeof(AudioReverbFilter))]
    public class WobbleFilter : MonoBehaviour
    {
        [SerializeField] private ImageEffector effector; // 映像側の歪み強度を参照;
        public float baseCutoff = 1200f;
        public float wobbleAmount = 800f;
        public float wobbleSpeed = 2.5f;
        public float resonance = 2.0f;

        private AudioLowPassFilter lp;
        private AudioReverbFilter rv;

        void Awake()
        {
            lp = GetComponent<AudioLowPassFilter>();
            rv = GetComponent<AudioReverbFilter>();
        }

        void Update()
        {
            lp.cutoffFrequency = Mathf.Lerp(7000, 700,effector.VignetteIntensity);
            lp.lowpassResonanceQ = Mathf.Lerp(1, 1.5f,effector.VignetteIntensity);
            rv.reverbDelay = Mathf.Lerp(0, 0.03f,effector.VignetteIntensity);
            //float lfo = (Mathf.Sin(Time.time * wobbleSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
            //lp.cutoffFrequency = baseCutoff + (lfo - 0.5f) * 2f * wobbleAmount;
            //lp.lowpassResonanceQ = resonance;
        }
    }
}