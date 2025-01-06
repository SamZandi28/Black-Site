using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Keyboard
{
    public class KeyboardManagerRoomTwo : MonoBehaviour
    {
        [Header("Keyboard Setup")]
        [SerializeField] private KeyChannel keyChannel;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button[] numericButtons;
        [SerializeField] private TMP_InputField outputField;
        [SerializeField] private Button enterButton;

        [SerializeField] private int maxCharacters = 3;
        [SerializeField] private int minCharacters = 2;

        [Header("Keyboard Button Colors")]
        [SerializeField] private Color normalColor = Color.black;
        [SerializeField] private Color highlightedColor = Color.yellow;
        [SerializeField] private Color pressedColor = Color.red;
        [SerializeField] private Color selectedColor = Color.blue;

        [Header("Audio Settings")]
        [SerializeField] private AudioSource soundImmediately;
        [SerializeField] private AudioSource accessDeniedSound;
        [SerializeField] private AudioSource accessDeniedDelayedSound;
        [SerializeField] private AudioSource oneTimeAudio;
        [SerializeField] private float soundDelay = 1f;

        [Header("Door Animator")]
        [SerializeField] private Animator doorAnimator;

        private bool inputDisabled = false;
        private string correctCode = "871";

        public static bool isPuzzleSolved = false;

        private void Awake()
        {
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
            if (outputField.text.Length >= maxCharacters)
            {
                Debug.Log("Max characters reached, Access Denied will play on further input.");
                PlayAccessDeniedSound();
                return;
            }

            string numberPressed = pressedButton.GetComponentInChildren<TMP_Text>().text;

            int startPos = Mathf.Min(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);
            int endPos = Mathf.Max(outputField.selectionAnchorPosition, outputField.selectionFocusPosition);

            outputField.text = outputField.text.Remove(startPos, endPos - startPos);
            outputField.text = outputField.text.Insert(startPos, numberPressed);

            outputField.selectionAnchorPosition = outputField.selectionFocusPosition = startPos + numberPressed.Length;

            CheckTextLength();
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

                outputField.interactable = false;
                Debug.Log("Correct code remains visible in the input field.");


                isPuzzleSolved = true;

                // Play one-time audio response after deactivation
                PlayOneTimeAudio();

                // Trigger the door opening animation
                if (doorAnimator != null)
                {
                    doorAnimator.SetTrigger("isOpening");
                    Debug.Log("Door opening animation triggered.");
                }
                else
                {
                    Debug.LogWarning("Door Animator is not assigned.");
                }
            }
            else
            {
                Debug.Log("Incorrect Code Entered.");
                StartCoroutine(PlayDelayedAccessDeniedSound());
            }

            CheckTextLength();
        }

        private void PlayOneTimeAudio()
        {
            if (oneTimeAudio != null)
            {
                oneTimeAudio.Play();
                Debug.Log("One-time audio response played.");
            }
            else
            {
                Debug.LogWarning("One-time audio source is not assigned.");
            }
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
        }

        private void CheckTextLength()
        {
            int currentLength = outputField.text.Length;

            if (currentLength < maxCharacters)
            {
                EnableNumericButtons();
            }

            if (currentLength >= maxCharacters)
            {
                Debug.Log("Max characters reached.");
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
    }
}
