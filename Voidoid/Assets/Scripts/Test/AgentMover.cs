using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class AgentMover : MonoBehaviour
{
    private Vector3 basePosition = new Vector3(1f, 0f, 0f);
    [SerializeField] private Vector3 center = Vector3.zero;
    [SerializeField, Range(0, 10)] private float rad = 1;
    [SerializeField, Range(0, 10)] private float radWeight = 1;
    [SerializeField, Range(0, 10)] private float swayWeight = 3;
    [SerializeField, Range(0, 10)] private float waveWeight = 1;
    [SerializeField, Range(0, 10)] private float attractWeight = 1;
    [SerializeField, Range(0, 10)] private float swaySpeed = 3;
    [SerializeField, Range(0, 10)] private float waveSpeed = 3;
    [SerializeField, Range(0, 10)] private float speed = 1;
    [SerializeField] private GameObject target;
    [SerializeField, Range(1, 1000)] private int maxPositions = 100;
    private Vector3 velocity = Vector3.zero;
    private List<Vector3> positions = new List<Vector3>();
    private LineRenderer lineRenderer;

    private int count = 0;
    [SerializeField] private int sampleCount = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.basePosition = new Vector3(0f, 0f, 1f);
        velocity = Vector3.one.normalized;
        InitLineRenderer();
    }

    // Update is called once per frame
    void Update()
    {
        var localPos = this.transform.position - center;
        var radForce = localPos * (rad - localPos.magnitude);
        var nor = localPos.normalized;
        var dir = velocity.normalized;
        var tan = Vector3.Cross(dir, nor);
        var angle = Mathf.PerlinNoise1D(Time.time * swaySpeed) - 0.5f;
        angle *= Mathf.PI;
        var swayForce = dir * Mathf.Cos(angle) + tan * Mathf.Sin(angle);
        var waveForce = nor * Mathf.PerlinNoise1D(100 - Time.time * waveSpeed);
        var attractForce = target.transform.position - this.transform.position;
        velocity += (radForce * radWeight + swayForce * swayWeight + waveForce * waveWeight + attractForce * attractWeight) * Time.deltaTime;
        velocity.Normalize();
        velocity *= speed;
        this.transform.position += velocity * Time.deltaTime;
        this.transform.rotation = Quaternion.LookRotation(velocity);

        count++;
        if (count >= sampleCount)
        {
            count = 0;
            positions.Add(this.transform.position);
        }
        if (positions.Count > maxPositions)
        {
            positions.RemoveAt(0);
        }
        UpdateLineRenderer();
    }

    void InitLineRenderer()
    {
        this.lineRenderer = this.gameObject.GetComponent<LineRenderer>();
    }
    void UpdateLineRenderer()
    {
        lineRenderer.endWidth = 0.05f;
        lineRenderer.startWidth = 0.0f;
        lineRenderer.endColor = new Color(0.5f, 1.0f, 0.7f, 1.0f);
        lineRenderer.startColor = new Color(0.0f, 1.0f, 1.0f, 0.0f);
        lineRenderer.positionCount = this.positions.Count;
        lineRenderer.SetPositions(this.positions.ToArray());
    }
}
