using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR;

public class TrackingInputData : MonoBehaviour
{
    private InputData _inputData;

    private float dataLeftVelocity = 0f;

    private float dataRightVelocity = 0f;

    private float dataHMDPosition = 0f;

    bool introTask = false;
    bool SubtractionTask = false;
    bool KeyboardTask = false;
    bool FiitLawTask = false;

    bool leftControllerFound = false;
    bool rightControllerFound = false;
    bool HMDFound = false;



            float xLeftPosition = 0f;
            float yLeftPosition = 0f;
            float zLeftPosition = 0f;
            float xLeftRotation = 0f;
            float yLeftRotation = 0f;
            float zLeftRotation = 0f;
private string fileName;
private string filePath;

private float elapsedTime = 0f;




    // Start is called before the first frame update
    void Start()
    {
        fileName = "BLeftControllerData.txt";
        filePath = Application.dataPath + "/" + fileName;
        
        
        _inputData = GetComponent<InputData>();

        // Check if the file exists
        if (!File.Exists(filePath))
        {
            // If it doesn't exist, create the file and write the header
            File.WriteAllText(filePath, "Left Controller Velocity,Left Controller Position X,Left Controller Position Y,Left Controller Position Z,Left Controller Rotation X,Left Controller Rotation Y,Left Controller Rotation Z\n");
            Debug.Log("Created File: " + fileName + "at path: " + filePath);
        }

        Debug.Log(filePath);
    }

    private void WriteLineToFile(string filePath, string line)
    {
        using (StreamWriter writer = File.AppendText(filePath))
        {
            writer.WriteLine(line);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Timer
        elapsedTime += Time.deltaTime;
        
        // Left Controller Data

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity))
        {
            dataLeftVelocity = Mathf.Max(leftVelocity.magnitude, dataLeftVelocity);

            leftControllerFound = true;
        }

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPosition))
        {
            // Do something with the device position data
            xLeftPosition = leftPosition.x;
            yLeftPosition = leftPosition.y;
            zLeftPosition = leftPosition.z;
        }

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRotation))
        {
                      // Do something with the device rotation data
            Vector3 eulerAngles = leftRotation.eulerAngles;
            xLeftRotation = eulerAngles.x;
            yLeftRotation = eulerAngles.y;
            zLeftRotation = eulerAngles.z;
        }

        if (elapsedTime >= 0.25f && leftControllerFound) {
        // Write the data to file in the desired format
        string data = dataLeftVelocity + "," + xLeftPosition + "," + yLeftPosition + "," + zLeftPosition + "," + xLeftRotation + "," + yLeftRotation + "," + zLeftRotation;
        WriteLineToFile(filePath, data);
        elapsedTime = 0f;
        }

        // Right Controller Data

        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 rightVelocity))
        {
            dataRightVelocity = Mathf.Max(leftVelocity.magnitude, dataRightVelocity);
        }

        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPosition))
        {
            // Do something with the device position data
            float xRightPosition = rightPosition.x;
            float yRightPosition = rightPosition.y;
            float zRightPosition = rightPosition.z;
        }

        if (_inputData._rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRotation))
        {
            // Do something with the device rotation data
            Vector3 eulerAngles = rightRotation.eulerAngles;
            float xRightRotation = eulerAngles.x;
            float yRightRotation = eulerAngles.y;
            float zRightRotation = eulerAngles.z;
        }

        // HMD Data

        if (_inputData._HMD.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 HMDPosition))
        {
            // Do something with the device position data
            float xHMDPosition = HMDPosition.x;
            float yHMDPosition = HMDPosition.y;
            float zHMDPosition = HMDPosition.z;
        }  
    }
}
