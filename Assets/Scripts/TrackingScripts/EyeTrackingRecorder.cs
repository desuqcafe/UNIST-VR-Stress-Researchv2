using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EyeTrackingRecorder : MonoBehaviour
{

    // Teleporting Controls:  Q W E R 






    // 6 File Paths to change for file writing 
    // 3 in EyeTracking Recorder
    // 1 in SimpleFittLaw
    // 1 in FittLawCircleSubtraction
    // 1 in StroopRoomController
    // 1 in PhraseChecker
    public static EyeTrackingRecorder Instance;


    private string folderPath;
    private string timeStamp;
    private string eyeTrackingDataFilePath;
    private string headsetAndControllerDataFilePath;
    private string headsetVelocityAndAccelerationDataFilePath;
    public string currentTask { get; set; }

    // Buffer to store data before writing to files
    private List<string> eyeTrackingDataBuffer = new List<string>();
    private List<string> headsetAndControllerDataBuffer = new List<string>();
    private List<string> headsetVelocityAndAccelerationDataBuffer = new List<string>();

    // Frame interval for writing data to the files
    public int writeInterval = 60;
    private int frameCounter = 0;


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
        folderPath = Application.persistentDataPath;
        timeStamp = System.DateTime.Now.ToString("HH-mm-ss");

        eyeTrackingDataFilePath = Path.Combine(folderPath, "eyeTrackingData_" + timeStamp + ".csv");
        headsetAndControllerDataFilePath = Path.Combine(folderPath, "headsetAndControllerData_" + timeStamp + ".csv");
        headsetVelocityAndAccelerationDataFilePath = Path.Combine(folderPath, "headsetVelocityAndAccelerationData_" + timeStamp + ".csv");
    }



    // Reference to the OVREyeGaze script for left and right eyes
    public OVREyeGaze leftEyeGaze;
    public OVREyeGaze rightEyeGaze;

    // Variables to store eye tracking data
    private Vector3 leftEyeGazeDirection;
    private Vector3 rightEyeGazeDirection;
    private float leftEyeConfidence;
    private float rightEyeConfidence;

    // Reference to headset and controllers
    public Transform headsetTransform;
    public Transform leftControllerTransform;
    public Transform rightControllerTransform;

    // Variables to store headset and controller positions and rotations
    private Vector3 headsetPosition;
    private Quaternion headsetRotation;
    private Vector3 leftControllerPosition;
    private Quaternion leftControllerRotation;
    private Vector3 rightControllerPosition;
    private Quaternion rightControllerRotation;

    private Vector3 previousHeadsetPosition;
    private Vector3 headsetVelocity;
    private Vector3 previousHeadsetVelocity;
    private Vector3 headsetAcceleration;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize variables
        leftEyeGazeDirection = Vector3.zero;
        rightEyeGazeDirection = Vector3.zero;
        leftEyeConfidence = 0;
        rightEyeConfidence = 0;

        previousHeadsetPosition = headsetTransform.position;
        headsetVelocity = Vector3.zero;
        previousHeadsetVelocity = Vector3.zero;
        headsetAcceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Get eye tracking data from OVREyeGaze for left and right eyes
        leftEyeGazeDirection = leftEyeGaze.transform.forward;
        leftEyeConfidence = leftEyeGaze.Confidence;
        rightEyeGazeDirection = rightEyeGaze.transform.forward;
        rightEyeConfidence = rightEyeGaze.Confidence;

        // Buffer eye tracking data
        BufferEyeTrackingData();

        // Get headset and controller positions and rotations
        headsetPosition = headsetTransform.position;
        headsetRotation = headsetTransform.rotation;
        leftControllerPosition = leftControllerTransform.position;
        leftControllerRotation = leftControllerTransform.rotation;
        rightControllerPosition = rightControllerTransform.position;
        rightControllerRotation = rightControllerTransform.rotation;

        // Buffer headset and controller data
        BufferHeadsetAndControllerData();

        // Calculate headset velocity and acceleration
        Vector3 currentHeadsetPosition = headsetTransform.position;
        headsetVelocity = (currentHeadsetPosition - previousHeadsetPosition) / Time.deltaTime;
        headsetAcceleration = (headsetVelocity - previousHeadsetVelocity) / Time.deltaTime;

        // Buffer headset velocity and acceleration data
        BufferHeadsetVelocityAndAccelerationData();

        // Update previous position and velocity values
        previousHeadsetPosition = currentHeadsetPosition;
        previousHeadsetVelocity = headsetVelocity;

        frameCounter++;

        if (frameCounter >= writeInterval)
        {
            // Write buffered data to the files
            WriteDataToFileAsync(eyeTrackingDataFilePath, eyeTrackingDataBuffer);
            WriteDataToFileAsync(headsetAndControllerDataFilePath, headsetAndControllerDataBuffer);
            WriteDataToFileAsync(headsetVelocityAndAccelerationDataFilePath, headsetVelocityAndAccelerationDataBuffer);

            // Clear the buffers
            eyeTrackingDataBuffer.Clear();
            headsetAndControllerDataBuffer.Clear();
            headsetVelocityAndAccelerationDataBuffer.Clear();

            // Reset the frame counter
            frameCounter = 0;
        }


    }

    // Function to buffer eye tracking data
    void BufferEyeTrackingData()
    {
        string eyeTrackingData = $"{Time.time}, {currentTask}, {leftEyeGazeDirection}, {rightEyeGazeDirection}, {leftEyeConfidence}, {rightEyeConfidence}";
        eyeTrackingDataBuffer.Add(eyeTrackingData);
    }

    // Function to buffer headset and controller data
    void BufferHeadsetAndControllerData()
    {
        string headsetAndControllerData = $"{Time.time}, {currentTask}, {headsetPosition}, {headsetRotation}, {leftControllerPosition}, {leftControllerRotation}, {rightControllerPosition}, {rightControllerRotation}";
        headsetAndControllerDataBuffer.Add(headsetAndControllerData);
    }

    void BufferHeadsetVelocityAndAccelerationData()
    {
        string headsetVelocityAndAccelerationData = $"{Time.time}, {currentTask}, {headsetVelocity}, {headsetAcceleration}";
        headsetVelocityAndAccelerationDataBuffer.Add(headsetVelocityAndAccelerationData);
    }



    // Write the data to file

    // Update the WriteDataToFile method to accept a List<string> instead of a single string
    public async void WriteDataToFileAsync(string filePath, List<string> dataBuffer)
    {
        try
        {
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                foreach (string data in dataBuffer)
                {
                    await outputFile.WriteLineAsync(data);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error writing data to file: " + ex.Message);
        }
    }



}
