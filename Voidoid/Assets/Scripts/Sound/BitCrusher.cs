using UnityEngine;

namespace Voidoid.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class Bitcrusher : MonoBehaviour
    {
        [SerializeField] private ImageEffector effector; // 色収差強度を参照;
        [Range(1, 50)]
        public int downsampleFactor = 10; // サンプル間引きの強さ

        [Range(1, 16)]
        public int bitDepth = 8; // ビット深度（小さいほど荒い）

        private float holdSample = 0f;
        private int count = 0;

        void OnAudioFilterRead(float[] data, int channels)
        {
            if (effector.AberrationIntensity == 0)
            {
                return;
            }
            downsampleFactor = Mathf.Max(1, 2 * Mathf.CeilToInt(effector.AberrationIntensity * 12));
            float step = Mathf.Pow(2, bitDepth);

            for (int i = 0; i < data.Length; i++)
            {
                // --- ダウンサンプリング ---
                if (count++ % downsampleFactor == 0)
                {
                    holdSample = data[i];
                }

                float sample = holdSample;

                // --- ビット深度を落とす ---
                sample = Mathf.Round(sample * step) / step;

                data[i] = sample;
            }
        }
    }
}