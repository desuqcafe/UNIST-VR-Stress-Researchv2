using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;


public class PhraseChecker : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Image inputFieldBackground; // New: reference to the image that will change color

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

            // Show feedback
            StartCoroutine(ShowFeedback());

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

     // New: Coroutine to briefly change the image color to green
    private IEnumerator ShowFeedback()
    {
        inputFieldBackground.color = Color.green;
        yield return new WaitForSeconds(0.50f); // wait for 0.5 seconds
        inputFieldBackground.color = Color.white;
    }
}
