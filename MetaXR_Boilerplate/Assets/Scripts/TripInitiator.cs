using UnityEngine;
using System.Collections;

public class TripInitiator : MonoBehaviour
{
    [Tooltip("Reference to the PassthroughControl component to modify.")]
    public PassthroughControl passthroughControl;

    [Tooltip("Tag of the player GameObject.")]
    public string playerTag = "Player";

    [Tooltip("Duration in seconds for the intensity to reach 1.")]
    public float increaseDuration = 1.0f;

    private Coroutine intensityCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && passthroughControl != null)
        {
            if (intensityCoroutine != null)
                StopCoroutine(intensityCoroutine);

            intensityCoroutine = StartCoroutine(IncreaseIntensity());
        }
    }

    private IEnumerator IncreaseIntensity()
    {
        float elapsed = 0f;
        float startValue = passthroughControl.intensity;
        float endValue = 1f;

        while (elapsed < increaseDuration)
        {
            elapsed += Time.deltaTime;
            passthroughControl.intensity = Mathf.Lerp(startValue, endValue, elapsed / increaseDuration);
            yield return null;
        }
        passthroughControl.intensity = endValue;
        intensityCoroutine = null;
    }
}
