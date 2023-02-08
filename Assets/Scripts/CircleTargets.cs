using System.Collections.Generic;
using UnityEngine;

public class CircleTargets : MonoBehaviour
{
    // Prefab for the target game objects
    public GameObject targetPrefab;
    [SerializeField] private GameObject theHolder;

    // Radius of the circle
    public float radius = 0.65f;

    // Number of targets to create
    public int numTargets = 9;

    // Keeps track of the number of selections
    private int selectionsCount = 0;

    // Value to increase or decrease the scale by
    public float scaleValue = 0.5f;

    private bool hasRun = false;

    // List to keep track of the highlighted objects
    private List<int> highlightedObjects = new List<int>();

void Start()
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
    }

    // Highlight the first target
    transform.GetChild(0).GetComponent<FittsLawCircle>().isHighlighted = true;
    highlightedObjects.Add(0);

    // Rotate the spawned object 90 degrees on the Y axis
    theHolder.transform.rotation = Quaternion.Euler(0, 90, 0);

    //float currentRadius = radius;
}

public void CircleTouched(GameObject hoveredObject)
{

    if (hasRun)
    {
        return;
    }
    hasRun = true;

    theHolder = GameObject.Find("FittGenerator");

    if (selectionsCount == 9)
    {
        Vector3 newScale = theHolder.transform.localScale;
        newScale.x -= 0.5f;
        newScale.y -= 0.5f;
        newScale.z -= 0.5f;
        theHolder.transform.localScale = newScale;
        // reduce the scale of all children by 0.5
        foreach (Transform child in theHolder.transform)
        {
            child.localScale -= new Vector3(0.5f, 0.5f, 0.5f);
        }
    } else if (selectionsCount == 18) {
        Vector3 newScale = theHolder.transform.localScale;
        newScale.x += 0.25f;
        newScale.y += 0.25f;
        newScale.z += 0.25f;
        theHolder.transform.localScale = newScale;
        // reduce the scale of all children by 0.5
        foreach (Transform child in theHolder.transform)
        {
            child.localScale += new Vector3(0.25f, 0.25f, 0.25f);
        }
    }

    // // Check if the first target is highlighted
    // FittsLawCircle firstTarget = theHolder.transform.GetChild(0).GetComponent<FittsLawCircle>();
    // if (!firstTarget.isHighlighted)
    // {
    //     // if no highlighted object is found, break out of the loop and do not execute the rest of the code
    //     hasRun = false;
    //     return;
    // }

    bool loopStarted = false;

    // Check if any of the targets are highlighted
    for (int i = 0; i < theHolder.transform.childCount; i++)
    {
        FittsLawCircle enteredObject = hoveredObject.GetComponent<FittsLawCircle>();
        FittsLawCircle target = theHolder.transform.GetChild(i).GetComponent<FittsLawCircle>();

        if (target.isHighlighted == enteredObject.isHighlighted && !loopStarted)
        {

            loopStarted = true;

            // Unhighlight the current target
            target.isHighlighted = false;
            target.renderer.material = target.originalMaterial;
            target.runOnce = false;

            // Select a random target
            //int nextIndex = Random.Range(0, theHolder.transform.childCount);

            // Select the target across from the current target
            int nextIndex = (i + (numTargets / 2)) % numTargets;

            // Add the index of the next selected object to the list of highlighted indices
            highlightedObjects.Add(nextIndex);

            // Highlight the selected target
            theHolder.transform.GetChild(nextIndex).GetComponent<FittsLawCircle>().isHighlighted = true;

            // Increase the selections count
            selectionsCount++;

            loopStarted = false;
            hasRun = false;
            return;
        }
 
    }

}

public GameObject GetHighlightedChild()
{
    for (int i = 0; i < theHolder.transform.childCount; i++)
    {
        FittsLawCircle target = theHolder.transform.GetChild(i).GetComponent<FittsLawCircle>();
        if (target.isHighlighted)
        {
            // Return the child object that has isHighlighted set to true
            return target.gameObject;
        }
    }
    // If no highlighted child is found, return null
    return null;
}
}