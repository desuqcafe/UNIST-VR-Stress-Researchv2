using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.IO;
using System.Text;


public class FittsLawCircleSubtraction : MonoBehaviour
{


    private StringBuilder dataBuffer = new StringBuilder();

    private string folderPath;
    private string timeStamp;
    private string mathTaskStressFilePath;

    public EyeTrackingRecorder eyeTrackingRecorder; // reference the EyeTrackingRecorder script

    public bool trackErrorRateAndAccuracy = false;

    public GameObject spawnCenterObject;

    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;

    List<TMP_Text> sphereTexts = new List<TMP_Text>();
    public List<GameObject> spheresList = new List<GameObject>(); // list to store all spawned spheres

    private int sphereCount = 0;
    private int correctAnswer = 1022;
    public int score = 0;
    private int roundCount = 0;
    private int maxRounds = 1000;

    private float startTime;
    private float taskTime;
    private int errors = 0;
    private int correctAnswers = 0;

    public static FittsLawCircleSubtraction Instance; // singleton

    // Additional data points
    private List<float> roundTimes = new List<float>();
    private List<int> incorrectClicksPerRound = new List<int>();
    private List<float> correctSphereGazeDurations = new List<float>();
    private List<float> incorrectSphereGazeDurations = new List<float>();
    
    private List<float> errorRates = new List<float>();
    private List<float> accuracies = new List<float>();

    public AudioClip wrongAnswerAudio; // The audio clip to play when wrong answer is given
    private AudioSource audioSource; // The audio source to play the clip

    private void Awake()
    {
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        mathTaskStressFilePath = Path.Combine(folderPath, "simpleFittData_" + timeStamp + ".csv");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        audioSource = GetComponent<AudioSource>();

    }

    void OnEnable()
    {
        eyeTrackingRecorder.currentTask = "MathTaskStress";

        startTime = (float)TimeManager.Instance.CurrentTime;
        SpawnSpheres();
    }

    void OnDisable()
    {
        // Cleanup actions, such as saving the game data and resetting necessary variables
        EndGame();

        // Reset state variables
        ResetState();
    }

    private void ResetState()
    {
        score = 0;
        roundCount = 0;
        errors = 0;
        correctAnswers = 0;
        correctAnswer = 1022;

        // Clear all the lists
        sphereTexts.Clear();
        spheresList.Clear();
        roundTimes.Clear();
        incorrectClicksPerRound.Clear();
        correctSphereGazeDurations.Clear();
        incorrectSphereGazeDurations.Clear();
        errorRates.Clear();
        accuracies.Clear();
    }

    void SetSphereText(GameObject sphere, string text)
    {
        sphereCount++;
        TMP_Text sphereText = sphere.GetComponentInChildren<TMP_Text>();

        if (sphereText != null)
        {
            sphereTexts.Add(sphereText);
            sphereText.text = text;
        }
        else
        {
            // Wrong answer spheres
            sphereText = sphere.AddComponent<TMP_Text>();
            int min = correctAnswer - 15;
            int max = correctAnswer + 15;
            int wrongAnswer = Random.Range(min, max);
            while (wrongAnswer == correctAnswer)
            {
                wrongAnswer = Random.Range(min, max);
            }
            sphereText.text = wrongAnswer.ToString();
        }
    }

    void SpawnSpheres()
    {
        float radius = 0.5f;

        Vector3 center = spawnCenterObject.transform.position;

        for (int i = 0; i < 9; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            spheresList.Add(sphere); // add to list to watch

            Debug.Log("Sphere instantiated: " + sphere.name);

            // Wrong answer spheres
            int min = correctAnswer - 15;
            int max = correctAnswer + 15;
            int wrongAnswer = Random.Range(min, max);
            while (wrongAnswer == correctAnswer)
            {
                wrongAnswer = Random.Range(min, max);
            }
            SetSphereText(sphere, wrongAnswer.ToString());

            float angle = (float)(i + 1) / 9f * Mathf.PI * 2f;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            sphere.transform.position = new Vector3(x + center.x, y + center.y, center.z);
        }

        // Randomly select a sphere to be the correct one
        int correctIndex = Random.Range(0, sphereTexts.Count);
        if (sphereTexts[correctIndex].transform.parent.parent.gameObject.TryGetComponent<FittsLawTarget>(out FittsLawTarget fittsLawTarget))
        {
            fittsLawTarget.correctSphere = true;
            sphereTexts[correctIndex].text = correctAnswer.ToString();
        }
        else
        {
            Debug.Log(sphereTexts[correctIndex].gameObject);
            Debug.Log(sphereTexts[correctIndex].transform.parent.gameObject);
            Debug.Log(sphereTexts[correctIndex].transform.parent.parent.gameObject);
            Debug.LogError("FittsLawTarget component not found on the sphere object.");
        }

        Debug.Log("Spheres Finished Spawning" + "\nRound Count: " + roundCount);
        Debug.Log("CorrectAnswer: " + correctAnswer);
    }

    public void ContinueGame(GameObject correctSphere)
    {
        roundCount++;
        correctAnswers++;
        correctAnswer -= 13;
        roundTimes.Add(taskTime);
        incorrectClicksPerRound.Add(errors);

        if (trackErrorRateAndAccuracy)
        {
            CalculateErrorRateAndAccuracy();
        }

        float correctSphereGazeDuration = (correctSphereGazeDurations.Count > roundCount - 1) ? correctSphereGazeDurations[roundCount - 1] : 0;
        float incorrectSphereGazeDuration = (incorrectSphereGazeDurations.Count > roundCount - 1) ? incorrectSphereGazeDurations[roundCount - 1] : 0;
        float errorRate = (errorRates.Count > roundCount - 1) ? errorRates[roundCount - 1] : 0;
        float accuracy = (accuracies.Count > roundCount - 1) ? accuracies[roundCount - 1] : 0;

        AppendDataToBuffer(roundCount, taskTime, errors, correctSphereGazeDuration, incorrectSphereGazeDuration, errorRate, accuracy);

        StartNewRound(); // Reset the startTime for the next round
    }



    public void StartNewRound()
    {
        if (roundCount < maxRounds)
        {
            startTime = (float)TimeManager.Instance.CurrentTime;
            errors = 0;

            // Delete previous spheres
            foreach (GameObject sphere in spheresList)
            {
                Destroy(sphere);
            }
            spheresList.Clear();
            sphereTexts.Clear();

            // Spawn new spheres
            SpawnSpheres();
        }
        else
        {
            EndGame();
        }
    }

    public void EndGame()
    {
        Debug.Log("Game Over!");

        // Save the additional data points to a file
        SaveData();
    }

    void SaveData()
    {
        string header = "Round,Time,Incorrect Clicks,Correct Sphere Gaze Duration,Incorrect Sphere Gaze Duration,Error Rate,Accuracy";

        EyeTrackingRecorder.Instance.WriteDataToFileAsync(mathTaskStressFilePath, new List<string> { header });
        EyeTrackingRecorder.Instance.WriteDataToFileAsync(mathTaskStressFilePath, new List<string> { dataBuffer.ToString() });

        Debug.Log("Data saved to: " + mathTaskStressFilePath);
    }




    void AppendDataToBuffer(int round, float time, int incorrectClicks, float correctGazeDuration, float incorrectGazeDuration, float errorRate, float accuracy)
    {
        dataBuffer.AppendLine($"{round},{time},{incorrectClicks},{correctGazeDuration},{incorrectGazeDuration},{errorRate},{accuracy}");
    }



    public void RegisterError(GameObject incorrectSphere)
    {
        errors++;
        score--; // decrease the score
        audioSource.PlayOneShot(wrongAnswerAudio); // play the sound
        correctAnswer = 1022; // reset the correct answer to 1022
        StartNewRound(); // reset the game

    }

    void CalculateErrorRateAndAccuracy()
    {
        float errorRate = (float)errors / (float)(errors + correctAnswers) * 100f;
        float accuracy = (float)correctAnswers / (float)(errors + correctAnswers) * 100f;

        errorRates.Add(errorRate);
        accuracies.Add(accuracy);

        Debug.Log($"Error Rate: {errorRate}% | Accuracy: {accuracy}%");
    }

    public void RegisterGazeDuration(bool isCorrectSphere, float gazeDuration)
    {
        if (isCorrectSphere)
        {
            correctSphereGazeDurations.Add(gazeDuration);
        }
        else
        {
            incorrectSphereGazeDurations.Add(gazeDuration);
        }
    }

    void Update()
    {
        taskTime = (float)TimeManager.Instance.CurrentTime - startTime;
    }
}