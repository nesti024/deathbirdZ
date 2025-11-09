using System.Collections.Generic;
using UnityEngine;

public class BirdsLogic : MonoBehaviour
{
    public Transform birdPrefab;
    public Transform userHead;
    private float orbitRadius = 1f;
    public float orbitSpeed = 30.0f;
    public AudioBeatController audioController;
    private float birdDiameter = 1.2f; // Set this to your bird's approximate width

    private List<Transform> birds = new List<Transform>();
    private List<float> orbitAngles = new List<float>();

    private const int blendShapeIndex = 1;
    private const float blendShapeMax = 100f;
    private const float blendShapeMin = 0f;
    private const float blendShapeDuration = 1f;

    private int maxBirds;

    void Start()
    {
        if (userHead == null && Camera.main != null)
            userHead = Camera.main.transform;

        // Calculate max birds needed to cover the sphere
        float sphereArea = 4f * Mathf.PI * orbitRadius * orbitRadius;
        float birdArea = Mathf.PI * Mathf.Pow(birdDiameter / 2f, 2);
        maxBirds = Mathf.CeilToInt(sphereArea / birdArea);

        Debug.Log($"[BirdsLogic] Max birds to cover sphere: {maxBirds}");

        if (birdPrefab != null)
            AddBird();
    }

    void Update()
    {
        if (userHead == null || audioController == null || birdPrefab == null)
            return;

        if (audioController == null) return;

        // Example: Use weighted energy as velocity
        float velocity = Mathf.Clamp01(
            0.6f * audioController.bass +
            0.3f * audioController.mid +
            0.1f * audioController.high
        );

        if (velocity > 0.1f)
        {
            //if (birds.Count < maxBirds)
            //{
                AddBird();
                AddBird();
                AddBird();
                AddBird();
 //           }

        }
        // Spawn birds until the sphere is full
  

        // Update all birds' positions
        for (int i = 0; i < birds.Count; i++)
        {
            // Clamp latitude to avoid poles (e.g., between 10 and 170 degrees)
            float minPhi = Mathf.Deg2Rad * 10f;
            float maxPhi = Mathf.Deg2Rad * 170f;
            float t = (i + 0.5f) / birds.Count;
            float phi = Mathf.Lerp(minPhi, maxPhi, t);

            // Fibonacci longitude with slight random offset
            float theta = Mathf.PI * (1 + Mathf.Sqrt(5)) * (i + 0.5f);

            Vector3 dir = new Vector3(
                Mathf.Cos(theta) * Mathf.Sin(phi),
                Mathf.Cos(phi),
                Mathf.Sin(theta) * Mathf.Sin(phi)
            );

            // Animate orbit
            float angle = orbitAngles[i] += orbitSpeed * Time.deltaTime;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 offset = rot * dir * orbitRadius;

            birds[i].position = userHead.position + offset;
            birds[i].LookAt(userHead.position);

            // Animate blend shape if renderer is assigned
            var smr = birds[i].GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
            {
                //AnimateBlendShape(smr);
            }
        }
    }

    void AddBird()
    {
        Transform newBird = Instantiate(birdPrefab, userHead.position, Quaternion.identity, transform);
        // Set scale to 1.2 for consistency
        newBird.localScale = Vector3.one * 1.2f; 
        //newBird.localScale = Vector3.one;
        birds.Add(newBird);
        orbitAngles.Add(Random.Range(0f, 360f));
        Debug.Log($"[BirdsLogic] Bird spawned. Total birds: {birds.Count}");
    }

    void AnimateBlendShape(SkinnedMeshRenderer renderer)
    {
        float t = Mathf.PingPong(Time.time / blendShapeDuration, 1f);
        float value = Mathf.Lerp(blendShapeMin, blendShapeMax, t);
        renderer.SetBlendShapeWeight(blendShapeIndex, value);
    }
}
