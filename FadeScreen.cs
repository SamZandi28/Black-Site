using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeScreen : MonoBehaviour
{
    public Image fadeImage;          // Reference to the black fade image in the UI Canvas
    public float fadeDuration = 1.5f; // Duration of the fade effect
    public Animator doorAnimator;     // Reference to the door animation controller
    private bool isTransitioning = false;

    [SerializeField] private RoomTrigger roomTrigger; // Reference to RoomTrigger

    private void OnTriggerEnter(Collider other)
    {
        // Trigger fade and door animation only if the player collides with the door and shaderObjectDeactivated is true
        if (other.CompareTag("Player") && !isTransitioning && roomTrigger != null && roomTrigger.shaderObjectDeactivated)
        {
            isTransitioning = true;
            StartCoroutine(PlayDoorAndFadeOut());
        }
    }

    private IEnumerator PlayDoorAndFadeOut()
    {
        // Start the door animation
        doorAnimator.SetTrigger("isOpening");

        // Wait for a moment to sync with the door animation timing (adjust as needed)
        yield return new WaitForSeconds(1.0f);

        // Fade to black
        yield return StartCoroutine(FadeToBlack());
    }

    private IEnumerator FadeToBlack()
    {
        float elapsedTime = 0f;
        Color fadeColor = fadeImage.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeColor.a = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeImage.color = fadeColor;
            yield return null;
        }
    }
}
