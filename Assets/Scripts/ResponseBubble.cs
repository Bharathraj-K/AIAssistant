using UnityEngine;
using TMPro;
using System.Collections;

public class ResponseBubble : MonoBehaviour
{
    public CanvasGroup canvasGroup;
    public TMP_Text responseText;
    public float fadeInDuration = 0.4f;
    public float typingSpeed = 0.001f;

    void Awake()
    {
        canvasGroup.alpha = 0f;
        responseText.text = "";
    }

    public void ShowBubble(string fullResponse)
    {
        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateBubble(fullResponse));
    }

    IEnumerator AnimateBubble(string text)
    {
        canvasGroup.alpha = 0f;
        responseText.text = "";

        // Fade in
        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0, 1, t / fadeInDuration);
            yield return null;
        }

        responseText.text = text;
        // // Typing effect
        // foreach (char c in text)
        // {
        //     responseText.text += c;
        //     yield return new WaitForSeconds(typingSpeed);
        // }
    }

    public void disable(){
        gameObject.SetActive(false);
    }
}
