using UnityEngine;
using System.Collections;
public class SphereMicReaction : MonoBehaviour
{
    public float pulseScale = 1.2f;
    public float pulseSpeed = 2f;
    public float fadeDuration = 0.5f;
    public CanvasGroup canvasGroup; // For UI fade (if using UI Image)

    private Vector3 originalScale;
    private bool isPulsing = false;

    void Start()
    {
        originalScale = transform.localScale;
        //HideInstant();
    }

    void Update()
    {
        if (isPulsing)
        {
            float scale = 10 * Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed) * 0.1f);
            transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    public void ShowAndPulse()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        transform.localScale = originalScale;
        if (canvasGroup) canvasGroup.alpha = 1f;
        isPulsing = true;
    }

    public void HideSmoothly()
    {
        isPulsing = false;
        StartCoroutine(FadeAndShrink());
    }

    void HideInstant()
    {
        gameObject.SetActive(false);
        if (canvasGroup) canvasGroup.alpha = 0f;
    }

    private IEnumerator FadeAndShrink()
    {
        float t = 0f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.zero;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float normalized = t / fadeDuration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, normalized);
            if (canvasGroup) canvasGroup.alpha = 1f - normalized;
            yield return null;
        }

        HideInstant();
    }
}
