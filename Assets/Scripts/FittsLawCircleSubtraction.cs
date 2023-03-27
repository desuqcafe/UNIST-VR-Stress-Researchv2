using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections.Generic;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

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
        canvas = sphere.GetComponentInChildren<Canvas>();
        sphereTextPrefab = sphereText;

        sphereTexts.Add(sphereText);

        sphereText.text = text;

        if (sphereCount == 5)
        {
            FittsLawTarget fittsLawTarget = sphere.GetComponent<FittsLawTarget>();

            // Correct answer sphere
            fittsLawTarget.correctSphere = true;
            Debug.Log("Correct sphere: " + fittsLawTarget.correctSphere);

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
    }

    public void SphereClicked(XRBaseInteractable interactable)
    {
        Collider collider = interactable.GetComponent<Collider>();
        FittsLawTarget fittsLawTarget = collider.GetComponent<FittsLawTarget>();

        if (fittsLawTarget.correctSphere)
        {
            //fittsLawTarget.correctSphere = false;
            ContinueGame();
        }
        else
        {
           // ResetGame();
        }
    }

    public void ContinueGame()
    {
        roundCount = roundCount + 1;
        correctAnswer -= 13;

        if (roundCount > maxRounds)
        {
            // gameOverText.text = "Game over! Final score: " + score;
        }
        else
        {
            // Update the existing spheres instead of spawning new ones
            foreach (TMP_Text sphereText in sphereTexts)
            {
                if (sphereText.text == correctAnswer.ToString())
                {
                    FittsLawTarget fittsLawTarget = sphereText.transform.parent.GetComponent<FittsLawTarget>();
                    fittsLawTarget.correctSphere = true;
                    // Update the text of the correct sphere
                    sphereText.text = correctAnswer.ToString();
                }
                else
                {
                    int min = correctAnswer - 15;
                    int max = correctAnswer + 15;
                    int wrongAnswer = Random.Range(min, max);
                    while (wrongAnswer == correctAnswer)
                    {
                        wrongAnswer = Random.Range(min, max);
                    }

                    // Update the text of the incorrect spheres
                    sphereText.text = wrongAnswer.ToString();
                }
            }
        }

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
            SpawnSpheres();
        }

        Debug.Log("Game Reset");
    }
}