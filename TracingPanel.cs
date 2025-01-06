using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class TracingPanel : MonoBehaviour
{
    public static TracingPanel Instance;

    private List<TracingSpot> tracingSpots;

    [Header("Door Animator")]
    [SerializeField] private Animator doorAnimator;

    public GameObject assembledPuzzlePrefab;
    private int placedPieceCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        tracingSpots = new List<TracingSpot>(FindObjectsOfType<TracingSpot>());

        // Make the assembled puzzle prefab invisible at the start
        if (assembledPuzzlePrefab != null)
        {
            assembledPuzzlePrefab.SetActive(false);
        }
    }

    public void CheckAllSpots()
    {
        foreach (var spot in tracingSpots)
        {
            if (!spot.IsFilled)
                return;
        }

        AssemblePuzzle();
    }

    private void AssemblePuzzle()
    {
        placedPieceCount++;
        if (placedPieceCount >= tracingSpots.Count)
        {
            HidePlaceholders();
            DestroyPuzzlePieces();
        }

        if (assembledPuzzlePrefab != null)
        {
            // Make the assembled puzzle prefab visible
            assembledPuzzlePrefab.SetActive(true);

            // Optionally set custom position, rotation, and scale if needed
           // assembledPuzzlePrefab.transform.position = new Vector3(0, 1, 0);
           // assembledPuzzlePrefab.transform.localScale = new Vector3(35f, 35f, 35f);

            SetupFinalPiece(assembledPuzzlePrefab);

           
        }

        // Trigger door opening animation
        if (doorAnimator != null)
        {
            doorAnimator.SetTrigger("isOpening");
            Debug.Log("Door opening animation triggered.");
        }
        else
        {
            Debug.LogWarning("Door Animator is not assigned.");
        }

        foreach (var spot in tracingSpots)
        {
            spot.ResetSpot();
        }

        Destroy(gameObject);
    }

    private void SetupFinalPiece(GameObject finalPiece)
    {
        Rigidbody rb = finalPiece.AddComponent<Rigidbody>();
        rb.isKinematic = false; // Set to allow interaction

        BoxCollider boxCollider = finalPiece.AddComponent<BoxCollider>();
        boxCollider.isTrigger = false; // Make it a solid collider

        XRGrabInteractable grabInteractable = finalPiece.AddComponent<XRGrabInteractable>();
        grabInteractable.interactionLayers = LayerMask.GetMask("Default"); // Set interaction layer as needed
    }

    private void HidePlaceholders()
    {
        gameObject.SetActive(false);
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void DestroyPuzzlePieces()
    {
        foreach (var spot in tracingSpots)
        {
            if (spot.transform.childCount > 0)
            {
                Transform piece = spot.transform.GetChild(0);
                Destroy(piece.gameObject);

            }
        }
    }
}
