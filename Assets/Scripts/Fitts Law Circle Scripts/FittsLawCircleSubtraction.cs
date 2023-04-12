using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;

    List<TMP_Text> sphereTexts = new List<TMP_Text>();

    private int sphereCount = 0;
    private int correctAnswer = 1022;
    private int score = 0;
    private int roundCount = 0;
    private int maxRounds = 5;

    void Start()
    {
        SpawnSpheres();
    }

    void SetSphereText(GameObject sphere, string text)
    {
        sphereCount++;
        TMP_Text sphereText = sphere.GetComponentInChildren<TMP_Text>();
        sphereTexts.Add(sphereText);
        sphereText.text = text;

        if (sphereCount == 5)
        {
            FittsLawTarget fittsLawTarget = sphere.GetComponent<FittsLawTarget>();

            // Correct answer sphere
            fittsLawTarget.correctSphere = true;
            Debug.Log("Setting Correct sphere: " + fittsLawTarget.correctSphere);

        }
        else
        {
            // Wrong answer spheres
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
            if (i == 4)
            {
                SetSphereText(sphere, correctAnswer.ToString());
            }
            else
            {
                // Wrong answer spheres
                int min = correctAnswer - 15;
                int max = correctAnswer + 15;
                int wrongAnswer = Random.Range(min, max);
                while (wrongAnswer == correctAnswer)
                {
                    wrongAnswer = Random.Range(min, max);
                }
                SetSphereText(sphere, wrongAnswer.ToString());
            }
            float angle = (float)(i + 1) / 9f * Mathf.PI * 2f;
            float x = Mathf.Sin(angle) * radius;
            float y = Mathf.Cos(angle) * radius;
            sphere.transform.position = new Vector3(x, y, 0f);
        }

        Debug.Log("Spheres Finished Spawning" + "\nRound Count: " + roundCount);
        Debug.Log("CorrectAnswer: " + correctAnswer);
    }

    public void SphereClicked(XRBaseInteractable interactable)
    {
        Collider collider = interactable.GetComponent<Collider>();
        FittsLawTarget fittsLawTarget = collider.GetComponent<FittsLawTarget>();

        if (fittsLawTarget.correctSphere)
        {
            score++;
            ContinueGame();
        }
        else
        {
            // ResetGame();
            Debug.Log("Incorrect Choice");
        }
    }

    public void ContinueGame()
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

        // Keep track of the index of the correct sphere text
        int correctIndex = -1;

        for (int i = 0; i < sphereTexts.Count; i++)
        {
            TMP_Text sphereText = sphereTexts[i];
            FittsLawTarget fittsLawTarget = sphereText.transform.parent.GetComponent<FittsLawTarget>();

            if (i == newCorrectIndex)
            {
                fittsLawTarget.correctSphere = true;
                correctIndex = i;
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

        // Update the text value of the correct sphere separately
        if (correctIndex >= 0)
        {
            TMP_Text correctSphereText = sphereTexts[correctIndex];
            correctSphereText.text = correctAnswer.ToString();
        }
    }

    Debug.Log("Next Round Started " + "\nRound Count: " + roundCount);
    Debug.Log("CorrectAnswer: " + correctAnswer);
}

    public void ResetGame()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

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