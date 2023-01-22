using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TrackingInputData : MonoBehaviour
{
    private InputData _inputData;

    private float dataLeftVelocity = 0f;

    private float dataRightVelocity = 0f;

    private float dataHMDPosition = 0f;




    // Start is called before the first frame update
    void Start()
    {
        _inputData = GetComponent<InputData>();
    }

    // Update is called once per frame
    void Update()
    {

        // Left Controller Data

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceVelocity, out Vector3 leftVelocity))
        {
            dataLeftVelocity = Mathf.Max(leftVelocity.magnitude, dataLeftVelocity);
        }

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPosition))
        {
            // Do something with the device position data
            float xLeftPosition = leftPosition.x;
            float yLeftPosition = leftPosition.y;
            float zLeftPosition = leftPosition.z;
        }

        if (_inputData._leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRotation))
        {
                      // Do something with the device rotation data
            Vector3 eulerAngles = leftRotation.eulerAngles;
            float xLeftRotation = eulerAngles.x;
            float yLeftRotation = eulerAngles.y;
            float zLeftRotation = eulerAngles.z;
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
