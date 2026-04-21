using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Voidoid
{
    public class MainManager : MonoBehaviour
    {
        [SerializeField] private int displayIndex = 0;
        [SerializeField] private int targetFPS = 60;

        private void Start()
        {
            Application.targetFrameRate = targetFPS;
            var list = new List<DisplayInfo>();
            Screen.GetDisplayLayout(list);
            //Screen.MoveMainWindowTo(list[displayIndex], list[displayIndex].workArea.position);
        }
    }
}