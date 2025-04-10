using UnityEngine;
using System.IO;

public class MicRecorder : MonoBehaviour
{
    private AudioClip recordedClip;
    private bool isRecording = false;
    private string micName;
    private string savePath;

    

    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            micName = Microphone.devices[0];
        }
        else
        {
            Debug.LogError("No microphone detected.");
        }

        // Save path inside project for testing
        savePath = Path.Combine(Application.dataPath, "Resources/recordings");
        Directory.CreateDirectory(savePath);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            StartRecording();

        if (Input.GetKeyUp(KeyCode.Space))
            StopRecording();
    }


    public void StartRecording()
    {
        if (isRecording || micName == null) return;

        recordedClip = Microphone.Start(micName, false, 10, 44100); // max 10s
        isRecording = true;
        Debug.Log("🎙️ Started Recording...");
    }

    public void StopRecording()
    {
        if (!isRecording) return;

        Microphone.End(micName);
        isRecording = false;
        Debug.Log("🛑 Stopped Recording");

        SaveClipToWav(recordedClip, "user_input.wav");
    }

    private void SaveClipToWav(AudioClip clip, string filename)
    {
        if (clip == null) return;

        string filePath = Path.Combine(savePath, filename);
        WavUtility.FromAudioClip(clip, filePath, true);
        Debug.Log("💾 Saved audio to: " + filePath);
    }
}
