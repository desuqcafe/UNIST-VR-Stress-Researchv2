using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
// using UnityEngine.Events;

public class StroopRoomController : MonoBehaviour
{

    private string folderPath;
    private string timeStamp;
    private string stroopRoomDataFilePath;
    public int totalRounds = 50; // Set a default value or make it configurable in the inspector


    private List<string> stroopRoomDataBuffer = new List<string>();

    private void Awake()
    {
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        stroopRoomDataFilePath = Path.Combine(folderPath, "stroopRoomData_" + timeStamp + ".csv");
    }

    public EyeTrackingRecorder eyeTrackingRecorder; // reference the EyeTrackingRecorder script

    public bool trackErrorRateAndAccuracy = false;
    private int trialNumber;
    public GameObject[] objects;
    public Material[] materials;
    public TextMeshProUGUI word;

    private int matchedColorIndex;
    private int textColorIndex;
    private int errors;
    private int correctAnswers;

    // Add variables to track startTime and taskTime
    private float startTime;
    private float taskTime;

    private float trialStartTime;
    private float responseTime;


    void Start()
    {
        eyeTrackingRecorder.currentTask = "StroopRoom";

        RandomizeColors();
    }

    void RandomizeColors()
    {
        List<int> colorIndices = new List<int>();

        // Fill the list with indices from 0 to the number of materials - 1
        for (int i = 0; i < materials.Length; i++)
        {
            colorIndices.Add(i);
        }

        // Shuffle the list
        for (int i = 0; i < colorIndices.Count; i++)
        {
            int temp = colorIndices[i];
            int randomIndex = Random.Range(i, colorIndices.Count);
            colorIndices[i] = colorIndices[randomIndex];
            colorIndices[randomIndex] = temp;
        }

        // Assign random materials to the objects
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].GetComponent<MeshRenderer>().material = materials[colorIndices[i]];
        }

        // Choose a random object to match the text color
        matchedColorIndex = Random.Range(0, objects.Length);
        Color matchedColor = objects[matchedColorIndex].GetComponent<MeshRenderer>().material.color;

        // Set the text to display the name of the color
        textColorIndex = (matchedColorIndex + 1) % materials.Length;
        word.text = materials[textColorIndex].name;

        // Set the text color to the matched color
        word.color = matchedColor;

        // Set the trial start time
        trialStartTime = Time.time;
    }

    public void ObjectSelected(int index)
    {
        trialNumber++; // Increment the trial number

        int clickedIndex = objects[index].GetComponent<ObjectIndex>().index;

        responseTime = Time.time - trialStartTime;


        if (clickedIndex == matchedColorIndex)
        {
            correctAnswers++;
        }
        else
        {
            errors++;
        }

        if (correctAnswers + errors >= totalRounds)
        {
            WriteBufferedDataToFile(); // Add this line to write the buffered data
        }

        else
        {
            RandomizeColors();

            if (trackErrorRateAndAccuracy)
            {
                CalculateErrorRateAndAccuracy();
            }
            
            StartNewRound(); // Reset the startTime for the next round
        }

        Debug.Log("Clicked Index: " + clickedIndex);
        Debug.Log("Matched Index: " + matchedColorIndex);
        Debug.Log("Total Rounds: " + totalRounds);
    }

    private void CalculateErrorRateAndAccuracy()
    {
        // Calculate taskTime when the task is completed
        taskTime = Time.time - startTime;

        float errorRate = (float)errors / totalRounds;
        float accuracy = (float)correctAnswers / totalRounds;

        string data = $"{trialNumber}, {taskTime}, {responseTime}, {errorRate}, {accuracy}";

        stroopRoomDataBuffer.Add(data);
    }

    private void WriteBufferedDataToFile()
    {
        if (stroopRoomDataBuffer.Count > 0)
        {
            eyeTrackingRecorder.WriteDataToFileAsync(stroopRoomDataFilePath, stroopRoomDataBuffer);

            // Clear the buffer
            stroopRoomDataBuffer.Clear();
        }
    }



    public void StartNewRound()
    {
        startTime = Time.time;
    }
}