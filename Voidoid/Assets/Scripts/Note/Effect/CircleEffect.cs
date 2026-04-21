using UnityEngine;

namespace Voidoid.Note.Effect
{
    public class CircleEffect : MonoBehaviour
    {
        public float LifeTime => lifeTime;
        [SerializeField] private Material originalMat;
        [SerializeField] private float lifeTime = 2;
        [SerializeField] private Color color = Color.red;

        private Material mat;
        

        private float time;
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Awake()
        {
            time = 0;
            mat = new Material(originalMat);
            color = Color.HSVToRGB(Random.Range(0f, 1f), 1f, 1f);
            mat.SetColor("_Color", color);
            GetComponent<Renderer>().material = mat;
        }

        public void SetColor(Color col)
        {
            this.color = col;
            mat.SetColor("_Color", this.color);
        }

        public void SetLifeTime(float lifeT)
        {
            this.lifeTime = lifeT;
        }

        public void Dispose()
        {
            Destroy(mat);
            Destroy(this.gameObject);
        }

        public bool Step(float t)
        {
            time += t;
            mat.SetFloat("_EffectTime", time / lifeTime);
            this.transform.localPosition += Vector3.forward * t;
            
            // 寿命が来たかどうかを返す
            return time >= lifeTime;
        }
    }
}