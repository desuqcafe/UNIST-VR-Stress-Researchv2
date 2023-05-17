using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour {

    public int timeRemaining = 240;
    public TextMeshProUGUI text;

    void Start() {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update() {
        timeRemaining--;

        text.text = timeRemaining.ToString();

        if (timeRemaining <= 0) {
            // Do something when the timer expires.
        }
    }
}