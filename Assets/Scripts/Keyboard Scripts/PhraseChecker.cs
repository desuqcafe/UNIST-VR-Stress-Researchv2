using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PhraseChecker : MonoBehaviour
{
    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Image inputFieldBackground;

    // Add variables to track startTime and taskTime
    private float startTime;
    private float taskTime;

    public List<string> phrases = new List<string> {
        "The boat floats on the water", 
        "The cat chased the mouse", 
        "The sun rises in the east", 
        "The leaves fall from the trees in autumn",
        "The stars shine brightly at night",
        "The train travels along the tracks" 
    };

    private int currentPhraseIndex;
    private int errors;
    private int correctAnswers;

    public void Start()
    {
        currentPhraseIndex = 0;
        errors = 0;
        correctAnswers = 0;
        UpdateDisplayText();
        InvokeRepeating(nameof(CheckInput), 1f, 1f);
        startTime = Time.time;
    }

    public void CheckInput()
    {
        if (inputField.text == phrases[currentPhraseIndex])
        {
            correctAnswers++;
            NextPhrase();
        }
        else if (inputField.text.Length == phrases[currentPhraseIndex].Length)
        {
            errors++;
            NextPhrase();
        }
    }

    public void NextPhrase()
    {
        inputField.text = "";
        currentPhraseIndex++;

        StartCoroutine(ShowFeedback());

        if (currentPhraseIndex < phrases.Count)
        {
            UpdateDisplayText();

            CalculateErrorRateAndAccuracy();
            StartNewRound(); // Reset the startTime for the next round
        }
        else
        {
            displayText.text = "";
        }
    }

    public void UpdateDisplayText()
    {
        displayText.text = phrases[currentPhraseIndex];
    }

    public IEnumerator ShowFeedback()
    {
        inputFieldBackground.color = Color.green;
        yield return new WaitForSeconds(0.50f);
        inputFieldBackground.color = Color.white;
    }

    public void CalculateErrorRateAndAccuracy()
    {
        // Calculate taskTime when the task is completed
        taskTime = Time.time - startTime;

        float errorRate = (float)errors / phrases.Count;
        float accuracy = (float)correctAnswers / phrases.Count;

        Debug.Log("Error Rate: " + errorRate);
        Debug.Log("Accuracy: " + accuracy);
        Debug.Log("Task Time: " + taskTime);
    }

    public void StartNewRound()
    {
        startTime = Time.time;
    }
}
