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


    public List<string> newNewphrases = new List<string> {
    "birds fly south in the winter",
    "a cat chased the curious mouse",
    "sunrise marks the beginning of the day",
    "trees shed their leaves in autumn",
    "at night the stars shine brightly",
    "following the tracks, the train travels swiftly",
    "dogs often bark at strangers",
    "through the open sky flies a solitary bird",
    "children often play in parks",
    "fish swim swiftly in the pond",
    "spring brings blooming flowers",
    "bees gather nectar from many flowers",
    "bright is the moon in the night sky",
    "many books are resting on the shelf",
    "an artist is painting a beautiful scene",
    "across the bridge the city can be seen",
    "students study diligently for their exams",
    "on the table there is a cup of coffee",
    "green hills roll under the blue sky",
    "she wore a white dress to the party",
    "the old man walks slowly along the path",
    "in the kitchen, someone is cooking dinner",
    "joggers run around the park in the morning",
    "a red balloon floats in the clear sky",
    "the football game attracted many spectators",
    "a young boy is flying a kite",
    "beneath the waves marine life thrives",
    "the library was filled with quiet whispers",
    "on a canvas paint comes to life",
    "the mountain range is beautifully outlined at sunset"
};

    public List<int> phraseIndices = new List<int>();  

    private int currentPhraseIndex;
    private int errors;
    private int correctAnswers;

    void OnEnable()
    {
        eyeTrackingRecorder.currentTask = "PhraseCheck";

        // Fill the phraseIndices with values from 0 to newNewphrases.Count - 1
        phraseIndices.Clear();
        for (int i = 0; i < newNewphrases.Count; i++)
        {
            phraseIndices.Add(i);
        }

        currentPhraseIndex = 0;
        errors = 0;
        correctAnswers = 0;
        trialNumber = 0;
        UpdateDisplayText();
        //InvokeRepeating(nameof(CheckInput), 1f, 1f);
        startTime = (float)TimeManager.Instance.CurrentTime;
    }

    void OnDisable()
    {
        WriteBufferedDataToFile();
    }

    public void CheckInput()
    {
        if (inputField.text == newNewphrases[currentPhraseIndex])
        {
            correctAnswers++;
        }
        else if (inputField.text.Length == newNewphrases[currentPhraseIndex].Length)
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
        timePerPhrase = (float)TimeManager.Instance.CurrentTime - startTime;
        typingSpeed = (inputField.text.Length / 5.0f) / (timePerPhrase / 60.0f);

        inputField.text = "";

        // Remove the current index from the list
        phraseIndices.Remove(currentPhraseIndex);

        StartCoroutine(ShowFeedback());

        if (phraseIndices.Count > 0)
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

    public void UpdateDisplayText()
    {
        // Select a random index from the list of remaining indices
        currentPhraseIndex = phraseIndices[Random.Range(0, phraseIndices.Count)];
        displayText.text = newNewphrases[currentPhraseIndex];
    }

    public IEnumerator ShowFeedback()
    {
        inputFieldBackground.color = Color.green;
        yield return new WaitForSeconds(0.50f);
        inputFieldBackground.color = Color.white;
    }

    public void CalculateErrorRateAndAccuracy()
    {
        taskTime = (float)TimeManager.Instance.CurrentTime - startTime;
        float errorRate = (float)errors / newNewphrases.Count;
        float accuracy = (float)correctAnswers / newNewphrases.Count;

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
        startTime = (float)TimeManager.Instance.CurrentTime;
    }
}
