using UnityEngine;
using Voidoid;

public class BackgroundManager : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private AnimationCurve cubeDanceCurve;
    [SerializeField] private AnimationCurve juliaCurve;

    private void Update()
    {
        SetTime(TimelineManager.Instance.Time);
    }

    public void SetTime(float time)
    {
        material.SetFloat("_CustomTime", time);
        material.SetFloat("_CubeDance", cubeDanceCurve.Evaluate(time));
        material.SetFloat("_Julia", juliaCurve.Evaluate(time));
    }
}