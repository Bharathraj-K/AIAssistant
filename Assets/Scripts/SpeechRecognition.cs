using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System.Collections;
using System.Text;

public class SpeechRecognition : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    private StringBuilder resultBuffer = new StringBuilder();
    private GroqManager groqManager;

    public TMP_Text userSpeechDisplay;   // Shows final user input
    public TMP_Text aiResponseDisplay;   // Shows AI response
    public float typingSpeed = 0.02f;

    private bool isRecording = false;

    void Start()
    {
        groqManager = FindFirstObjectByType<GroqManager>();

        dictationRecognizer = new DictationRecognizer();
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            resultBuffer.Clear();
            userSpeechDisplay.text = "";
            Debug.Log("🎙️ Mic started");
            isRecording = true;

            if (dictationRecognizer.Status != SpeechSystemStatus.Running)
                dictationRecognizer.Start();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("🛑 Mic released, finalizing...");
            isRecording = false;

            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();
        }
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        resultBuffer.Append(text + " ");
        userSpeechDisplay.text = resultBuffer.ToString();
    }

    private async void OnDictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"Dictation completed: {cause}");

        if (isRecording)
        {
            // Auto-restart if still holding space
            Debug.Log("🔁 Restarting due to silence...");
            dictationRecognizer.Start();
        }
        else
        {
            string finalText = resultBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(finalText))
            {
                Debug.Log("📤 Sending to Groq: " + finalText);
                string response = await groqManager.SendMessageToGroq(finalText);
                StartCoroutine(TypeText(response));
            }
        }
    }

    private void OnDictationHypothesis(string text)
    {
        // Optional live preview, you can show this elsewhere
        Debug.Log($"🧠 Hypothesis: {text}");
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError($"❌ Dictation error: {error}; HResult = {hresult}");
    }

    IEnumerator TypeText(string response)
    {
        aiResponseDisplay.text = "";
        foreach (char letter in response.ToCharArray())
        {
            aiResponseDisplay.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}
