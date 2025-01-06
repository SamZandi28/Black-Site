using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Hand : MonoBehaviour
{
    public GameObject handPrefab;

    public InputDeviceCharacteristics inputDeviceCharacteristics;

    private InputDevice _targetDevice;

    private Animator _handAnimator;

    void Start()
    {
        InitializeHand();
    }

    private void InitializeHand()
    {
        List<InputDevice> devices = new List<InputDevice>();

        InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, devices);

        if (devices.Count > 0)
        {
            Debug.Log("Devices found: " + devices.Count);

            _targetDevice = devices[0];

            // Instantiate the new hand prefab and ensure it is active
            GameObject spawnedHand = Instantiate(handPrefab, transform);
            spawnedHand.SetActive(true);
            _handAnimator = spawnedHand.GetComponent<Animator>();

            // Deactivate the default hand model only after the new hand is instantiated
            Transform handLNoAnim = transform.Find("Hand_L_NoAnim");
            if (handLNoAnim != null)
            {
                Transform leftHand = handLNoAnim.Find("left_hand");
                if (leftHand != null)
                {
                    Debug.Log("Deactivating default left hand model: " + leftHand.name);
                    leftHand.gameObject.SetActive(false);
                }
                else
                {
                    Debug.Log("left_hand not found under Hand_L_NoAnim");
                }
            }
            else
            {
                Debug.Log("Hand_L_NoAnim not found under the controller");
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
        if (!_targetDevice.isValid)
        {
            InitializeHand();
        }
        else 
        {
            UpdateHand();
        }
    }

    private void UpdateHand()
    {
        if (_targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue)) 
        {
            Debug.Log("Trigger Value: " + triggerValue);
            _handAnimator.SetFloat("Trigger", triggerValue);
        }
        else 
        {
            _handAnimator.SetFloat("Trigger", 0);
            Debug.Log("Trigger not found");
        }
        if (_targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
        {
            Debug.Log("Grip Value: " + gripValue);  // Log grip value to verify input
            _handAnimator.SetFloat("Grip", gripValue);
        }
        else 
        {
            Debug.Log("Grip not found");  // Log if grip is not detected
            _handAnimator.SetFloat("Grip", 0);
        }
    }
}
