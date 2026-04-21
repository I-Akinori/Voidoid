using UnityEngine;

public class ImageEffector : MonoBehaviour
{
    public float MaxDistortionIntensity => distIntensity[maxIndex];
    public float AberrationIntensity => abeIntensity;
    public float VignetteIntensity => vigIntensity;
    
    [SerializeField] private Material midMaterial;
    [SerializeField] private RenderTexture tmpTex;
    [SerializeField] private Material material;
    [SerializeField] private RenderTexture output;
    private float[] distTimes = new float[16];
    private float[] distIntensity = new float[16];
    private float vigTime;
    private float vigIntensity;
    private float abeTime;
    [SerializeField] private float abeIntensity;
    private int maxIndex = 0;

    [SerializeField] private float distSustain;
    [SerializeField] private AnimationCurve distCurve;
    private int target = 0;

    [SerializeField] private float vigSustain;
    [SerializeField] private AnimationCurve vigCurve;
    [SerializeField] private float abeSustain;
    [SerializeField] private AnimationCurve abeCurve;

    private void Update()
    {
        var dt = Time.deltaTime;
        for (var i = 0; i < 16; i++)
        {
            if (distTimes[i] > 0)
            {
                distTimes[i] -= dt;
            }
            else
            {
                distTimes[i] = 0;
            }
            distIntensity[i] = distCurve.Evaluate(1 - distTimes[i] / distSustain);
        }
        
        if (vigTime > 0)
        {
            vigTime -= dt;
        }
        else
        {
            vigTime = 0;
        }
        vigIntensity = vigCurve.Evaluate(1 - vigTime / vigSustain);
        
        if (abeTime > 0)
        {
            abeTime -= dt;
        }
        else
        {
            abeTime = 0;
        }
        abeIntensity = abeCurve.Evaluate(1 - abeTime / abeSustain);
    }

    public void StartDistortion()
    {
        target = target < 15 ? target + 1 : 0;
    }
    public void ContinueDistortion()
    {
        var dV = (distSustain - distTimes[target]) * 0.01f;
        distTimes[target] += dV;
        distTimes[target] = Mathf.Min(distTimes[target], distSustain);
        if (target != maxIndex && distTimes[target] > MaxDistortionIntensity)
        {
            maxIndex = target;
        }
    }
    public void StartVignette()
    {
    }
    public void ContinueVignette()
    {
        var dV = (vigSustain - vigTime) * 0.01f;
        vigTime += dV;
        vigTime = Mathf.Min(vigTime, vigSustain);
    }
    public void StartAberration()
    {
    }
    public void ContinueAberration()
    {
        var dV = (abeSustain - abeTime) * 0.04f;
        abeTime += dV;
        abeTime = Mathf.Min(abeTime, abeSustain);
    }
    
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // 壁エフェクトを引継ぐエフェクト (TextureEffectorがマテリアルを操作)
        Graphics.Blit(source, tmpTex, midMaterial);
        // インタラクティブエフェクト
        material.SetFloatArray("_DistortionIntensity", distIntensity);
        material.SetFloat("_VignetteIntensity", vigIntensity);
        material.SetFloat("_AberrationIntensity", abeIntensity);
        Graphics.Blit(tmpTex, output, material);
    }
}
