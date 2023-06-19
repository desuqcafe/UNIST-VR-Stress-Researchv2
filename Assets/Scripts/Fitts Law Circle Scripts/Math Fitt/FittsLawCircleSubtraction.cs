using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    public EyeTrackingRecorder eyeTrackingRecorder; // reference the EyeTrackingRecorder script

    string mathTaskStressFilePath = @"/mnt/sdcard/mathTaskStressData.csv";

    public bool trackErrorRateAndAccuracy = false;

    public GameObject spawnCenterObject;

    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;

    List<TMP_Text> sphereTexts = new List<TMP_Text>();
    private List<GameObject> spheresList = new List<GameObject>(); // list to store all spawned spheres

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

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        eyeTrackingRecorder.currentTask = "MathTaskStress";

        startTime = Time.time;
        SpawnSpheres();
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
        StartNewRound(); // Reset the startTime for the next round```
    }


    public void StartNewRound()
    {
        if (roundCount < maxRounds)
        {
            startTime = Time.time;
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
        string header = "Round,Time,Incorrect Clicks,Correct Sphere Gaze Duration,Incorrect Sphere Gaze Duration";
        string data = "";

        for (int i = 0; i < roundTimes.Count; i++)
        {
            data += $"{i + 1},{roundTimes[i]},{incorrectClicksPerRound[i]},{correctSphereGazeDurations[i]},{incorrectSphereGazeDurations[i]},{errorRates[i]},{accuracies[i]}\n";
        }

        EyeTrackingRecorder.WriteDataToFile(mathTaskStressFilePath, header);
        EyeTrackingRecorder.WriteDataToFile(mathTaskStressFilePath, data);

        Debug.Log("Data saved to: " + mathTaskStressFilePath);
    }

    public void RegisterError(GameObject incorrectSphere)
    {
        errors++;
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
        taskTime = Time.time - startTime;
    }
}