using UnityEngine;
using UnityEngine.XR;

public class FittsLawCircle : MonoBehaviour
{
    // Constants for the Fitts's law model
    public float a = 200;
    public float b = 0.5f;

    // Distance and width of the target
    public float distance = 1f;
    public float width = 0.1f;

    // Movement time for the target
    public float movementTime;

    // Indicates whether the target is currently highlighted
    public bool isHighlighted;

    // Renderer for the target game object
    public new Renderer renderer;

    // Material to use when the target is highlighted
    public Material highlightMaterial;

    // Original material of the target
    public Material originalMaterial;

    CircleTargets circleTarget;

    public bool runOnce = false;

    void Awake()
    {
        // Save the original material of the target
        originalMaterial = renderer.material;

        // Calculate the index of difficulty (ID) for the target
        float id = Mathf.Log(distance / width + 1, 2);

        // Calculate the movement time (MT) for the target
        movementTime = a + b * id;

    }

    void Update()
    {

        if (isHighlighted && !runOnce)
        {
            renderer.material = highlightMaterial;
            runOnce = true;
        }

    }
}