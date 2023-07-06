using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


public class EyeTrackingRecorder : MonoBehaviour
{


    public static EyeTrackingRecorder Instance;
    public double currentTime;

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

    // File path for face region confidence data
    private string faceRegionConfidenceDataFilePath;

    // Buffer to store face region confidence data
    private List<string> faceRegionConfidenceDataBuffer = new List<string>();


    public int writeInterval = 600;  // Write every 10 seconds or so if running at 60fps
    //public int maxBufferSize = 5000; // Change this to suit your needs    1000 elements
    private int frameCounter = 0;

    private void WriteHeadersToFile(string filePath)
    {
        // Get all the names of the FaceExpression enum
        List<string> faceExpressionNames = Enum.GetNames(typeof(OVRFaceExpressions.FaceExpression)).ToList();

        // Join them together with commas to make a CSV header
        string headers = string.Join(", ", faceExpressionNames);

        // Append 'currentTime' and 'currentTask' at the start of the header
        headers = "currentTime, currentTask, " + headers;

        // Write the headers to the file
        File.WriteAllText(filePath, headers + Environment.NewLine);
    }

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
        WriteHeadersToFile(faceExpressionDataFilePath);
        eyeTrackingDataFilePath = Path.Combine(folderPath, "eyeTrackingData_" + timeStamp + ".csv");
        headsetAndControllerDataFilePath = Path.Combine(folderPath, "headsetAndControllerData_" + timeStamp + ".csv");
        headsetVelocityAndAccelerationDataFilePath = Path.Combine(folderPath, "headsetVelocityAndAccelerationData_" + timeStamp + ".csv");

        faceRegionConfidenceDataFilePath = Path.Combine(folderPath, "faceRegionConfidenceData_" + timeStamp + ".csv");

        // Write headers to the face region confidence data file
        WriteFaceRegionConfidenceHeaderToFile(faceRegionConfidenceDataFilePath);


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


    void WriteFaceRegionConfidenceHeaderToFile(string filePath)
    {
        // Generate headers based on FaceRegionConfidenceComponent names
        string headers = "currentTime, currentTask";
        foreach (FaceRegionConfidenceComponent comp in faceRegionConfidenceComponents)
        {
            headers += $", {comp.Name}";
        }

        // Write the headers to the file
        File.WriteAllText(filePath, headers + Environment.NewLine);
    }

    void BufferFaceRegionConfidenceData()
    {
        // Generate a line of data
        string faceRegionConfidenceData = $"{currentTime}, {currentTask}";
        foreach (FaceRegionConfidenceComponent comp in faceRegionConfidenceComponents)
        {
            faceRegionConfidenceData += $", {comp.Confidence}";
        }

        // Add the line of data to the buffer
        faceRegionConfidenceDataBuffer.Add(faceRegionConfidenceData);
    }

    // Hit position and time data
    // public LayerMask trackingLayerMask;
    private RaycastHit hit;
    private Dictionary<GameObject, List<float>> hitTimeForObjects = new Dictionary<GameObject, List<float>>();
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

    // Class to store FaceRegionConfidence and its associated confidence value
    private class FaceRegionConfidenceComponent
    {
        public string Name;
        public float Confidence;
        public OVRFaceExpressions.FaceRegionConfidence FaceRegionConfidence { get; set; }

        public override string ToString() => $"{FaceRegionConfidence}: {Name}: {Confidence}";
    }
    private List<FaceRegionConfidenceComponent> faceRegionConfidenceComponents = new List<FaceRegionConfidenceComponent>();

    // Initialize the face region confidence components
    private void InitFaceRegionConfidence()
    {
        foreach (OVRFaceExpressions.FaceRegionConfidence e in Enum.GetValues(typeof(OVRFaceExpressions.FaceRegionConfidence)))
        {
            FaceRegionConfidenceComponent faceRegionConfidenceComponent = new FaceRegionConfidenceComponent();
            faceRegionConfidenceComponent.FaceRegionConfidence = e;
            faceRegionConfidenceComponent.Name = e.ToString();
            faceRegionConfidenceComponents.Add(faceRegionConfidenceComponent);
        }
    }


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
        InitFaceRegionConfidence();
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
        // Use stopwatch.Elapsed.TotalSeconds instead of Time.time
        currentTime = TimeManager.Instance.CurrentTime;

        // hit data
        // trackingLayerMask = LayerMask.GetMask("TrackingLayer");
        maxDistance = 1000;
        if (Physics.Raycast(leftEye.transform.position, leftEye.transform.forward, out hit, maxDistance) ||
            Physics.Raycast(rightEye.transform.position, rightEye.transform.forward, out hit, maxDistance)) {
            // filePathText.text +=  "\n\nHit Position:" + hit.point.ToString();
            hitObject = hit.collider.gameObject;
            // filePathText.text += "\nHit Object:" + hitObject;
            
            hitPosition = hit.point;
            currentTime = (float)TimeManager.Instance.CurrentTime;
            if (!hitTimeForObjects.ContainsKey(hitObject)) {
                // This is the first time the object is looked at and store current time
                hitTimeForObjects[hitObject].Add(currentTime);
            }
            else
            {
                // The object is being looked at, calculate the duration to the previous. Store current time to List of hit time for objects
                duration = currentTime - hitTimeForObjects[hitObject][-1];
                hitTimeForObjects[hitObject].Add(currentTime);
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

        foreach (FaceRegionConfidenceComponent comp in faceRegionConfidenceComponents)
        {
            float confidence;
            if (faceExpressions.TryGetWeightConfidence(comp.FaceRegionConfidence, out confidence))  // Get confidence value
            {
                comp.Confidence = confidence;  // Assign confidence value
            }
        }
        BufferFaceRegionConfidenceData();

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

        // Once you hit the write interval or max buffer size, write data to file
        if (frameCounter >= writeInterval)
        {
            var hitDataToWrite = new List<string>(hitDataBuffer);
            hitDataBuffer.Clear();
            WriteDataToFileAsync(hitDataFilePath, hitDataToWrite);

            var faceExpressionDataToWrite = new List<string>(faceExpressionDataBuffer);
            faceExpressionDataBuffer.Clear();
            WriteDataToFileAsync(faceExpressionDataFilePath, faceExpressionDataToWrite);

            var eyeTrackingDataToWrite = new List<string>(eyeTrackingDataBuffer);
            eyeTrackingDataBuffer.Clear();
            WriteDataToFileAsync(eyeTrackingDataFilePath, eyeTrackingDataToWrite);

            var headsetAndControllerDataToWrite = new List<string>(headsetAndControllerDataBuffer);
            headsetAndControllerDataBuffer.Clear();
            WriteDataToFileAsync(headsetAndControllerDataFilePath, headsetAndControllerDataToWrite);

            var headsetVelocityAndAccelerationDataToWrite = new List<string>(headsetVelocityAndAccelerationDataBuffer);
            headsetVelocityAndAccelerationDataBuffer.Clear();
            WriteDataToFileAsync(headsetVelocityAndAccelerationDataFilePath, headsetVelocityAndAccelerationDataToWrite);

            var faceRegionConfidenceDataToWrite = new List<string>(faceRegionConfidenceDataBuffer);
            WriteDataToFileAsync(faceRegionConfidenceDataFilePath, new List<string>(faceRegionConfidenceDataToWrite));
            faceRegionConfidenceDataBuffer.Clear();

            // Reset the frame counter
            frameCounter = 0;
        }
    }
    // Function to buffer face expression data
    void BufferHitData()
    {
        string hitData = $"{currentTime}, {currentTask}, {hitObject.name}, {hitPosition}, {duration}";
        hitDataBuffer.Add(hitData);
    }

    // Function to buffer face expression data
    void BufferFaceExpressionData()
    {
        string faceExpressionData = $"{currentTime}, {currentTask}";
        foreach (FaceWeightComponent comp in components)
        {
            faceExpressionData += $", {comp.Weight}";
        }
        faceExpressionDataBuffer.Add(faceExpressionData);
    }


    void WriteFaceRegionConfidenceDataToFile(string filePath)
    {
        System.IO.File.WriteAllLines(filePath, faceRegionConfidenceDataBuffer);
    }

    // Function to buffer eye tracking data
    void BufferEyeTrackingData()
    {
        string eyeTrackingData = $"{currentTime}, {currentTask}, {leftEyeGazeDirection}, {rightEyeGazeDirection}, {leftEyeConfidence}, {rightEyeConfidence}, {leftEyePosition}, {rightEyePosition}, {leftEyeRotation}, {rightEyeRotation}";
        eyeTrackingDataBuffer.Add(eyeTrackingData);
    }

    // Function to buffer headset and controller data
    void BufferHeadsetAndControllerData()
    {
        string headsetAndControllerData = $"{currentTime}, {currentTask}, {headsetPosition}, {headsetRotation}, {leftControllerPosition}, {leftControllerRotation}, {rightControllerPosition}, {rightControllerRotation}";
        headsetAndControllerDataBuffer.Add(headsetAndControllerData);
    }

    void BufferHeadsetVelocityAndAccelerationData()
    {
        string headsetVelocityAndAccelerationData = $"{currentTime}, {currentTask}, {headsetVelocity}, {headsetAcceleration}";
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


    void OnApplicationQuit()
    {
        try
        {
            // Synchronous write for hitDataBuffer
            if (hitDataBuffer.Count > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(hitDataFilePath, true))
                {
                    foreach (string data in hitDataBuffer)
                    {
                        outputFile.WriteLine(data);
                    }
                }
                hitDataBuffer.Clear();
            }

            // Synchronous write for faceExpressionDataBuffer
            if (faceExpressionDataBuffer.Count > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(faceExpressionDataFilePath, true))
                {
                    foreach (string data in faceExpressionDataBuffer)
                    {
                        outputFile.WriteLine(data);
                    }
                }
                faceExpressionDataBuffer.Clear();
            }

            // Synchronous write for eyeTrackingDataBuffer
            if (eyeTrackingDataBuffer.Count > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(eyeTrackingDataFilePath, true))
                {
                    foreach (string data in eyeTrackingDataBuffer)
                    {
                        outputFile.WriteLine(data);
                    }
                }
                eyeTrackingDataBuffer.Clear();
            }

            // Synchronous write for headsetAndControllerDataBuffer
            if (headsetAndControllerDataBuffer.Count > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(headsetAndControllerDataFilePath, true))
                {
                    foreach (string data in headsetAndControllerDataBuffer)
                    {
                        outputFile.WriteLine(data);
                    }
                }
                headsetAndControllerDataBuffer.Clear();
            }

            // Synchronous write for headsetVelocityAndAccelerationDataBuffer
            if (headsetVelocityAndAccelerationDataBuffer.Count > 0)
            {
                using (StreamWriter outputFile = new StreamWriter(headsetVelocityAndAccelerationDataFilePath, true))
                {
                    foreach (string data in headsetVelocityAndAccelerationDataBuffer)
                    {
                        outputFile.WriteLine(data);
                    }
                }
                headsetVelocityAndAccelerationDataBuffer.Clear();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error writing data to file: " + ex.Message);
        }
    }




}