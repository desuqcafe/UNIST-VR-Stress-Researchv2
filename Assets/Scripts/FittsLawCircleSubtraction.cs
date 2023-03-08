using UnityEngine;
using TMPro;

public class FittsLawCircleSubtraction : MonoBehaviour
{
    public GameObject spherePrefab;
    public Canvas canvas;
    public TMP_Text sphereTextPrefab;
    public TMP_Text scoreText;
    public TMP_Text gameOverText;

    private int sphereCount = 0;
    private int correctAnswer = 1009;
    private int score = 0;
    private int roundCount = 0;
    private int maxRounds = 5;

    void Start()
    {
        SpawnSpheres();
    }

    void SetSphereText(GameObject sphere)
    {
        sphereCount++;
        TMP_Text sphereText = sphere.GetComponentInChildren<TMP_Text>();
        canvas = sphere.GetComponentInChildren<Canvas>();
        sphereTextPrefab = sphereText;

        if (sphereCount == 5)
        {
            // Correct answer sphere
            sphereText.text = correctAnswer.ToString();
        }
        else
        {
            // Wrong answer spheres
            int min = correctAnswer - 10;
            int max = correctAnswer + 10;
            int wrongAnswer = Random.Range(min, max);
            while (wrongAnswer == correctAnswer)
            {
                wrongAnswer = Random.Range(min, max);
            }
            sphereText.text = wrongAnswer.ToString();
        }

        float angle = (float)sphereCount / 9f * Mathf.PI * 2f;
        float radius = 0.5f;
        float x = Mathf.Sin(angle) * radius;
        float y = Mathf.Cos(angle) * radius;
        sphere.transform.position = new Vector3(x, y, 0f);
    }

    void SpawnSpheres()
    {
        for (int i = 0; i < 9; i++)
        {
            GameObject sphere = Instantiate(spherePrefab);
            SetSphereText(sphere);
        }
    }

    public void SphereClicked(string text)
    {
        int value;
        if (int.TryParse(text, out value))
        {
            if (value == correctAnswer)
            {
                // Correct answer
                score++;
                ContinueGame();
            }
            else
            {
                // Wrong answer
                score--;
                // scoreText.text = "Score: " + score;
            }
        }
        else
        {
            Debug.LogError("Invalid integer value: " + text);
        }
    }

    public void ContinueGame()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        roundCount++;
        correctAnswer -= 13;
       // scoreText.text = "Score: " + score;

        if (roundCount > maxRounds)
        {
           // gameOverText.text = "Game over! Final score: " + score;
        }
        else
        {
            SpawnSpheres();
        }
    }
}