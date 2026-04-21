using System.Collections.Generic;
using UnityEngine;

namespace Voidoid
{
    public class CubeController : MonoBehaviour
    {
        
        public Vector3 Scale => this.transform.localScale;
        public Vector3 Position => this.transform.position;
        public Quaternion Rotation => this.transform.rotation;
        
        [SerializeField] private bool enableBottom = true;
        [SerializeField] private bool enableTop = true;
        [SerializeField] private List<GameObject> bottom;
        [SerializeField] private List<GameObject> top;
        [SerializeField] private List<GameObject> left;
        [SerializeField] private List<GameObject> right;
        [SerializeField] private List<GameObject> back;
        [SerializeField] private List<GameObject> front;

        private void Start()
        {
            SetActiveBottom(enableBottom);
            SetActiveTop(enableTop);
        }
        private void Update()
        {
        
        }
        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }
        public void SetRotation(Quaternion rotation)
        {
            this.transform.rotation = rotation;
        }

        public void SetActiveBottom(bool b)
        {
            foreach (var btm in bottom)
            {
                btm.SetActive(b);
            }
        }
        public void SetActiveTop(bool b)
        {
            foreach (var tp in top)
            {
                tp.SetActive(b);
            }
        }
    }
}