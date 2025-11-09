using UnityEngine;

public class BirdsLogic : MonoBehaviour
{
    // Assign the bird GameObject (with SkinnedMeshRenderer) in the Inspector
    public Transform bird;
    // Assign the user's head (camera) in the Inspector, or leave null to auto-detect
    public Transform userHead;
    // Assign the SkinnedMeshRenderer in the Inspector
    public SkinnedMeshRenderer birdRenderer;
    // Orbit parameters
    public float orbitRadius = 2.0f;
    public float orbitSpeed = 30.0f; // degrees per second
    private float orbitAngle;

    // Blend shape animation parameters
    private float blendShapeTimer = 0f;
    private bool goingUp = true;
    private const int blendShapeIndex = 1; // "key 1" is usually index 1
    private const float blendShapeMax = 100f;
    private const float blendShapeMin = 0f;
    private const float blendShapeDuration = 1f; // 1 second up, 1 second down

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Auto-assign user head if not set
        if (userHead == null && Camera.main != null)
        {
            userHead = Camera.main.transform;
        }
        // Start at a random angle for variety
        orbitAngle = Random.Range(0f, 360f);
    }

    // Update is called once per frame
    void Update()
    {
        if (bird == null || userHead == null)
            return;

        // Update orbit angle
        orbitAngle += orbitSpeed * Time.deltaTime;
        if (orbitAngle > 360f) orbitAngle -= 360f;

        // Calculate new position around the user's head
        float radians = orbitAngle * Mathf.Deg2Rad;
        Vector3 offset = new Vector3(Mathf.Cos(radians), 0.5f, Mathf.Sin(radians)) * orbitRadius;
        bird.position = userHead.position + offset;

        // Make the bird look at the user's head
        bird.LookAt(userHead.position);

        // Animate blend shape if renderer is assigned
        if (birdRenderer != null)
        {
            AnimateBlendShape();
        }
    }

    void AnimateBlendShape()
    {
        // Update timer
        blendShapeTimer += Time.deltaTime;
        float t = Mathf.Clamp01(blendShapeTimer / blendShapeDuration);
        float value;
        if (goingUp)
        {
            value = Mathf.Lerp(blendShapeMin, blendShapeMax, t);
        }
        else
        {
            value = Mathf.Lerp(blendShapeMax, blendShapeMin, t);
        }
        birdRenderer.SetBlendShapeWeight(blendShapeIndex, value);

        if (blendShapeTimer >= blendShapeDuration)
        {
            blendShapeTimer = 0f;
            goingUp = !goingUp;
        }
    }
}
