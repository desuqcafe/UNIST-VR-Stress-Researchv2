using UnityEngine;
using System.Collections.Generic;

public class SimpleFittLaw : MonoBehaviour
{
    public GameObject spherePrefab;
    private int sphereCount = 9;

    public Material correctMaterial;
    public Material incorrectMaterial;

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
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            SpawnSpheres();
        }
    }

}
