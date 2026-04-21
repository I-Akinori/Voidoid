using UnityEngine;

public class OpenWall : MonoBehaviour
{
    private float angle = 0;
    private float time = 0;
    private Vector3 originalAngle;
    [SerializeField] AnimationCurve curve;

    void Start()
    {
        time = 0;
        originalAngle = this.transform.localEulerAngles;
        angle = originalAngle.x;
    }
    void Update()
    {
        time += Time.deltaTime;
        angle = curve.Evaluate(time);
        this.transform.localRotation = 
            Quaternion.Euler(angle, originalAngle.y, originalAngle.z);
    }
}
