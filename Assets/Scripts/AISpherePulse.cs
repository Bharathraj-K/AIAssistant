using UnityEngine;

public class AISpherePulse : MonoBehaviour
{
    public float scaleAmount = 0.15f;
    public float pulseSpeed = 2f;
    private Vector3 baseScale;
    private Material sphereMat;
    private float pulse;

    void Start()
    {
        baseScale = transform.localScale;
        Renderer renderer = GetComponent<Renderer>();
        sphereMat = renderer.material;
    }

    void Update()
    {
        // Calculate synced pulse
        pulse = Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f;

        // Apply scale pulse
        transform.localScale = baseScale + Vector3.one * (pulse * scaleAmount);

        // Apply to emission (pulse intensity)
        if (sphereMat != null)
        {
            sphereMat.SetFloat("_pulseValue", pulse);
        }

        // Optional rotation
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }
}
