using UnityEngine;

namespace Voidoid.Note.Effect
{
    public class CubeEffect : MonoBehaviour
    {
        public float LifeTime => lifeTime;
        [SerializeField] private Material origCoreMat;
        [SerializeField] private Material origWaveMat;
        [SerializeField] private float lifeTime = 2;
        [SerializeField] private Color color = Color.red;
        [SerializeField] private AnimationCurve curve;
        
        private Material coreMat;
        private Material waveMat;
        [SerializeField] private GameObject coreObj;
        [SerializeField] private GameObject waveObj;
        

        private float time;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            time = 0;
            coreMat = new Material(origCoreMat);
            coreObj.GetComponent<Renderer>().material = coreMat;
            waveMat = new Material(origWaveMat);
            waveObj.GetComponent<Renderer>().material = waveMat;
            coreObj.transform.localScale = Vector3.zero;
            waveObj.transform.localScale = Vector3.zero;
        }

        public void SetColor(Color col)
        {
            this.color = col;
            coreMat.SetColor("_Color", this.color);
            waveMat.SetColor("_Color", this.color);
        }

        public void SetLifeTime(float lifeT)
        {
            this.lifeTime = lifeT;
        }

        public void Dispose()
        {
            Destroy(coreMat);
            Destroy(waveMat);
            Destroy(gameObject);
            Destroy(coreObj);
            Destroy(waveObj);
        }

        public bool Step(float t)
        {
            time += t;
            waveMat.SetFloat("_Value", time / lifeTime);
            coreObj.transform.localScale = Vector3.one * (curve.Evaluate(time) * 1f);
            waveObj.transform.localScale = Vector3.one * (2 + time * 4f);
            waveObj.transform.localRotation = Quaternion.Euler(0, 0, -45 + 180 * time / lifeTime);
            coreObj.transform.localPosition += Vector3.forward * (-2 * t);
            
            // 寿命が来たかどうかを返す
            return time >= lifeTime;
        }
    }
}