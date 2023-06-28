using UnityEngine;
using System.Collections.Generic;

public class FittBehaviorLaw : MonoBehaviour
{
    public GameObject spherePrefab;
    private int sphereCount = 9;

    public Material correctMaterial;
    public Material incorrectMaterial;

    private int currentSphereIndex;
    private List<GameObject> spheres = new List<GameObject>();

    void Start()
    {
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
        sphere.GetComponent<FittBehaviorSphere>().isCorrect = isCorrect; // Update isCorrect field
    }

    void SpawnSpheres()
    {
        float radius = 0.5f;

        for (int i = 0; i < sphereCount; i++)
        {
            GameObject sphere = Instantiate(spherePrefab, transform);
            spheres.Add(sphere);

            float angle = (float)(i + 1) / sphereCount * Mathf.PI * 2f;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            sphere.transform.localPosition = new Vector3(x, y, 0f);

            FittBehaviorSphere clickHandler = sphere.GetComponent<FittBehaviorSphere>();
            clickHandler.sphereSelectorBehavior = this.gameObject;
            clickHandler.isCorrect = (i == 0);  // Only the first sphere is correct initially

            SetSphereMaterial(sphere, clickHandler.isCorrect); // Set sphere material based on correctness
        }

        currentSphereIndex = 0;  // Start with the first sphere
    }

    public void SphereClicked(bool isCorrect)
    {
        Debug.Log("Entered Sphere Clicked: " + isCorrect);

        if (isCorrect)
        {
            // Select the sphere at the opposite side as the next correct one
            currentSphereIndex = (currentSphereIndex + sphereCount / 2) % sphereCount;
            SetSphereMaterial(spheres[currentSphereIndex], true);

            // Reset all other spheres as incorrect
            for (int i = 0; i < sphereCount; i++)
            {
                if (i != currentSphereIndex)
                {
                    SetSphereMaterial(spheres[i], false);
                }
            }
        }
    }
}