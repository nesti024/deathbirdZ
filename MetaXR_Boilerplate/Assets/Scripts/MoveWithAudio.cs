using UnityEngine;

public class MoveWithAudio : MonoBehaviour
{
    public AudioBeatController audioController;
    public Vector3 direction = Vector3.up;
    public float movementScale = 5f;

    private SkinnedMeshRenderer skinnedMeshRenderer;

    void Start()
    {
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
    }

    void Update()
    {
        if (audioController == null) return;

        // Example: Use weighted energy as velocity
        float velocity = Mathf.Clamp01(
            0.6f * audioController.bass +
            0.3f * audioController.mid +
            0.1f * audioController.high
        );

        // Move the object
        transform.position += direction * velocity * movementScale * Time.deltaTime;

        // Gradually change blend shape key 1 from 0 to 100 and back to 0
        if (skinnedMeshRenderer != null)
        {
            float pingPongValue = Mathf.PingPong(Time.time * 50f, 100f); // Adjust speed as needed
            skinnedMeshRenderer.SetBlendShapeWeight(1, pingPongValue);
        }
    }
}