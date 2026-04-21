using System.Collections.Generic;
using UnityEngine;

namespace Voidoid
{
    public class MaterialController : MonoBehaviour
    {
        [SerializeField] private List<MaterialControlProperty> settings;
        
        void Update()
        {
            foreach (var s in settings)
            {
                s.material.SetFloat(s.property, s.curve.Evaluate(TimelineManager.Instance.Time));
            }
        }
    }
}