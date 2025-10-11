using UnityEngine;

public class CubeMover : MonoBehaviour
{
    [SerializeField, Range(0, 10f)] float scalingSpeed = 5f;
    private float time = 0f;
    void Start()
    {
        time = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        var scale = 1 + Mathf.Sin(time * scalingSpeed);
        scale *= 0.5f;
        var scaleMin = 0.2f;
        scale = Mathf.Lerp(scaleMin, 1f, scale);
        this.transform.localScale = new Vector3(1, scale, 1);
        this.transform.localPosition = new Vector3(0, 0.5f - scale * 0.5f, 0);
    }
}
