using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Keyboard
{
    public class KeyboardManagerThree : MonoBehaviour
    {
        [Header("TV Setup")]
        [SerializeField] private GameObject tvObject;         // TV object to change material
        [SerializeField] private Material newTVMaterial;      // New material for the TV

        [Header("Audio Setup")]
        [SerializeField] private AudioSource pickupSound;     // Audio played when the note is picked up
        [SerializeField] private KeyboardManagerOne keyboardManagerOne; // Reference to KeyboardManagerOne
        [SerializeField] private KeyboardManagerTwo keyboardManagerTwo; // Reference to KeyboardManagerTwo

        private bool notePickedUp = false;                    // To prevent multiple triggers

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;          // Reference to the XRGrabInteractable

        private MeshRenderer tvRenderer;                      // Cached TV renderer to avoid repeated GetComponent calls

        private void Awake()
        {
            // Get the XRGrabInteractable component attached to the note
            grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

            // Cache the TV's MeshRenderer for performance
            tvRenderer = tvObject.GetComponent<MeshRenderer>();

            // Ensure the grabInteractable exists
            if (grabInteractable != null)
            {
                // Subscribe to the grab event (when the player grabs the note)
                grabInteractable.selectEntered.AddListener(OnNoteGrabbed);
            }
            else
            {
                Debug.LogError("XRGrabInteractable component is missing from the note object.");
            }

            // Check if the TV object and new material are properly set
            if (tvRenderer == null || newTVMaterial == null)
            {
                Debug.LogWarning("TV object or new material is not assigned.");
            }
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event to avoid memory leaks
            if (grabInteractable != null)
            {
                grabInteractable.selectEntered.RemoveListener(OnNoteGrabbed);
            }
        }

        // This method is called when the player grabs the note
        private void OnNoteGrabbed(SelectEnterEventArgs args)
        {
            if (notePickedUp)  // Ensure this happens only once
            {
                Debug.Log("Note has already been picked up. Ignoring further interactions.");
                return;
            }

            // Check if puzzles are solved
            if (KeyboardManagerOne.isPuzzleSolved && KeyboardManagerTwo.isPuzzleSolved)
            {
                notePickedUp = true;

                // Change the TV's material
                ChangeTVMaterial();

                // Play the pickup sound
                if (pickupSound != null)
                {
                    pickupSound.Play();
                }

                Debug.Log("Note picked up, TV material changed and sound played.");
            }
            else
            {
                Debug.Log("Cannot pick up note. Puzzles not solved.");
            }
        }

        // This method changes the material of the TV
        private void ChangeTVMaterial()
        {
            if (tvRenderer != null && newTVMaterial != null)
            {
                // Use sharedMaterial to change the material for all instances, avoiding flickering
                if (tvRenderer.sharedMaterial != newTVMaterial)
                {
                    tvRenderer.sharedMaterial = newTVMaterial;  // Assign the new material to the TV
                    Debug.Log("TV Material changed.");
                }
                else
                {
                    Debug.Log("TV Material is already set. No change needed.");
                }
            }
            else
            {
                Debug.LogWarning("TV object or new TV material is not assigned.");
            }
        }
    }
}
