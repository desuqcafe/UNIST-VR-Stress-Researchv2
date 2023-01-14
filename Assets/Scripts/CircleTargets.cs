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
}

public void CircleTouched()
{
    if (hasRun)
    {
        return;
    }
    hasRun = true;
    theHolder = GameObject.Find("FittGenerator");

    Debug.Log(theHolder.transform.childCount);

    // Check if the first target is highlighted
    FittsLawCircle firstTarget = theHolder.transform.GetChild(0).GetComponent<FittsLawCircle>();
    if (!firstTarget.isHighlighted)
    {
        // if no highlighted object is found, break out of the loop and do not execute the rest of the code
        return;
    }
    bool loopStarted = false;

    // Check if any of the targets are highlighted
    for (int i = 0; i < theHolder.transform.childCount; i++)
    {
        FittsLawCircle target = theHolder.transform.GetChild(i).GetComponent<FittsLawCircle>();
        if (target.isHighlighted && !loopStarted)
        {
            loopStarted = true;

            // Unhighlight the current target
            target.isHighlighted = false;
            target.renderer.material = target.originalMaterial;
            target.runOnce = false;

            // Select a random target
            int nextIndex = Random.Range(0, theHolder.transform.childCount);

            // Highlight the selected target
            theHolder.transform.GetChild(nextIndex).GetComponent<FittsLawCircle>().isHighlighted = true;

            // Increase the selections count
            selectionsCount++;

            loopStarted = false;
            hasRun = false;
        }
    }
}
}