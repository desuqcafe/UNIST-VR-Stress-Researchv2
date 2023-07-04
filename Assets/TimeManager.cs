
using UnityEngine;
using System.Diagnostics;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    private Stopwatch stopwatch;
    public double CurrentTime { get { return stopwatch.Elapsed.TotalSeconds; } }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            stopwatch = Stopwatch.StartNew();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

