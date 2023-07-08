using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;

public class TranscriptionChecker : MonoBehaviour, IKeyboardInputHandler
{
    private Keyboard keyboard;
    private List<int> indices;
    private List<int> shuffledIndices;

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

        // Initialize the indices based on the length of the list of sentences
        indices = Enumerable.Range(0, newPhrases.Count).ToList();
    }

    public EyeTrackingRecorder eyeTrackingRecorder; // reference the EyeTrackingRecorder script


    public bool trackErrorRateAndAccuracy = true;

    public TMP_InputField inputField;
    public TextMeshProUGUI displayText;
    public Image inputFieldBackground;

    private float startTime;
    private float taskTime;

    public List<string> newPhrases = new List<string> {
    "i enjoy drinking a warm cup of coffee while reading the news",
    "the weather today is quite pleasant with a clear sky and a gentle breeze",
    "for dinner i plan to cook pasta with a side of garlic bread",
    "i need to remember to buy milk and eggs from the grocery store on my way home",
    "in my spare time, I like to read books and play video games",
    "the city lights at night are a beautiful sight to behold",
    "i like to go for a long walk in the park and one of my favorite hobbies is gardening",
    "it is important to stay hydrated throughout the day",
    "reading helps me relax and unwind at the end of the day",
    "on weekends i enjoy exploring new places and tomorrow I need to wake up early for a meeting",
    "a healthy breakfast is a great way to start the day",
    "i enjoy the calm and serenity of early mornings and my dog loves to play fetch in the park"
};

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
        startTime = (float)TimeManager.Instance.CurrentTime;

        // Shuffle the indices at the beginning of each round
        shuffledIndices = new List<int>(indices);
        Shuffle(shuffledIndices);
        currentPhraseIndex = shuffledIndices[0];
        shuffledIndices.RemoveAt(0);
        UpdateDisplayText();
    }

    private void Shuffle(List<int> list)
    {
        int n = list.Count;
        System.Random rnd = new System.Random();
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public void CheckInput()
    {
        // Trim leading/trailing spaces and ignore case during comparison
        if (inputField.text.Trim().ToLower() == newPhrases[currentPhraseIndex].Trim().ToLower())
        {
            correctAnswers++;
        }
        else if (inputField.text.Length == newPhrases[currentPhraseIndex].Length)
        {
            errors++;
        }

        // Regardless of whether input is correct or not, clear the input field
        inputField.text = "";

        // Show feedback
        StartCoroutine(ShowFeedback());

        // Start a new round
        StartNewRound();

        if (trackErrorRateAndAccuracy)
        {
            CalculateErrorRateAndAccuracy();
        }

        WriteBufferedDataToFile();
    }



    public void NextPhrase()
    {
        // CheckInput() method now takes care of moving to the next phrase
    }

    public void UpdateDisplayText()
    {
        displayText.text = newPhrases[currentPhraseIndex];
    }

    public IEnumerator ShowFeedback()
    {
        //inputFieldBackground.color = Color.green;
        yield return new WaitForSeconds(0.50f);
        //inputFieldBackground.color = Color.white;
    }

    public void CalculateErrorRateAndAccuracy()
    {
        taskTime = (float)TimeManager.Instance.CurrentTime - startTime;
        timePerPhrase = taskTime / newPhrases.Count;
        typingSpeed = (float)inputField.text.Length / taskTime;

        float errorRate = (float)errors / newPhrases.Count;
        float accuracy = (float)correctAnswers / newPhrases.Count;

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
        if (shuffledIndices.Count > 0)
        {
            // Select a new phrase
            currentPhraseIndex = shuffledIndices[0];
            shuffledIndices.RemoveAt(0);
            // Reset the display text
            UpdateDisplayText();
        }
        else
        {
            // No more phrases left, clear the display text
            displayText.text = "";
        }
        // Reset the start time
        startTime = (float)TimeManager.Instance.CurrentTime;
    }

    public void IncrementBackspaceCount()
    {
        backspaceCount++;
    }
}
