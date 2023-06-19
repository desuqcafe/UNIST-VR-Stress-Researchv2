using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class TranscriptionChecker : MonoBehaviour
{

    string transcriptionCheckFilePath = @"/mnt/sdcard/transcriptionCheckData.csv";

    public bool trackErrorRateAndAccuracy = false;

    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Image inputFieldBackground;

    private float startTime;
    private float taskTime;

    public List<string> phrases = new List<string> {
        "Wait to hear the sentence then type it",
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it", 
        "Wait to hear the sentence then type it"  
    };

    public List<string> phrasesSecret = new List<string> {
        "the dog jumped the moon",
        "the dog jumped the moon", 
        "the dog jumped the moon", 
        "the dog jumped the moon", 
        "the dog jumped the moon", 
        "the dog jumped the moon", 
        "the dog jumped the moon", 
        "Wait to hear the sentence then type it"  
    };

    private int currentPhraseIndex;
    private int errors;
    private int correctAnswers;
    private int backspaceCount;

    // Add new variables
    private int trialNumber;
    private float timePerPhrase;
    private float typingSpeed;

    public void Start()
    {
        currentPhraseIndex = 0;
        errors = 0;
        correctAnswers = 0;
        backspaceCount = 0;
        trialNumber = 1;
        UpdateDisplayText();
        startTime = Time.time;
    }

    public void CheckInput()
    {
        if (inputField.text == phrasesSecret[currentPhraseIndex])
        {
            correctAnswers++;
        }
        else if (inputField.text.Length == phrases[currentPhraseIndex].Length)
        {
            errors++;
        }
    }

    public void NextPhrase()
    {
        CheckInput();
        inputField.text = "";
        currentPhraseIndex++;

        StartCoroutine(ShowFeedback());

        if (currentPhraseIndex < phrases.Count)
        {
            UpdateDisplayText();

            if (trackErrorRateAndAccuracy)
            {
                CalculateErrorRateAndAccuracy();
            }
            StartNewRound();
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
        taskTime = Time.time - startTime;
        timePerPhrase = taskTime / phrases.Count;
        typingSpeed = (float)inputField.text.Length / taskTime;

        float errorRate = (float)errors / phrases.Count;
        float accuracy = (float)correctAnswers / phrases.Count;

        string data = $"{trialNumber}, {timePerPhrase}, {backspaceCount}, {typingSpeed}, {taskTime}, {errorRate}, {accuracy}";
        EyeTrackingRecorder.WriteDataToFile(transcriptionCheckFilePath, data);

        trialNumber++; // Increment trial number
        backspaceCount = 0; // Reset the backspace count after recording the data
    }

    public void StartNewRound()
    {
        startTime = Time.time;
    }

    public void IncrementBackspaceCount()
    {
        backspaceCount++;
    }
}
