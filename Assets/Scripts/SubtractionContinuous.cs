using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SubtractionContinuous : MonoBehaviour
{
    public TMP_Text questionText;
    public TMP_Text remainingText;
    public Button[] answerButtons;
    [SerializeField] private int correctAnswer;
    [SerializeField] private int currentValue = 1022;
    [SerializeField] private int currentQuestion = 0;

    void Start()
    {
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

        if (currentQuestion >= 50)
        {
            Debug.Log("Quiz finished");
            return;
        }
        GenerateQuestion();
        currentQuestion++;
    }

    private void GenerateQuestion()
    {
        questionText.text = "Please choose the correct answer:\n\n" + currentValue + " - 13 = ?";

        for (int i = 0; i < 4; i++)
        {
                        // Get the Image component on the button
            Image buttonImage = answerButtons[i].GetComponent<Image>();
            // Change the color of the image to green
            buttonImage.color = Color.white;

            int buttonValue = currentValue - 13 + i - 2;
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = buttonValue.ToString();
            if (buttonValue == currentValue - 13)
            {
                correctAnswer = i;
            }
        }
    }
}