using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class PhraseChecker : MonoBehaviour
{
    public bool trackErrorRateAndAccuracy = false;

    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Image inputFieldBackground;

    private float startTime;
    private float taskTime;
    private int trialNumber;
    private float timePerPhrase;
    public int backspaceCount; // used in keyboard.cs
    private float typingSpeed;

    string phraseCheckerFilePath = @"/mnt/sdcard/typingPhraseData.csv";


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
        trialNumber = 0;
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
        trialNumber++;
        timePerPhrase = Time.time - startTime;
        typingSpeed = (inputField.text.Length / 5.0f) / (timePerPhrase / 60.0f);

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
        float errorRate = (float)errors / phrases.Count;
        float accuracy = (float)correctAnswers / phrases.Count;

        string data = $"{trialNumber}, {timePerPhrase}, {backspaceCount}, {typingSpeed}, {taskTime}, {errorRate}, {accuracy}";
        EyeTrackingRecorder.WriteDataToFile(phraseCheckerFilePath, data);
    }

    public void StartNewRound()
    {
        startTime = Time.time;
    }
}
