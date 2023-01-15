using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SubtractionContinuous : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text remainingText;
    public Button[] answerButtons;
    [SerializeField] private int correctAnswer;
    [SerializeField] private int currentValue = 1022;
    [SerializeField] private int currentQuestion = 50;

    void Start()
    {
        currentQuestion = 50;
        GenerateQuestion();
        remainingText.text = "Remaining: " + currentQuestion.ToString();
    }

    public void OnButtonClick(int buttonIndex)
    {
        if (buttonIndex == correctAnswer)
        {
            // Get the Image component on the button
            Image buttonImage = answerButtons[buttonIndex].GetComponent<Image>();
            // Change the color of the image to green
            buttonImage.color = Color.green;

            currentValue -= 13;
        }
        else
        {
            // Get the Image component on the button
            Image buttonImage = answerButtons[buttonIndex].GetComponent<Image>();
            // Change the color of the image to green
            buttonImage.color = Color.red;

            currentValue = 1022;
        }
        
        remainingText.text = "Remaining: " + currentQuestion;

        if (currentQuestion <= 0)
        {
            Debug.Log("Quiz finished");
            return;
        }

        currentQuestion--;

StartCoroutine(WaitAndGenerateQuestion());

    }

    private IEnumerator WaitAndGenerateQuestion()
{
    yield return new WaitForSeconds(0.35f);
    GenerateQuestion();
}

    private void GenerateQuestion()
    {
        questionText.text = "Please choose the correct answer:\n\n" + currentValue + " - 13 = ?";

        // Generate a random number between 0 and 3 (inclusive)
        correctAnswer = Random.Range(0, 4);
        int[] incorrectAnswers = new int[3];
        int incorrectAnswerIndex = 0;
        for (int i = 0; i < 4; i++)
        {
            // Get the Image component on the button
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            // Change the color of the image to green
            buttonImage.color = Color.white;

            if (i == correctAnswer)
            {
                // Assign the correct answer to the button at the randomly generated index
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = (currentValue - 13).ToString();
            }
            else
            {
                //generate 3 different incorrect answers
                int incorrectAnswer;
                do
                {
                    incorrectAnswer = currentValue - 13 + Random.Range(-2, 3);
                    if (incorrectAnswer == currentValue - 13)
                    {
                        incorrectAnswer += 4;
                    }
                } while (incorrectAnswers.Contains(incorrectAnswer));

                incorrectAnswers[incorrectAnswerIndex] = incorrectAnswer;
                incorrectAnswerIndex++;
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = incorrectAnswer.ToString();
            }
        }
    }
}