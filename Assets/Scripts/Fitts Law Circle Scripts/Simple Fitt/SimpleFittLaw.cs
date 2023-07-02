using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Collections;

public class SimpleFittLaw : MonoBehaviour
{

    private string folderPath;
    private string timeStamp;
    private string fittTaskBaseFilePath;

    private List<string> simpleFittDataBuffer = new List<string>();


    private void Awake()
    {
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        fittTaskBaseFilePath = Path.Combine(folderPath, "simpleFittData_" + timeStamp + ".csv");
    }
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

    // Additional data points
    private List<float> roundTimes = new List<float>();
    private List<int> incorrectClicksPerRound = new List<int>();
    private List<float> correctSphereGazeDurations = new List<float>();
    private List<float> incorrectSphereGazeDurations = new List<float>();
    private List<float> errorRates = new List<float>();
    private List<float> accuracies = new List<float>();

    private int lastCorrectSphereIndex = -3;  // Initialize this to -3 so the first time it does not affect the choice



    void OnEnable()
    {
        eyeTrackingRecorder.currentTask = "FittTaskBase";

        // Initialize your variables here.
        errors = 0;
        correctAnswers = 0;
        totalRounds = 1000; // You may want to set this as a constant or as a public variable for flexibility.
        startTime = 0f;
        taskTime = 0f;
        simpleFittDataBuffer.Clear();
        roundTimes.Clear();
        incorrectClicksPerRound.Clear();
        correctSphereGazeDurations.Clear();
        incorrectSphereGazeDurations.Clear();
        errorRates.Clear();
        accuracies.Clear();

        // Call your methods to setup the task
        SpawnSpheres();
        StartNewRound();
    }

    void OnDisable()
    {
        EndGame();
    }


    IEnumerator SetSphereMaterialDelayed(GameObject sphere, bool isCorrect)
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

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

        // Exclude the adjacent spheres of the last correct sphere
        List<int> possibleIndices = new List<int>();
        for (int i = 0; i < sphereCount; i++)
        {
            if (i < lastCorrectSphereIndex - 1 || i > lastCorrectSphereIndex + 1)
            {
                possibleIndices.Add(i);
            }
        }

        int correctSphereIndex = possibleIndices[Random.Range(0, possibleIndices.Count)];
        lastCorrectSphereIndex = correctSphereIndex;

        for (int i = 0; i < sphereCount; i++)
        {
            bool isCorrect = (i == correctSphereIndex);
            GameObject sphere = Instantiate(spherePrefab, transform);

            StartCoroutine(SetSphereMaterialDelayed(sphere, isCorrect));

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
            List<GameObject> objectsToDestroy = new List<GameObject>();
            foreach (Transform child in transform)
            {
                objectsToDestroy.Add(child.gameObject);
            }
            foreach (var obj in objectsToDestroy)
            {
                Destroy(obj);
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

        string data = $"{correctAnswers + errors}, {taskTime}, {errors}, {errorRate}, {accuracy}";
        simpleFittDataBuffer.Add(data);
    }

    private void WriteBufferedDataToFile()
    {
        if (simpleFittDataBuffer.Count > 0)
        {
            eyeTrackingRecorder.WriteDataToFileAsync(fittTaskBaseFilePath, simpleFittDataBuffer);
            // Clear the buffer
            simpleFittDataBuffer.Clear();
        }
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
        string header = "Round,Time,Errors,Error Rate,Accuracy";
        eyeTrackingRecorder.WriteDataToFileAsync(fittTaskBaseFilePath, new List<string> { header });

        WriteBufferedDataToFile(); // Write the buffered data to the file
    }



}
