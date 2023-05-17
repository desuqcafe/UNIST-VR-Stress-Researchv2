using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSphereHandler : MonoBehaviour
{
    public GameObject sphereSelector;
    public bool isCorrect;

    public void checkSphere()
    {
        Debug.Log("Sphere: " + isCorrect);
        sphereSelector.GetComponent<SimpleFittLaw>().SphereClicked(isCorrect);
    }
}
