using UnityEngine;

public class MoonMover : MonoBehaviour
{
    [SerializeField, Range(0f, 100f)] private float rotAngleSpeed = 0.25f;
    [SerializeField, Range(0f, 100f)] private float posAngleSpeed = 0.25f;
    private float rotAngle = 0f;
    private float posAngle = 0f;
    [SerializeField] private Vector3 orbitCenter = Vector3.zero;
    [SerializeField] private Vector3 orbitDir = Vector3.up;
    [SerializeField] private Vector3 rotAxis = Vector3.up;
    [SerializeField, Range(0, 10)] private float orbitRad = 2f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rotAngle = 0f;
        posAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        rotAngle += Time.deltaTime * rotAngleSpeed;
        posAngle += Time.deltaTime * posAngleSpeed;
        this.transform.rotation = Quaternion.AngleAxis(rotAngle, rotAxis);
        Debug.DrawRay(orbitCenter, orbitDir, Color.red);
        var orbitPolorAxis = Vector3.Cross(orbitDir, Vector3.forward).normalized;
        var orbitPolorDir = Vector3.Cross(orbitDir, orbitPolorAxis).normalized;
        Debug.DrawLine(orbitCenter, orbitCenter + orbitPolorAxis * orbitRad, Color.green);
        Debug.DrawLine(orbitCenter, orbitCenter + orbitPolorDir * orbitRad, Color.cyan);

        this.transform.localPosition = orbitCenter 
                                       + orbitPolorAxis * Mathf.Cos(posAngle) * orbitRad
                                       + orbitPolorDir * Mathf.Sin(posAngle) * orbitRad;
    }
}
