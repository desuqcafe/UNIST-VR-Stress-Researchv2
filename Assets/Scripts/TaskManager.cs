using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager Instance { get; private set; }

    public GameObject KeyboardTask;
    public GameObject SubtractionTask;
    public GameObject IntroTask;
    public GameObject EvaluationTask;
    public GameObject FittTask;

    public GameObject introCube;
    public GameObject introSphere;
    private IntroObject introCubeScript;
    private IntroObject introSphereScript;

    bool introFinished = false;

    void Awake(){

    if (Instance != null && Instance != this) 
    { 
        Destroy(this); 
    } 
    else 
    { 
        Instance = this; 
    } 
    
    introCubeScript = introCube.GetComponent<IntroObject>();
    introSphereScript = introSphere.GetComponent<IntroObject>();

    }

    void Start(){
    InvokeRepeating("introObjectsCheck", 1.0f, 1.0f);
    InvokeRepeating("CancelInvokesCheck", 5.0f, 5.0f);
    }

    private void introObjectsCheck()
    {
      //  Debug.Log("Clled IntroObjectsCheck" + introCubeScript.userHandCame + " " + introSphereScript.userHandCame);

        if (introCubeScript.userHandCame == true && introSphereScript.userHandCame == true) {
            introFinished = true;
            introObjectDisable();
            IntroEnable();
        }
    }

    private void CancelInvokesCheck()
    {
        if (introFinished == true) {
            CancelInvoke("introObjectsCheck");
            CancelInvoke("CancelInvokesCheck");
        }
    }

    public void introObjectEnable(){
        introCube.SetActive(true);
        introSphere.SetActive(true);
    }

    public void introObjectDisable(){
        introCube.SetActive(false);
        introSphere.SetActive(false);
    }

    public void KeyboardEnable(){
        KeyboardTask.SetActive(true);
    }

    public void KeyboardDisable(){
        KeyboardTask.SetActive(false);
    }

    public void SubtractionEnable(){
        SubtractionTask.SetActive(true);
    }

    public void SubtractionDisable(){
        SubtractionTask.SetActive(false);
    }

    public void IntroEnable(){
        IntroTask.SetActive(true);
    }

    public void IntroDisable(){
        IntroTask.SetActive(false);
    }

    public void EvaluationEnable(){
        EvaluationTask.SetActive(true);
    }

    public void EvaluationDisable(){
        EvaluationTask.SetActive(false);
    }
    
    public void FittEnable(){
        FittTask.SetActive(true);
    }

    public void FittDisable(){
        FittTask.SetActive(false);
    }

}
