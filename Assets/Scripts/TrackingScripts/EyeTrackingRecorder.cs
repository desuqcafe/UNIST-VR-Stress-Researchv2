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

    public TMPro.TextMeshProUGUI filePathText;

    private string folderPath;
    private string timeStamp;
    private string hitDataFilePath;
    private string faceExpressionDataFilePath;
    private string eyeTrackingDataFilePath;
    private string headsetAndControllerDataFilePath;
    private string headsetVelocityAndAccelerationDataFilePath;
    public string currentTask { get; set; }

    // Buffer to store data before writing to files
    private List<string> hitDataBuffer = new List<string>();
    private List<string> faceExpressionDataBuffer = new List<string>();
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

        hitDataFilePath = Path.Combine(folderPath, "hitData_" + timeStamp + ".csv");
        faceExpressionDataFilePath = Path.Combine(folderPath, "faceExpressionData_" + timeStamp + ".csv");
        eyeTrackingDataFilePath = Path.Combine(folderPath, "eyeTrackingData_" + timeStamp + ".csv");
        headsetAndControllerDataFilePath = Path.Combine(folderPath, "headsetAndControllerData_" + timeStamp + ".csv");
        headsetVelocityAndAccelerationDataFilePath = Path.Combine(folderPath, "headsetVelocityAndAccelerationData_" + timeStamp + ".csv");


        string directoryPath = Path.GetDirectoryName(eyeTrackingDataFilePath);

        if (Directory.Exists(directoryPath))
        {
            filePathText.text = "Eye tracking data file path: " + eyeTrackingDataFilePath;
        }
        else
        {
            filePathText.text = "Directory does not exist: " + directoryPath;
        }
    }

    // Hit position and time data
    // public LayerMask trackingLayerMask;
    private RaycastHit hit;
    private Dictionary<GameObject, float> startTimeForObjects = new Dictionary<GameObject, float>();
    float duration;
    Vector3 hitPosition;
    GameObject hitObject;
    // Reference to OVRFaceExpressions script
    [SerializeField]
    private OVRFaceExpressions faceExpressions;
    private class FaceWeightComponent
    {
        public string Name;
        public float Weight;
        public OVRFaceExpressions.FaceExpression FaceExpression { get; set; }

        public override string ToString() => $"{FaceExpression}: {Name}: {Weight}";
    }
    private List<FaceWeightComponent> components = new List<FaceWeightComponent>();

    // Reference to the OVREyeGaze script for left and right eyes
    public OVREyeGaze leftEyeGaze;
    public OVREyeGaze rightEyeGaze;

    public Transform leftEye;
    public Transform rightEye;

    // Variables to store eye tracking data
    private Vector3 leftEyeGazeDirection;
    private Vector3 rightEyeGazeDirection;
    private float leftEyeConfidence;
    private float rightEyeConfidence;
    private Vector3 leftEyePosition;
    private Vector3 rightEyePosition;
    private Quaternion leftEyeRotation;
    private Quaternion rightEyeRotation;

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
        InitFaceExpression();
        leftEyeGazeDirection = Vector3.zero;
        rightEyeGazeDirection = Vector3.zero;
        leftEyeConfidence = 0;
        rightEyeConfidence = 0;

        previousHeadsetPosition = headsetTransform.position;
        headsetVelocity = Vector3.zero;
        previousHeadsetVelocity = Vector3.zero;
        headsetAcceleration = Vector3.zero;
    }
    private void InitFaceExpression() 
    {
        foreach (OVRFaceExpressions.FaceExpression e in Enum.GetValues(typeof(OVRFaceExpressions.FaceExpression)))
        {
            //Create a component for each face expression features
            FaceWeightComponent faceWeightComponent = new FaceWeightComponent();
            faceWeightComponent.FaceExpression = e;
            faceWeightComponent.Name = e.ToString();
            components.Add(faceWeightComponent);
        }
    }
    // Update is called once per frame
    void Update()
    {
        // hit data
        // trackingLayerMask = LayerMask.GetMask("TrackingLayer");
        if (Physics.Raycast(leftEye.transform.position, leftEye.transform.forward, out hit, 10) ||
            Physics.Raycast(rightEye.transform.position, rightEye.transform.forward, out hit, 10)) {
            // filePathText.text +=  "\n\nHit Position:" + hit.point.ToString();
            hitObject = hit.collider.gameObject;
            // filePathText.text += "\nHit Object:" + hitObject;
            
            hitPosition = hit.point;
            if (!startTimeForObjects.ContainsKey(hitObject)) {
                // This is the first time the object is looked at and store current time
                startTimeForObjects[hitObject] = Time.time;
            } else {
                // The object is being looked at and calculate the duration
                duration = Time.time - startTimeForObjects[hitObject];
                // filePathText.text += "\nObject: " + hitObject.name + ", Duration: " + duration; // secs: duration%60; mins: duration/60 
            }
        }
        // Buffer hit data
        BufferHitData();

        // Get face expression data from OVRFaceExpressions.cs
        foreach (FaceWeightComponent comp in components)
        {
            float weight;
            // filePathText.text += "- Num " + frameCounter + ":" + "\n" ;
            // filePathText.text += comp.FaceExpression + "\n";
            if (faceExpressions.TryGetFaceExpressionWeight(comp.FaceExpression, out weight))
            {
                // filePathText.text += weight;
                comp.Weight = weight;
            }
        }
        // Buffer face expression data
        BufferFaceExpressionData();

        // Get eye tracking data from OVREyeGaze for left and right eyes
        leftEyeGazeDirection = leftEyeGaze.transform.forward;
        leftEyeConfidence = leftEyeGaze.Confidence;
        rightEyeGazeDirection = rightEyeGaze.transform.forward;
        rightEyeConfidence = rightEyeGaze.Confidence;
        leftEyePosition = leftEyeGaze.transform.position;
        rightEyePosition = rightEyeGaze.transform.position;
        leftEyeRotation = leftEyeGaze.transform.rotation;
        rightEyeRotation = rightEyeGaze.transform.rotation;

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
            // Create new lists that are copies of the original lists and pass those to WriteDataToFileAsync
            var hitDataToWrite = new List<string>(hitDataBuffer);
            WriteDataToFileAsync(hitDataFilePath, hitDataToWrite);

            var faceExpressionDataToWrite = new List<string>(faceExpressionDataBuffer);
            WriteDataToFileAsync(faceExpressionDataFilePath, faceExpressionDataToWrite);

            var eyeTrackingDataToWrite = new List<string>(eyeTrackingDataBuffer);
            WriteDataToFileAsync(eyeTrackingDataFilePath, eyeTrackingDataToWrite);

            var headsetAndControllerDataToWrite = new List<string>(headsetAndControllerDataBuffer);
            WriteDataToFileAsync(headsetAndControllerDataFilePath, headsetAndControllerDataToWrite);

            var headsetVelocityAndAccelerationDataToWrite = new List<string>(headsetVelocityAndAccelerationDataBuffer);
            WriteDataToFileAsync(headsetVelocityAndAccelerationDataFilePath, headsetVelocityAndAccelerationDataToWrite);

            // Clear the buffers
            eyeTrackingDataBuffer.Clear();
            headsetAndControllerDataBuffer.Clear();
            headsetVelocityAndAccelerationDataBuffer.Clear();

            // Reset the frame counter
            frameCounter = 0;
        }
    }
    // Function to buffer face expression data
    void BufferHitData()
    {
        string hitData = $"{Time.time}, {currentTask}, {hitObject.name}, {hitPosition}, {duration}";
        hitDataBuffer.Add(hitData);
    }

    // Function to buffer face expression data
    void BufferFaceExpressionData()
    {
        string faceExpressionData = $"{Time.time}, {currentTask}";
        foreach (FaceWeightComponent comp in components)
        {
            faceExpressionData += $", {comp.Weight}";
        }
        faceExpressionDataBuffer.Add(faceExpressionData);
    }

    // Function to buffer eye tracking data
    void BufferEyeTrackingData()
    {
        string eyeTrackingData = $"{Time.time}, {currentTask}, {leftEyeGazeDirection}, {rightEyeGazeDirection}, {leftEyeConfidence}, {rightEyeConfidence}, {leftEyePosition}, {rightEyePosition}, {leftEyeRotation}, {rightEyeRotation}";
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