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


    public GameObject[] objects;
    public Material[] materials;
    public TextMeshProUGUI word;

    private int matchedColorIndex;
    private int textColorIndex;

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
}