using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;


public class TranscriptionChecker : MonoBehaviour, IKeyboardInputHandler
{
    private Keyboard keyboard;

    public void OnEnterPressed()
    {
        CheckInput();
        NextPhrase();
    }
    public void OnDeletePressed()
    {
        backspaceCount++;
    }

    private string folderPath;
    private string timeStamp;
    private string transcriptionCheckFilePath;

    private List<string> transcriptionCheckDataBuffer = new List<string>();

    private void Awake()
    {
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        transcriptionCheckFilePath = Path.Combine(folderPath, "transcriptionCheckData_" + timeStamp + ".csv");

        GameObject keyboardObject = GameObject.Find("KeyboardTranscript");
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

    public List<string> phrases = new List<string> {
        "i enjoy drinking a warm cup of coffee while reading the news",
        "the weather today is quite pleasant with a clear sky and a gentle breeze",
        "for dinner i plan to cook pasta with a side of garlic bread.",
        "i need to remember to buy milk and eggs from the grocery store on my way home",
        "in my spare time, I like to read books and play video games",
        "tomorrow, I need to wake up early for a meeting",
        "the city lights at night are a beautiful sight to behold",
        "i like to go for a long walk in the park"
    };

    // public List<string> phrasesSecret = new List<string> {
    //     "the dog jumped the moon",
    //     "the dog jumped the moon", 
    //     "the dog jumped the moon", 
    //     "the dog jumped the moon", 
    //     "the dog jumped the moon", 
    //     "the dog jumped the moon", 
    //     "the dog jumped the moon", 
    //     "Wait to hear the sentence then type it"  
    // };

    private int currentPhraseIndex;
    private int errors;
    private int correctAnswers;
    private int backspaceCount;

    private int trialNumber;
    private float timePerPhrase;
    private float typingSpeed;

    private void OnEnable()
    {
        eyeTrackingRecorder.currentTask = "Transcription";

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
        if (inputField.text == phrases[currentPhraseIndex])
        {
            correctAnswers++;
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
                WriteBufferedDataToFile();
            }
            else
            {
                displayText.text = "";
            }
        }
        else if (inputField.text.Length == phrases[currentPhraseIndex].Length)
        {
            errors++;
            // Here you can decide what to do if the input is wrong
            inputField.text = "";
        }
    }

    public void NextPhrase()
    {
        // CheckInput() method now takes care of moving to the next phrase
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
        transcriptionCheckDataBuffer.Add(data);

        trialNumber++; // Increment trial number
        backspaceCount = 0; // Reset the backspace count after recording the data
    }

    private void WriteBufferedDataToFile()
    {
        if (transcriptionCheckDataBuffer.Count > 0)
        {
            eyeTrackingRecorder.WriteDataToFileAsync(transcriptionCheckFilePath, transcriptionCheckDataBuffer);

            // Clear the buffer
            transcriptionCheckDataBuffer.Clear();
        }
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
