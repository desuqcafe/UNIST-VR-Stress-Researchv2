using UnityEngine;
using System.Collections.Generic;

public class SimpleFittLaw : MonoBehaviour
{
    public EyeTrackingRecorder eyeTrackingRecorder;

    public GameObject spherePrefab;
    private int sphereCount = 9;

    public Material correctMaterial;
    public Material incorrectMaterial;

    private int errors;
    private int correctAnswers;
    private int totalRounds;
    private float startTime;
    private float taskTime;

    public bool trackErrorRateAndAccuracy = false;

    string fittTaskBaseFilePath = @"/mnt/sdcard/fittTaskBaseData.csv";

    // Additional data points
    private List<float> roundTimes = new List<float>();
    private List<int> incorrectClicksPerRound = new List<int>();
    private List<float> correctSphereGazeDurations = new List<float>();
    private List<float> incorrectSphereGazeDurations = new List<float>();
    private List<float> errorRates = new List<float>();
    private List<float> accuracies = new List<float>();


    void Start()
    {
        eyeTrackingRecorder.currentTask = "FittTaskBase";

        SpawnSpheres();
        errors = 0;
        correctAnswers = 0;
        totalRounds = 10;
        startTime = Time.time;
        SpawnSpheres();
    }

    void SetSphereMaterial(GameObject sphere, bool isCorrect)
    {
        Material material;

        if (isCorrect)
        {
            material = correctMaterial;
        }
        else
        {
            material = incorrectMaterial;
        }

        sphere.GetComponent<Renderer>().material = material;
    }

    void SpawnSpheres()
    {
        float radius = 0.5f;
        int correctSphereIndex = Random.Range(0, sphereCount);

        for (int i = 0; i < sphereCount; i++)
        {
            bool isCorrect = (i == correctSphereIndex);
            GameObject sphere = Instantiate(spherePrefab, transform);

            SetSphereMaterial(sphere, isCorrect);

            float angle = (float)(i + 1) / sphereCount * Mathf.PI * 2f;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            sphere.transform.localPosition = new Vector3(x, y, 0f);

            SimpleSphereHandler clickHandler = sphere.GetComponent<SimpleSphereHandler>();
            clickHandler.sphereSelector = this.gameObject;
            clickHandler.isCorrect = isCorrect;
        }
    }

    public void SphereClicked(bool isCorrect)
    {
        Debug.Log("Entered Sphere Clicked: " + isCorrect);

        if (isCorrect)
        {
            correctAnswers++;
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            if (correctAnswers + errors >= totalRounds)
            {
                EndGame();
            }
            else
            {
                SpawnSpheres();
                if (trackErrorRateAndAccuracy) 
                {
                    CalculateErrorRateAndAccuracy();
                }
                StartNewRound();
            }
        }
        else
        {
            errors++;
            if (trackErrorRateAndAccuracy) 
            {
                CalculateErrorRateAndAccuracy();
            }
            StartNewRound();
        }
    }

    private void CalculateErrorRateAndAccuracy()
    {
        taskTime = Time.time - startTime;
        float errorRate = (float)errors / totalRounds;
        float accuracy = (float)correctAnswers / totalRounds;

        errorRates.Add(errorRate);
        accuracies.Add(accuracy);
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

    public void StartNewRound()
    {
        startTime = Time.time;
    }

    void EndGame()
    {
        Debug.Log("Game Over!");

        // Save the additional data points to a file
        SaveData();
    }

    void SaveData()
    {
        string header = "Round,Time,Incorrect Clicks,Correct Sphere Gaze Duration,Incorrect Sphere Gaze Duration,Error Rate,Accuracy";
        string data = "";

        for (int i = 0; i < roundTimes.Count; i++)
        {
            data += $"{i + 1},{roundTimes[i]},{incorrectClicksPerRound[i]},{correctSphereGazeDurations[i]},{incorrectSphereGazeDurations[i]},{errorRates[i]},{accuracies[i]}\n";
        }

        EyeTrackingRecorder.WriteDataToFile(fittTaskBaseFilePath, header);
        EyeTrackingRecorder.WriteDataToFile(fittTaskBaseFilePath, data);
    }

}
