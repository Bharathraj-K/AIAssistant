using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
public class GroqManager : MonoBehaviour
{
    private List<Message> conversationHistory = new List<Message>();
    private string apiKey = "";
    private string keyFileName = "groq_key.txt";
    private string apiUrl = "https://api.groq.com/openai/v1/chat/completions";

    void Awake()
    {
        LoadApiKey();
    }

    private void LoadApiKey()
    {
        string path = Path.Combine(Application.persistentDataPath, keyFileName);

        if (File.Exists(path))
        {
            apiKey = File.ReadAllText(path).Trim();
            Debug.Log("✅ Groq API Key loaded.");
        }
        else
        {
            apiKey = null; // important for fallback
        }
    }


    public async Task<string> SendMessageToGroq(string userMessage)
    {
        if (string.IsNullOrEmpty(apiKey))
        {
            return $"❌ No API key found.\n\nTo use AI, please create a file named `groq_key.txt` with your API key and place it here:\n{Application.persistentDataPath}";
        }

        // Add user's message to the history
        conversationHistory.Add(new Message { role = "user", content = userMessage });

        var data = new
        {
            model = "llama3-8b-8192",
            messages = conversationHistory,
            temperature = 0.7f
        };

        var json = JsonConvert.SerializeObject(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                var parsed = JObject.Parse(responseBody);
                string result = parsed["choices"][0]["message"]["content"].ToString();

                // Add AI's response to the history
                conversationHistory.Add(new Message { role = "assistant", content = result });

                return result;
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Groq request failed: " + e.Message);
            return "⚠️ Error contacting Groq API.";
        }
    }
    public void ResetConversation()
    {
        conversationHistory.Clear();
    }


}

[System.Serializable]
public class GroqResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string role;
    public string content;
}
