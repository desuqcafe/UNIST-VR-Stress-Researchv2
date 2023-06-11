using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class EyeTrackingRecorder : MonoBehaviour
{
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

        // Record eye tracking data
        RecordEyeTrackingData();

        // Get headset and controller positions and rotations
        headsetPosition = headsetTransform.position;
        headsetRotation = headsetTransform.rotation;
        leftControllerPosition = leftControllerTransform.position;
        leftControllerRotation = leftControllerTransform.rotation;
        rightControllerPosition = rightControllerTransform.position;
        rightControllerRotation = rightControllerTransform.rotation;

        // Record headset and controller data
        RecordHeadsetAndControllerData();

        // Calculate headset velocity and acceleration
        Vector3 currentHeadsetPosition = headsetTransform.position;
        headsetVelocity = (currentHeadsetPosition - previousHeadsetPosition) / Time.deltaTime;
        headsetAcceleration = (headsetVelocity - previousHeadsetVelocity) / Time.deltaTime;

        // Record headset velocity and acceleration data
        RecordHeadsetVelocityAndAccelerationData();

        // Update previous position and velocity values
        previousHeadsetPosition = currentHeadsetPosition;
        previousHeadsetVelocity = headsetVelocity;
    }

    // Function to record eye tracking data
    void RecordEyeTrackingData()
    {
        // Here, you can save the eye tracking data to a file, send it to a server, or use it for further analysis
        // Debug.Log("Left Eye Gaze Direction: " + leftEyeGazeDirection);
        // Debug.Log("Right Eye Gaze Direction: " + rightEyeGazeDirection);
        // Debug.Log("Left Eye Confidence: " + leftEyeConfidence);
        // Debug.Log("Right Eye Confidence: " + rightEyeConfidence);

        string filePath = @"C:\Users\INTERACTIONS\Desktop\eyeTrackingData.csv";
        string data = $"{leftEyeGazeDirection}, {rightEyeGazeDirection}, {leftEyeConfidence}, {rightEyeConfidence}";
        WriteDataToFile(filePath, data);
    }

    // Function to record headset and controller data
    void RecordHeadsetAndControllerData()
    {
        // Here, you can save the headset and controller data to a file, send it to a server, or use it for further analysis
        // Debug.Log("Headset Position: " + headsetPosition);
        // Debug.Log("Headset Rotation: " + headsetRotation);
        // Debug.Log("Left Controller Position: " + leftControllerPosition);
        // Debug.Log("Left Controller Rotation: " + leftControllerRotation);
        // Debug.Log("Right Controller Position: " + rightControllerPosition);
        // Debug.Log("Right Controller Rotation: " + rightControllerRotation);

        string filePath = @"C:\Users\INTERACTIONS\Desktop\headsetAndControllerData.csv";
        string data = $"{headsetPosition}, {headsetRotation}, {leftControllerPosition}, {leftControllerRotation}, {rightControllerPosition}, {rightControllerRotation}";
        WriteDataToFile(filePath, data);
    }

    void RecordHeadsetVelocityAndAccelerationData()
    {
        // // Here, you can save the headset velocity and acceleration data to a file, send it to a server, or use it for further analysis
        // Debug.Log("Headset Velocity: " + headsetVelocity);
        // Debug.Log("Headset Acceleration: " + headsetAcceleration);

        string filePath = @"C:\Users\INTERACTIONS\Desktop\headsetVelocityAndAccelerationData.csv";
        string data = $"{headsetVelocity}, {headsetAcceleration}";
        WriteDataToFile(filePath, data);
    }




    // Write the data to file

    public static void WriteDataToFile(string filePath, string data)
    {
        try
        {
            using (StreamWriter outputFile = new StreamWriter(filePath, true))
            {
                outputFile.WriteLine(data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error writing data to file: " + ex.Message);
        }
    }



}
