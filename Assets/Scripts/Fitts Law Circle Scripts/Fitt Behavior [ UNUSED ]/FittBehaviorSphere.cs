
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FittBehaviorSphere : MonoBehaviour
{
    public GameObject sphereSelectorBehavior;
    public bool isCorrect;

    public void checkSphere()
    {
        Debug.Log("Sphere: " + isCorrect);
        sphereSelectorBehavior.GetComponent<FittBehaviorLaw>().SphereClicked(isCorrect);
    }

}