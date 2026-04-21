using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Voidoid
{
    [Serializable]
    public struct EventSet
    {
        public KeyCode key;
        public UnityEvent onKeyDown;
        public UnityEvent onKey;
    }
    public class InputManager : MonoBehaviour
    {
        [SerializeField] public List<EventSet> onInput;

        private void Update()
        {
            if (onInput.Count <= 0)
            {
                return;
            }

            foreach (var input in onInput)
            {
                var key = input.key;
                if (Input.GetKeyDown(key))
                {
                    input.onKeyDown?.Invoke();
                }
                if (Input.GetKey(key))
                {
                    input.onKey?.Invoke();
                }
            }
        }
    }
}