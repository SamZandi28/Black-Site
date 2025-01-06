using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class NightVisionTrigger : MonoBehaviour
{
    public PostProcessVolume nightVisionVolume;  // Assign the Post-Processing Volume for night vision
    public Collider playerCollider;  // Reference to the XR Rig's collider (Capsule or Character Controller)

    void Start()
    {
        // Ensure night vision is initially disabled
        nightVisionVolume.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        // Check if the XR player enters the room
        if (other == playerCollider)
        {
            ActivateNightVision();
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Check if the XR player exits the room
        if (other == playerCollider)
        {
            DeactivateNightVision();
        }
    }

    void ActivateNightVision()
    {
        nightVisionVolume.enabled = true;
    }

    void DeactivateNightVision()
    {
        nightVisionVolume.enabled = false;
    }
}
