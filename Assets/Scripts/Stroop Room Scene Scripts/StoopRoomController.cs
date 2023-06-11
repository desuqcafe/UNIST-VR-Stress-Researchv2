using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
// using UnityEngine.Events;

public class StroopRoomController : MonoBehaviour
{

    // [System.Serializable]
    // public class ObjectSelectedEvent : UnityEvent<GameObject> { }

    // public ObjectSelectedEvent OnCorrectObjectSelected;
    // public ObjectSelectedEvent OnIncorrectObjectSelected;

    public bool trackErrorRateAndAccuracy = false;



    public GameObject[] objects;
    public Material[] materials;
    public TextMeshProUGUI word;

    private int matchedColorIndex;
    private int textColorIndex;
    private int errors;
    private int correctAnswers;
    private int totalRounds; // Add a variable to set the number of rounds for the task

    // Add variables to track startTime and taskTime
    private float startTime;
    private float taskTime;

    void Start()
    {
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
    }

    public void ObjectSelected(int index)
    {
        int clickedIndex = objects[index].GetComponent<ObjectIndex>().index;

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
            //
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
    }

    private void CalculateErrorRateAndAccuracy()
    {

        // Calculate taskTime when the task is completed
        taskTime = Time.time - startTime;

        float errorRate = (float)errors / totalRounds;
        float accuracy = (float)correctAnswers / totalRounds;

        // Debug.Log("Error Rate: " + errorRate);
        // Debug.Log("Accuracy: " + accuracy);
        // Debug.Log("Task Time: " + taskTime);

        string filePath = @"C:\Users\INTERACTIONS\Desktop\StroopRoomData.csv";
        string data = $"{taskTime}, {errorRate}, {accuracy}";
        EyeTrackingRecorder.WriteDataToFile(filePath, data);
    }

    public void StartNewRound()
    {
        startTime = Time.time;
    }
}