using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;

    List<TMP_Text> sphereTexts = new List<TMP_Text>();
    private List<GameObject> spheresList = new List<GameObject>(); // list to store all spawned spheres


    private int sphereCount = 0;
    private int correctAnswer = 1022;
    public int score = 0;
    private int roundCount = 0;
    private int maxRounds = 20;


    public static FittsLawCircleSubtraction Instance; // singleton

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        SpawnSpheres();
    }

    void SetSphereText(GameObject sphere, string text)
    {
        sphereCount++;
        TMP_Text sphereText = sphere.GetComponentInChildren<TMP_Text>();

        if (sphereText != null)
        {
            sphereTexts.Add(sphereText);
            sphereText.text = text;
        }
        else
        {
            // Wrong answer spheres
            sphereText = sphere.AddComponent<TMP_Text>();
            int min = correctAnswer - 15;
            int max = correctAnswer + 15;
            int wrongAnswer = Random.Range(min, max);
            while (wrongAnswer == correctAnswer)
            {
                wrongAnswer = Random.Range(min, max);
            }
            sphereText.text = wrongAnswer.ToString();
        }
    }





    void SpawnSpheres()
    {
        float radius = 0.5f;
        for (int i = 0; i < 9; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            spheresList.Add(sphere); // add to list to watch


            Debug.Log("Sphere instantiated: " + sphere.name);

            // Wrong answer spheres
            int min = correctAnswer - 15;
            int max = correctAnswer + 15;
            int wrongAnswer = Random.Range(min, max);
            while (wrongAnswer == correctAnswer)
            {
                wrongAnswer = Random.Range(min, max);
            }
            SetSphereText(sphere, wrongAnswer.ToString());

            float angle = (float)(i + 1) / 9f * Mathf.PI * 2f;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            sphere.transform.position = new Vector3(x, y, 0f);
        }

        // Randomly select a sphere to be the correct one
        int correctIndex = Random.Range(0, sphereTexts.Count);
        if (sphereTexts[correctIndex].transform.parent.parent.gameObject.TryGetComponent<FittsLawTarget>(out FittsLawTarget fittsLawTarget))
        {
            fittsLawTarget.correctSphere = true;
            sphereTexts[correctIndex].text = correctAnswer.ToString();
        }
        else
        {
            Debug.Log(sphereTexts[correctIndex].gameObject);
            Debug.Log(sphereTexts[correctIndex].transform.parent.gameObject);
            Debug.Log(sphereTexts[correctIndex].transform.parent.parent.gameObject);
            Debug.LogError("FittsLawTarget component not found on the sphere object.");
        }

        Debug.Log("Spheres Finished Spawning" + "\nRound Count: " + roundCount);
        Debug.Log("CorrectAnswer: " + correctAnswer);
    }

    // void Update()
    // {
    //     if (Keyboard.current.eKey.wasPressedThisFrame)
    //     {
    //         deepBreathCoRoutine();
    //     }
    // }


    public void ContinueGame(GameObject correctSphere)
    {
        roundCount++;
        correctAnswer -= 13;

        if (roundCount > maxRounds)
        {
            // gameOverText.text = "Game over! Final score: " + score;
        }
        else
        {
            // Choose a new correct sphere index randomly
            int newCorrectIndex = Random.Range(0, sphereTexts.Count);

            for (int i = 0; i < sphereTexts.Count; i++)
            {
                TMP_Text sphereText = sphereTexts[i];
                GameObject sphere = sphereText.transform.parent.parent.gameObject;
                FittsLawTarget fittsLawTarget = sphere.GetComponent<FittsLawTarget>();

                // Check if the fittsLawTarget component exists
                if (fittsLawTarget == null)
                {
                    Debug.LogError("FittsLawTarget component not found on the sphere object.");
                    continue;
                }

                FittsLawTarget correctFittsLawTarget = correctSphere.GetComponent<FittsLawTarget>();
                if (fittsLawTarget == correctFittsLawTarget)
                {
                    // Set the previously correct sphere to false
                    fittsLawTarget.correctSphere = false;
                }

                // If current index is the new correct index, set correctSphere to true
                if (i == newCorrectIndex)
                {
                    fittsLawTarget.correctSphere = true;
                    sphereText.text = correctAnswer.ToString();
                }
                else
                {
                    fittsLawTarget.correctSphere = false;
                    int min = correctAnswer - 15;
                    int max = correctAnswer + 15;
                    int wrongAnswer = Random.Range(min, max);
                    while (wrongAnswer == correctAnswer)
                    {
                        wrongAnswer = Random.Range(min, max);
                    }

                    sphereText.text = wrongAnswer.ToString();
                }
            }
        }

        Debug.Log("Next Round Started " + "\nRound Count: " + roundCount);
        Debug.Log("CorrectAnswer: " + correctAnswer);
    }






    public void ResetGame()
    {
        foreach (GameObject sphere in spheresList)
        {
            Destroy(sphere);
        }

        spheresList.Clear();

        roundCount++;
        correctAnswer = 1022;

        if (roundCount > maxRounds)
        {
            // gameOverText.text = "Game over! Final score: " + score;
        }
        else
        {
            sphereCount = 0;
            score = 0;
            sphereTexts.Clear();
            SpawnSpheres();
        }

        Debug.Log("Game Reset");
    }
}