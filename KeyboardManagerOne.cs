using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Keyboard
{
    public class KeyboardManagerOne : MonoBehaviour
    {
        [Header("Keyboard Setup")]
        [SerializeField] private KeyChannel keyChannel;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button[] numericButtons;
        [SerializeField] private TMP_InputField outputField;
        [SerializeField] private Button enterButton;
        [SerializeField] private Button shiftButton;

        [SerializeField] private int maxCharacters = 15;
        [SerializeField] private int minCharacters = 3;

        [Header("Keyboard Button Colors")]
        [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color highlightedColor = Color.yellow;
        [SerializeField] private Color pressedColor = Color.red;
        [SerializeField] private Color selectedColor = Color.blue;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource soundImmediately;
        [SerializeField] private AudioSource soundWithDelay;
        [SerializeField] private float soundDelay = 1f;

        [Header("TV Material Change")]
        [SerializeField] private GameObject tvObject;
        [SerializeField] private Material newTVMaterial;

        public static bool isPuzzleSolved = false; // New variable to track if the puzzle is solved

        private bool inputDisabled = false;
        private bool shiftPressed = false;
        private Renderer tvRenderer;

        private bool materialChanged = false;

        private void Awake()
        {
            if (tvObject != null)
            {
                tvRenderer = tvObject.GetComponent<Renderer>();
            }

            DisableNumericAndDeleteButtons();

            shiftButton.onClick.AddListener(OnShiftPress);
            keyChannel.RaiseKeyColorsChangedEvent(normalColor, highlightedColor, pressedColor, selectedColor);
        }

        private void OnDestroy()
        {
            shiftButton.onClick.RemoveListener(OnShiftPress);
        }

        private void DisableNumericAndDeleteButtons()
        {
            foreach (var button in numericButtons)
            {
                button.interactable = false;
            }

            deleteButton.interactable = false;

            Debug.Log("Numeric and Delete buttons disabled.");
        }

        private void OnShiftPress()
        {
            if (shiftPressed) return;

            Debug.Log("Shift Button Pressed");

            DisableInput();
            PlaySounds();
            ChangeTVMaterial();

            shiftPressed = true;
        }

        private void ChangeTVMaterial()
        {
            if (tvRenderer != null && newTVMaterial != null && !materialChanged)
            {
                tvRenderer.material = newTVMaterial;
                materialChanged = true;
                isPuzzleSolved = true; // Mark the puzzle as solved
                Debug.Log("TV Material changed.");
            }
            else
            {
                Debug.LogWarning("TV Object or New TV Material is not assigned.");
            }
        }

        private void DisableInput()
        {
            inputDisabled = true;
            Debug.Log("Input Disabled after Shift Press.");
            deleteButton.interactable = false;
        }

        private void PlaySounds()
        {
            if (soundImmediately != null)
            {
                soundImmediately.Play();
                Debug.Log("Immediate Sound Played.");
            }

            if (soundWithDelay != null)
            {
                StartCoroutine(PlayDelayedSound());
            }
        }

        private IEnumerator PlayDelayedSound()
        {
            yield return new WaitForSeconds(soundDelay);
            soundWithDelay.Play();
            Debug.Log("Delayed Sound Played.");
        }
    }
}
