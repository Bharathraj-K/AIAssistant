using UnityEngine;
using System.Collections;

public class SphereMicReaction : MonoBehaviour
{
    public float targetScale = 10f;        // The final size it should grow to
    public float growSpeed = 2f;           // Speed of growth
    public float fadeDuration = 0.5f;
    public CanvasGroup canvasGroup;

    private Vector3 originalScale;
    private bool isGrowing = false;

    void Start()
    {
        originalScale = transform.localScale;
        //HideInstant();
    }

    void Update()
    {
        if (isGrowing)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(targetScale, targetScale, targetScale), Time.deltaTime * growSpeed);
        }
    }

    public void ShowAndGrow()
    {
        StopAllCoroutines();
        gameObject.SetActive(true);
        transform.localScale = originalScale;
        if (canvasGroup) canvasGroup.alpha = 1f;
        isGrowing = true;
    }

    public void HideSmoothly()
    {
        isGrowing = false;
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
