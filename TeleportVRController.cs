using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportVRController : MonoBehaviour
{
    // Reference to the target position or area where the controller should be teleported
    public Transform targetSpawnPoint;

    // Offset to adjust the player's height after teleport
    [SerializeField] private float yOffset = -1.0f; // Negative value to lower the player's position

    // Reference to the player's VR Rig (the object that will actually be teleported)
    public XROrigin _xrOrigin;

    // Health reduction amount per teleport
    public float healthLossPerTeleport = 10f;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has enough health to teleport
        if (HealthManager.instance.currentHealth > 0)
        {
            // Teleport the VR Rig to the target position
            TeleportVRRig();

            
            HealthManager.instance.TakeDamage(healthLossPerTeleport);
        }
        else
        {
           
            Debug.Log("Not enough health to teleport!");
        }
    }

    // Teleports the entire VR rig to the target spawn point
    private void TeleportVRRig()
    {
        Debug.Log($"orb position {targetSpawnPoint.position}");

        // Adjust the Y position to lower the player to a better height
        Vector3 adjustedPosition = targetSpawnPoint.position;
        adjustedPosition.y += yOffset; // Apply the Y offset to lower the player

        Debug.Log($"adjustedPosition {adjustedPosition}");

        // Move the entire VR rig (e.g., XR Rig) to the adjusted target position
        _xrOrigin.transform.position = adjustedPosition;
        _xrOrigin.transform.rotation = targetSpawnPoint.rotation;
        _xrOrigin.MoveCameraToWorldLocation(adjustedPosition);
    }
}