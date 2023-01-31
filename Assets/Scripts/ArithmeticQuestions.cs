using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ArithmeticQuestions : MonoBehaviour
{
    public TMP_Text questionText;
    public Button[] answerButtons;

    private int correctAnswer;
    private string[] questions = new string[4];
    private string[,] answers = new string[4,4];
    private int currentQuestion = 0;

    TaskManager taskManager;

    void Start()
    {
        questions[0] = "Please choose the correct answer:\n\n30";
        questions[1] = "Please choose the correct answer:\n\n40";
        questions[2] = "Please choose the correct answer:\n\n20";
        questions[3] = "Please choose the correct answer:\n\n10";

        answers[0,0] = "2";
        answers[0,1] = "30";
        answers[0,2] = "4";
        answers[0,3] = "1";

        answers[1,0] = "2";
        answers[1,1] = "3";
        answers[1,2] = "40";
        answers[1,3] = "1";

        answers[2,0] = "20";
        answers[2,1] = "3";
        answers[2,2] = "4";
        answers[2,3] = "1";

        answers[3,0] = "2";
        answers[3,1] = "3";
        answers[3,2] = "4";
        answers[3,3] = "10";

        currentQuestion = 0;
        GenerateQuestion();
    }

public void OnButtonClick(int buttonIndex)
{
    string[] correctAnswers = new string[] { "30", "40", "20", "10" };
    if (Array.IndexOf(correctAnswers, answers[currentQuestion,buttonIndex]) != -1)
    {
        Debug.Log("Correct!");
        currentQuestion++;
        if (currentQuestion >= questions.Length)
        {
            currentQuestion = 0;

            taskManager.IntroDisable();
            taskManager.SubtractionEnable();

        }
        GenerateQuestion();
    }
    else
    {
        Debug.Log("Incorrect");
    }
}

    private void GenerateQuestion()
    {
        questionText.text = questions[currentQuestion];
        for (int i = 0; i < 4; i++)
        {
            answerButtons[i].GetComponentInChildren<TMP_Text>().text = answers[currentQuestion,i];
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }
    }
}
