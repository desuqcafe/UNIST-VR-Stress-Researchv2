using System.Collections;
using UnityEngine;
using TMPro;

public class InterviewQuestions : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public TextMeshProUGUI timerText;
    public AudioClip endQuestionAudio; // The audio clip to play at the end of each question
    private AudioSource audioSource; // The audio source to play the clip

    private int currentQuestionIndex = 0;
    private float questionTime = 50f; // Time for each question
    private float initialWaitTime = 90f; // Initial waiting time
    private float delayBetweenQuestions = 4f; // Delay before switching to new question

    private string[] questions = new string[] {
        "Tell me about yourself.",
        "Why should we hire you?",
        "What are your greatest professional strengths?",
        "Tell me about a challenge or conflict you've faced at work, and how you dealt with it.",
        "Where do you see yourself in five years?"
    };

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private Coroutine questionCoroutine; // The current question timer coroutine

    private void OnEnable()
    {
        currentQuestionIndex = 0; // Reset the current question index

        // If a question timer coroutine is currently running, stop it
        if (questionCoroutine != null)
            StopCoroutine(questionCoroutine);

        // Start a new question timer coroutine
        questionCoroutine = StartCoroutine(QuestionTimer());
    }

    private IEnumerator QuestionTimer()
    {
        // Initial waiting time
        yield return StartCoroutine(StartCountdown(initialWaitTime));

        // Loop through the questions
        while (currentQuestionIndex < questions.Length)
        {
            questionText.text = questions[currentQuestionIndex];

            yield return StartCoroutine(StartCountdown(questionTime));

            audioSource.PlayOneShot(endQuestionAudio);

            // Start countdown for the audio length plus delay before switching to new question
            yield return StartCoroutine(StartCountdown(endQuestionAudio.length + delayBetweenQuestions));

            currentQuestionIndex++;
        }
    }

    private IEnumerator StartCountdown(float countdownValue)
    {
        while (countdownValue > 0)
        {
            timerText.text = "Time Remaining: " + countdownValue;
            yield return new WaitForSeconds(1.0f);
            countdownValue--;
        }
    }
}
