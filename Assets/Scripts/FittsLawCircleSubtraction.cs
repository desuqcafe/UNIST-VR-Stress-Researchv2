// using UnityEngine;
// using TMPro;

// public class FittsLawCircleSubtraction : MonoBehaviour
// {
//     public GameObject spherePrefab;
//     public Canvas canvas;
//     public TMP_Text sphereTextPrefab;

//     private int sphereCount = 0;

//     void Start()
//     {
//         // Instantiate the first sphere prefab
//         GameObject sphere1 = Instantiate(spherePrefab);
//         SetSphereText(sphere1);

//         // Instantiate the second sphere prefab
//         GameObject sphere2 = Instantiate(spherePrefab);
//         SetSphereText(sphere2);
//     }

//     void SetSphereText(GameObject sphere)
//     {
//         // Increment the sphere count
//         sphereCount++;

//         // Get the TMP Text component on the sphere
//         TMP_Text sphereText = sphere.GetComponentInChildren<TMP_Text>();

//         // Assign the canvas and TMP Text prefab to the sphere
//         canvas = sphere.GetComponentInChildren<Canvas>();
//         sphereTextPrefab = sphereText;

//         // Update the TMP Text with the sphere count
//         sphereText.text = sphereCount.ToString();
//     }
// }


using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    // Prefab for the target game objects
    public GameObject targetPrefab;
    [SerializeField] private GameObject theHolder;

    // Radius of the circle
    public float radius = 0.65f;

    // Number of targets to create
    public int numTargets = 9;

    private bool hasRun = false;

    // List to keep track of the highlighted objects
    private List<int> highlightedObjects = new List<int>();

    // Array to store the values for the target objects
    public float[] values;

    // Current answer for the game
    private float currentAnswer;

    void Start()
    {
        PlacePosition();
        AssignFormulaValue();
    }

    private void AssignFormulaValue()
    {
        // Initialize the values array
        values = new float[numTargets];

        // Generate random near values for the eight elements
        int correctIndex = -1;
        float correctValue = 1022f;
        float minValue = correctValue - 13f;
        float maxValue = correctValue + 13f;
        List<int> availableIndices = new List<int>();

        for (int i = 0; i < numTargets; i++)
        {
            if (i == 0)
            {
                // Assign the correct value to the first element
                values[i] = correctValue;
                correctIndex = i;
            }
            else
            {
                // Generate a list of available indices
                if (i != correctIndex)
                {
                    availableIndices.Add(i);
                }
                // Generate a random near value for the current element
                float nearValue = Random.Range(minValue, maxValue);
                values[i] = nearValue;
            }
        }
        // Randomly select an index from the available indices for the correct value
        if (availableIndices.Count > 0)
        {
            correctIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            values[correctIndex] = correctValue;
        }

        // Set the current answer to the correct value
        currentAnswer = correctValue;
    }

private void PlacePosition()
{
    // Calculate the angle between each target
    float angle = 360f / numTargets;

    // Create the targets in a circular pattern
    for (int i = 0; i < numTargets; i++)
    {
        // Calculate the position of this target
        float yPos = radius * Mathf.Cos(angle * i * Mathf.Deg2Rad);
        float zPos = radius * Mathf.Sin(angle * i * Mathf.Deg2Rad);
        Vector3 position = new Vector3(0, yPos, zPos);

// Create the target game object and set its position
GameObject target = Instantiate(targetPrefab, position, Quaternion.identity);
target.transform.SetParent(transform);

// Get the FittsLawTarget script and set its value and valueText fields
FittsLawTarget fittsLawTarget = target.GetComponent<FittsLawTarget>();

if (fittsLawTarget != null) {
    Debug.Log("All Good");
    if (i >= values.Length)
    {
        Debug.LogError("Index out of bounds for values array: " + i);
        break;
    }
}

fittsLawTarget.value = values[i];
fittsLawTarget.valueText = target.GetComponentInChildren<TMP_Text>();

// Set the value text component to the near value
fittsLawTarget.valueText.text = values[i].ToString();
    }

    // Highlight the first target
    transform.GetChild(0).GetComponent<FittsLawCircle>().isHighlighted = true;
    highlightedObjects.Add(0);

    // Rotate the spawned object 90 degrees on the Y axis
    theHolder.transform.rotation = Quaternion.Euler(0, 90, 0);
}

public void CircleTouched(GameObject hoveredObject)
{
    if (hasRun)
    {
        return;
    }
    hasRun = true;

    theHolder = GameObject.Find("FittGenSubtraction");

    // Check if the selected object has the correct value
    int selectedObjectIndex = hoveredObject.transform.GetSiblingIndex();
    if (Mathf.Approximately(values[selectedObjectIndex], currentAnswer))
    {
        // Update the current answer
        currentAnswer -= 13f;

        // Generate random near values for the other objects using the new current answer as the center
        float minValue = currentAnswer - 13f;
        float maxValue = currentAnswer + 13f;
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < numTargets; i++)
        {
            if (i != selectedObjectIndex)
            {
                // Generate a list of available indices
                availableIndices.Add(i);

                // Generate a random near value for the current element
                float nearValue = Random.Range(minValue, maxValue);
                values[i] = nearValue;

                // Set the value of the target object
                FittsLawTarget fittsLawTarget = transform.GetChild(i).GetComponent<FittsLawTarget>();
                fittsLawTarget.value = nearValue;

                // Update the value text of the target object on the canvas
                fittsLawTarget.valueText.text = nearValue.ToString();
            }
        }

        // Randomly select an index from the available indices for the correct value
        if (availableIndices.Count > 0)
        {
            int correctIndex = availableIndices[Random.Range(0, availableIndices.Count)];
            values[correctIndex] = currentAnswer;

            // Set the value of the target object
            FittsLawTarget fittsLawTarget = transform.GetChild(correctIndex).GetComponent<FittsLawTarget>();
            fittsLawTarget.value = currentAnswer;

            // Update the value text of the target object on the canvas
            fittsLawTarget.valueText.text = currentAnswer.ToString();
        }
    }

    // Unhighlight the current target
    FittsLawCircle hoveredCircle = hoveredObject.GetComponent<FittsLawCircle>();
    hoveredCircle.isHighlighted = false;
    hoveredCircle.renderer.material = hoveredCircle.originalMaterial;
    hoveredCircle.runOnce = false;

    // Select a new target
    int nextIndex = Random.Range(0, numTargets);
    while (highlightedObjects.Contains(nextIndex))
    {
        nextIndex = Random.Range(0, numTargets);
    }

    // Add the index of the next selected object to the list of highlighted indices
    highlightedObjects.Add(nextIndex);

    // Highlight the selected target
    FittsLawCircle nextCircle = transform.GetChild(nextIndex).GetComponent<FittsLawCircle>();
    nextCircle.isHighlighted = true;

    hasRun = false;
}

public GameObject GetHighlightedChild()
{
    for (int i = 0; i < theHolder.transform.childCount; i++)
    {
        FittsLawCircle target = theHolder.transform.GetChild(i).GetComponent<FittsLawCircle>();
        if ( target.isHighlighted)
{
// Return the child object that has isHighlighted set to true
return target.gameObject;
}
}
// If no highlighted child is found, return null
return null;
}
}