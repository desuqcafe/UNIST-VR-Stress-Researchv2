using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class OpenAIExample : MonoBehaviour
{
    public TextMeshProUGUI outputText;
    private string prompt = "Translate the following English text to Korean: 'Hello, world!'";

    // OpenAI API URL
    private string url = "https://api.openai.com/v4/engines/text-davinci-003/completions";

    // Your OpenAI API Key
    private string apiKey = "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";

    IEnumerator Start()
    {
        // Formulate the request
        var requestBody = new
        {
            prompt = this.prompt,
            max_tokens = 60
        };

        string jsonBody = JsonUtility.ToJson(requestBody);

        // Create the web request
        UnityWebRequest www = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("Authorization", "Bearer " + apiKey);

        // Send the request and wait for a response
        yield return www.SendWebRequest();

        // Handle the response
        if (www.result == UnityWebRequest.Result.Success)
        {
            string responseBody = www.downloadHandler.text;
            Response response = JsonUtility.FromJson<Response>(responseBody);

            // Use the output in your TextMesh Pro text
            outputText.text = response.choices[0].text.Trim();
        }
        else
        {
            Debug.Log("Error: " + www.error);
        }
    }

    [System.Serializable]
    public class Response
    {
        public List<Choice> choices;
    }

    [System.Serializable]
    public class Choice
    {
        public string text;
    }
}
