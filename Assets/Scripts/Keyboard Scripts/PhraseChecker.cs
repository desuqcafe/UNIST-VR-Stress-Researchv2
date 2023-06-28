using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;

public class PhraseChecker : MonoBehaviour, IKeyboardInputHandler
{

    private Keyboard keyboard;

    public void OnEnterPressed()
    {
        SubmitInput();
    }
    public void OnDeletePressed()
    {
        backspaceCount++;
    }

    private string folderPath;
    private string timeStamp;
    private string phraseCheckerFilePath;

    private List<string> phraseCheckerDataBuffer = new List<string>();

    private void Awake()
    {
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        phraseCheckerFilePath = Path.Combine(folderPath, "transcriptionCheckData_" + timeStamp + ".csv");

        GameObject keyboardObject = GameObject.Find("KeyboardPhrase");
        keyboard = keyboardObject.GetComponent<Keyboard>();
        keyboard.SetInputHandler(this);
    }

    public EyeTrackingRecorder eyeTrackingRecorder; // reference the EyeTrackingRecorder script

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

    void OnEnable()
    {
        eyeTrackingRecorder.currentTask = "PhraseCheck";

        currentPhraseIndex = 0;
        errors = 0;
        correctAnswers = 0;
        trialNumber = 0;
        UpdateDisplayText();
        //InvokeRepeating(nameof(CheckInput), 1f, 1f);
        startTime = Time.time;
    }

    public void CheckInput()
    {
        if (inputField.text == phrases[currentPhraseIndex])
        {
            correctAnswers++;
        }
        else if (inputField.text.Length == phrases[currentPhraseIndex].Length)
        {
            errors++;
        }

        NextPhrase();
    }

    public void SubmitInput()
    {
        CheckInput();
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
            WriteBufferedDataToFile(); // Add this line
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
        phraseCheckerDataBuffer.Add(data);

        trialNumber++; // Increment trial number
        backspaceCount = 0; // Reset the backspace count after recording the data
    }


    private void WriteBufferedDataToFile()
    {
        if (phraseCheckerDataBuffer.Count > 0)
        {
            eyeTrackingRecorder.WriteDataToFileAsync(phraseCheckerFilePath, phraseCheckerDataBuffer);

            // Clear the buffer
            phraseCheckerDataBuffer.Clear();

            // Reset the backspace count
            ResetBackspaceCount();
        }
    }


    public void ResetBackspaceCount()
    {
        backspaceCount = 0;
    }


    public void StartNewRound()
    {
        startTime = Time.time;
    }
}
