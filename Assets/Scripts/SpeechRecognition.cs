using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;
using TMPro;


public class SpeechRecognition : MonoBehaviour
{
    private DictationRecognizer dictationRecognizer;
    public TMP_Text userSpeechDisplay;   // e.g., user input
    public TMP_Text aiResponseDisplay;   // e.g., AI response output
    private GroqManager groqManager;
    void Start()
    {
        // Initialize DictationRecognizer
        dictationRecognizer = new DictationRecognizer();
        groqManager = FindFirstObjectByType<GroqManager>();
        // Subscribe to DictationRecognizer events
        dictationRecognizer.DictationResult += OnDictationResult;
        dictationRecognizer.DictationHypothesis += OnDictationHypothesis;
        dictationRecognizer.DictationComplete += OnDictationComplete;
        dictationRecognizer.DictationError += OnDictationError;

        // Start the DictationRecognizer
        dictationRecognizer.Start();
    }

    private async void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        userSpeechDisplay.text = text;

        if (groqManager != null)
        {
            string response = await groqManager.SendMessageToGroq(text);
            aiResponseDisplay.text = response;
        }
    }
    private void OnDictationHypothesis(string text)
    {
        Debug.Log($"Dictation hypothesis: {text}");
        // Optionally, update UI to show the hypothesis
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        if (cause != DictationCompletionCause.Complete)
            Debug.LogError($"Dictation completed unsuccessfully: {cause}");
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError($"Dictation error: {error}; HResult = {hresult}");
    }

    void OnDestroy()
    {
        // Clean up
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}
