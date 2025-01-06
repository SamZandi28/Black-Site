using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Keyboard
{
    public class KeyboardManagerTwo : MonoBehaviour
    {
        [Header("Keyboard Setup")]
        [SerializeField] private KeyChannel keyChannel;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button[] numericButtons;
        [SerializeField] private TMP_InputField outputField;
        [SerializeField] private Button enterButton;
        [SerializeField] private Button shiftButton;

        [SerializeField] private int maxCharacters = 4;
        [SerializeField] private int minCharacters = 3;

        [Header("Keyboard Button Colors")]
        [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color highlightedColor = Color.yellow;
        [SerializeField] private Color pressedColor = Color.red;
        [SerializeField] private Color selectedColor = Color.blue;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource soundImmediately;
        [SerializeField] private AudioSource soundWithDelay;
        [SerializeField] private AudioSource accessDeniedSound;
        [SerializeField] private AudioSource accessDeniedDelayedSound;
        [SerializeField] private float soundDelay = 1f;

        [Header("Material Change Setup")]
        [SerializeField] private GameObject tvGameObject;  // The actual TV GameObject
        [SerializeField] private Material oneKeyMaterial;
        [SerializeField] private Material twoKeysMaterial;
        [SerializeField] private Material threeKeysMaterial;
        [SerializeField] private Material fourKeysMaterial;
        [SerializeField] private Material correctCodeMaterial; // Add this material for correct code

        private bool inputDisabled = false;
        private string correctCode = "1115";
        private bool shiftPressed = false;
        private string numberToDisplay = "7";

        public static bool isPuzzleSolved = false; // Added static variable to track puzzle status

        private void Awake()
        {
            ResetShiftButton();
            CheckTextLength();

            deleteButton.onClick.AddListener(OnDeletePress);
            enterButton.onClick.AddListener(OnEnterPress);
            keyChannel.RaiseKeyColorsChangedEvent(normalColor, highlightedColor, pressedColor, selectedColor);

            foreach (var button in numericButtons)
            {
                button.onClick.AddListener(() =>
                {
                    if (!inputDisabled) OnNumericButtonPress(button);
                });
            }
        }

        private void OnDestroy()
        {
            foreach (var button in numericButtons)
            {
                button.onClick.RemoveAllListeners();
            }

            deleteButton.onClick.RemoveListener(OnDeletePress);
            enterButton.onClick.RemoveListener(OnEnterPress);
        }

        private void OnNumericButtonPress(Button pressedButton)
        {
            // Check if input has already reached maxCharacters
            if (outputField.text.Length >= maxCharacters)
            {
                Debug.Log("Max characters reached, Access Denied will play on further input.");

                // If player presses more than 4 keys, play Access Denied Sound
                PlayAccessDeniedSound();
                return; // Stop adding characters after maxCharacters
            }

            string numberPressed = pressedButton.GetComponentInChildren<TMP_Text>().text;

            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            outputField.text = outputField.text.Remove(startPos, endPos - startPos);
            outputField.text = outputField.text.Insert(startPos, numberPressed);

            outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos + numberPressed.Length;

            CheckTextLength();
            ChangeTVMaterialBasedOnInput();  // Update TV material based on input length
        }

        private void PlayAccessDeniedSound()
        {
            if (accessDeniedSound != null)
            {
                accessDeniedSound.Play();
                Debug.Log("Access Denied Sound Played.");
            }
        }

        private void OnEnterPress()
        {
            if (outputField.text == correctCode && outputField.text.Length == maxCharacters)
            {
                Debug.Log("Correct Code Entered.");
                inputDisabled = true;
                DisableAllButtons();

                if (soundImmediately != null)
                {
                    soundImmediately.Play();
                    Debug.Log("Immediate Sound Played.");
                }
                if (soundWithDelay != null)
                {
                    StartCoroutine(PlayDelayedSound());
                }

                outputField.text = numberToDisplay;
                outputField.interactable = false;
                Debug.Log($"Displayed number '{numberToDisplay}' after correct code.");

                // Change TV material for the correct code
                if (tvGameObject != null)
                {
                    Renderer tvRenderer = tvGameObject.GetComponent<Renderer>(); // Fetch the Renderer component
                    if (tvRenderer != null)
                    {
                        tvRenderer.material = correctCodeMaterial;  // Assign the correct material when the correct code is entered
                        Debug.Log("TV material changed after correct code.");
                    }
                }

                isPuzzleSolved = true; // Mark the puzzle as solved
                KeyboardManagerOne.isPuzzleSolved = true; // Mark the puzzle as solved in KeyboardManagerOne
            }
            else
            {
                Debug.Log("Incorrect Code Entered.");
                StartCoroutine(PlayDelayedAccessDeniedSound()); // Only play delayed Access Denied sound for incorrect code
            }

            CheckTextLength();
        }

        private IEnumerator PlayDelayedSound()
        {
            yield return new WaitForSeconds(soundDelay);
            soundWithDelay.Play();
            Debug.Log("Delayed Sound Played.");
        }

        private IEnumerator PlayDelayedAccessDeniedSound()
        {
            yield return new WaitForSeconds(soundDelay);
            accessDeniedDelayedSound.Play();
            Debug.Log("Delayed Access Denied Sound Played.");
        }

        private void OnDeletePress()
        {
            if (string.IsNullOrEmpty(outputField.text)) return;

            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            if (endPos > startPos)
            {
                outputField.text = outputField.text.Remove(startPos, endPos - startPos);
                outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos;
            }
            else if (startPos > 0)
            {
                outputField.text = outputField.text.Remove(startPos - 1, 1);
                outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos - 1;
            }

            CheckTextLength();
            ChangeTVMaterialBasedOnInput();  // Update TV material after deletion
        }

        private void CheckTextLength()
        {
            int currentLength = outputField.text.Length;

            // Enable buttons if fewer than max characters
            if (currentLength < maxCharacters)
            {
                EnableNumericButtons();
            }

            // No sound plays on 4th key press, but it plays on attempts after maxCharacters
            if (currentLength >= maxCharacters)
            {
                Debug.Log("Max characters reached.");
                // Don't play sound immediately for exactly 4 keys
            }
            else if (currentLength < minCharacters)
            {
                DisableEnterButton();
            }
            else
            {
                EnableEnterButton();
            }
        }

        private void ChangeTVMaterialBasedOnInput()
        {
            int inputLength = outputField.text.Length;

            if (tvGameObject != null)
            {
                Renderer tvRenderer = tvGameObject.GetComponent<Renderer>();  // Fetch the Renderer component
                if (tvRenderer != null)
                {
                    if (inputLength == 1)
                    {
                        tvRenderer.material = oneKeyMaterial;
                        Debug.Log("Changed TV material for 1 key.");
                    }
                    else if (inputLength == 2)
                    {
                        tvRenderer.material = twoKeysMaterial;
                        Debug.Log("Changed TV material for 2 keys.");
                    }
                    else if (inputLength == 3)
                    {
                        tvRenderer.material = threeKeysMaterial;
                        Debug.Log("Changed TV material for 3 keys.");
                    }
                    else if (inputLength == 4)
                    {
                        tvRenderer.material = fourKeysMaterial;
                        Debug.Log("Changed TV material for 4 keys.");
                    }
                }
            }
        }

        private void DisableAllButtons()
        {
            foreach (var button in numericButtons)
            {
                button.interactable = false;
            }

            deleteButton.interactable = false;
            Debug.Log("All buttons disabled.");
        }

        private void EnableNumericButtons()
        {
            foreach (var button in numericButtons)
            {
                button.interactable = true;
            }
        }

        private void EnableEnterButton()
        {
            enterButton.interactable = true;
        }

        private void DisableEnterButton()
        {
            enterButton.interactable = false;
        }

        private void ResetShiftButton()
        {
            shiftPressed = false;
        }
    }
}
