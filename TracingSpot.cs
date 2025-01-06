using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TracingSpot : MonoBehaviour
{
    public string correctPieceTag;
    private bool isFilled = false;

    public bool IsFilled => isFilled;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("big shit");
        if (other.CompareTag(correctPieceTag) && !isFilled)
        {
            Debug.Log($"Correct piece placed: {other.name} in spot: {gameObject.name}");

            isFilled = true;

            PlacePiece(other);
            TracingPanel.Instance.CheckAllSpots();
        }
    }

    private void PlacePiece(Collider piece)
    {
        piece.transform.position = transform.position;
        piece.transform.rotation = transform.rotation;

        XRGrabInteractable grabInteractable = piece.GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
            Debug.Log($"GrabInteractable disabled for: {piece.name}");
        }

        Rigidbody rb = piece.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            Debug.Log($"Rigidbody set to kinematic for: {piece.name}");
        }

        Collider collider = piece.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        piece.transform.SetParent(transform);
    }
    public void ResetSpot()
    {
        isFilled = false;
    }
}