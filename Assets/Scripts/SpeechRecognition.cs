using UnityEngine;
using UnityEngine.Windows.Speech;
using TMPro;
using System.Collections;
using System.Text;
using System.Threading.Tasks;

public class SpeechRecognition : MonoBehaviour
{
    public ResponseBubble responseBubble; 

    public SphereMicReaction micSphere; 

    private DictationRecognizer dictationRecognizer;
    private StringBuilder resultBuffer = new StringBuilder();
    private GroqManager groqManager;

    public TMP_Text userSpeechDisplay;
    public TMP_Text aiResponseDisplay;
    public TMP_Text micButtonText; 
    // public float typingSpeed = 0.02f;
    private bool isMicActive = false;
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
    if (Input.GetKeyUp(KeyCode.DownArrow))
    {
        resultBuffer.Clear();
        userSpeechDisplay.text = "";
        Debug.Log("🎙️ Mic started");
        isRecording = true;

        if (dictationRecognizer.Status != SpeechSystemStatus.Running)
        {
            micSphere.ShowAndGrow();
            responseBubble.disable();
            dictationRecognizer.Start();
        }
    }

    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
        Debug.Log("🛑 Mic released , finalizing...");
        isRecording = false;

        if (dictationRecognizer.Status == SpeechSystemStatus.Running)
            dictationRecognizer.Stop();
    }
}



    public void OnMicToggle()
    {
        if (isMicActive)
        {
            micButtonText.text = "ClickToSpeak";
            isMicActive = false;
            if (dictationRecognizer.Status == SpeechSystemStatus.Running)
                dictationRecognizer.Stop();
            Debug.Log("🛑 Mic OFF");
        }
        else
        {
            micButtonText.text = "Listening..";
            resultBuffer.Clear();
            userSpeechDisplay.text = "";
            isMicActive = true;
            if (dictationRecognizer.Status != SpeechSystemStatus.Running){
                //controlling sphere and bubble 
                micSphere.ShowAndGrow();
                responseBubble.disable();
                dictationRecognizer.Start();
            }
            Debug.Log("🎙️ Mic ON");
        }
    }

    async public void PromptByText(string text){
        if (text != null)
        {
            micSphere.HideSmoothly();
            if (groqManager != null)
            {
                string response = await groqManager.SendMessageToGroq(text);
                //show bubble
                responseBubble.ShowBubble(response);
            }
        }
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        
        resultBuffer.Append(text + " ");
        userSpeechDisplay.text = resultBuffer.ToString();
    }

    private void OnDictationHypothesis(string text)
    {
        Debug.Log($"🧠 Hypothesis: {text}");
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError($"❌ Dictation error: {error}; HResult = {hresult}");
    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        micSphere.HideSmoothly();
        Debug.Log($"Dictation completed: {cause}");

        // If it was caused by silence/timeout
        if (cause != DictationCompletionCause.Complete)
        {
            micButtonText.text = "ClickToSpeak";
            isMicActive = false;
        }

        if (isRecording)
        {
            Debug.Log("🔁 Restarting due to silence...");
            dictationRecognizer.Start();
        }
        else
        {
            _ = ProcessUserInputAfterDelay();
        }
    }


    private async Task ProcessUserInputAfterDelay()
    {
        await Task.Delay(750);

        string finalText = resultBuffer.ToString().Trim();

        if (string.IsNullOrEmpty(finalText))
        {
            userSpeechDisplay.text = "Hmmm...All Silence";
            aiResponseDisplay.text = "";
            Debug.Log("❓ No audio captured.");
            return;
        }

        Debug.Log("📤 Sending to Groq: " + finalText);
        userSpeechDisplay.text = finalText;

        if (groqManager != null)
        {
            string response = await groqManager.SendMessageToGroq(finalText);
            //show bubble
            responseBubble.ShowBubble(response);
        }
    }

    // IEnumerator TypeText(string response)
    // {
    //     aiResponseDisplay.text = "";
    //     foreach (char letter in response.ToCharArray())
    //     {
    //         aiResponseDisplay.text += letter;
    //         yield return new WaitForSeconds(typingSpeed);
    //     }
    // }

    void OnDestroy()
    {
        if (dictationRecognizer != null)
        {
            dictationRecognizer.Stop();
            dictationRecognizer.Dispose();
        }
    }
}
