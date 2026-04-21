using UnityEngine;

public class InteractiveMaterialController : MonoBehaviour
{
    [SerializeField] private Material material;
    private float[] inputTimes = new float[16];
    private float[] inputIntensity = new float[16];

    [SerializeField] private float sustain;
    [SerializeField] private AnimationCurve curve;

    private void Update()
    {
        var dt = Time.deltaTime;
        for (var i = 0; i < 16; i++)
        {
            if (inputTimes[i] > 0)
            {
                inputTimes[i] -= dt;
            }
            else
            {
                inputTimes[i] = 0;
            }
            inputIntensity[i] = curve.Evaluate(1 - inputTimes[i] / sustain);
        }

        material.SetFloatArray("_InputIntensity", inputIntensity);
    }

    public void EmitEffect(int i)
    {
        inputTimes[i] = sustain;
    }
}