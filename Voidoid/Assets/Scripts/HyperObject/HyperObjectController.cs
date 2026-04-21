using UnityEngine;

namespace Voidoid.HyperObject
{
    [RequireComponent(typeof(BaseHyperObject))]
    public class HyperObjectController : MonoBehaviour
    {
        private BaseHyperObject obj;
        [SerializeField] AnimationCurve alphaCurve;
        [SerializeField] AnimationCurve scaleCurve;
        public void Awake()
        {
            obj =  gameObject.GetComponent<BaseHyperObject>();
        }

        private void Update()
        {
            obj.Alpha = alphaCurve.Evaluate(TimelineManager.Instance.Time);
            obj.Scaler = scaleCurve.Evaluate(TimelineManager.Instance.Time);
            if (obj.Active)
            {
                if (obj.Scaler <= 0 || obj.Alpha <= 0)
                {
                    obj.Active = false;
                }
            }
            else
            {
                if (obj.Scaler > 0 && obj.Alpha > 0)
                {
                    obj.Active = true;
                }
            }
        }
    }
}