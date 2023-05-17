using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PhraseChecker : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;

    private List<string> phrases = new List<string> { 
        "hello my name is good", 
        "what is up", 
        "how are you", 
        "what is your plan today" 
    };

    private int currentPhraseIndex;

    private void Start()
    {
        currentPhraseIndex = 0;
        // Display the first phrase to type
        UpdateDisplayText();

        // Start checking the input field every second
        InvokeRepeating(nameof(CheckInput), 1f, 1f);
    }

    public void CheckInput()
    {
        if (inputField.text == phrases[currentPhraseIndex])
        {
            // Clear the input field
            inputField.text = "";

            // Update the current phrase index
            currentPhraseIndex++;

            // If there are still phrases left, update the display text with the new phrase
            if (currentPhraseIndex < phrases.Count)
            {
                UpdateDisplayText();
            }
            else
            {
                // If there are no phrases left, you can handle this however you like
                // This example simply clears the display text
                displayText.text = "";
            }
        }
    }

    private void UpdateDisplayText()
    {
        displayText.text = phrases[currentPhraseIndex];
    }
}
