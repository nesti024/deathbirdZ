using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.Networking;

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

    public AudioSource birdAudioSourcePrefab;


    private List<string> audioFilePaths = new List<string>();

    private const int blendShapeIndex = 1;
    private const float blendShapeMax = 100f;
    private const float blendShapeMin = 0f;
    private const float blendShapeDuration = 1f;

    private int maxBirds;

    private bool soundActive = false;
    private float soundTimer = 0f;
    private const float soundThreshold = 0.1f; // Adjust as needed
    private const float birdSpawnInterval = 3f; // 3 seconds

    void Start()
    {
        if (userHead == null && Camera.main != null)
            userHead = Camera.main.transform;

        // Calculate max birds needed to cover the sphere
        float sphereArea = 4f * Mathf.PI * orbitRadius * orbitRadius;
        float birdArea = Mathf.PI * Mathf.Pow(birdDiameter / 2f, 2);
        maxBirds = Mathf.CeilToInt(sphereArea / birdArea);

        Debug.Log($"[BirdsLogic] Max birds to cover sphere: {maxBirds}");

        string audioFolder = Path.Combine(Application.streamingAssetsPath, "Audio/FX");
        if (Directory.Exists(audioFolder))
        {
            string[] files = Directory.GetFiles(audioFolder);
            foreach (var file in files)
            {
                if (file.EndsWith(".wav") || file.EndsWith(".mp3") || file.EndsWith(".ogg"))
                    audioFilePaths.Add(file.Replace("\\", "/")); // Normalize to forward slashes
            }
        }
        else
        {
            Debug.LogWarning($"[BirdsLogic] Audio folder not found: {audioFolder}");
        }

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

        // Detect if sound is "active"
        soundActive = velocity > soundThreshold;

        if (soundActive)
        {
            soundTimer += Time.deltaTime;
            if (soundTimer >= birdSpawnInterval)
            {
                AddBird();
                soundTimer = 0f;
            }
        }
        else
        {
            soundTimer = 0f; // Reset timer if sound stops
        }

        // Update all birds' positions
        for (int i = 0; i < birds.Count; i++)
        {
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

            // Apply additional -95 degree rotation on the X axis
            birds[i].Rotate(-95f, 0f, 0f, Space.Self);

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
        newBird.localScale = Vector3.one * 1.2f;
        birds.Add(newBird);
        orbitAngles.Add(Random.Range(0f, 360f));
        Debug.Log($"[BirdsLogic] Bird spawned. Total birds: {birds.Count}");

        // Attach random sound
        if (audioFilePaths.Count > 0)
        {
            string randomPath = audioFilePaths[Random.Range(0, audioFilePaths.Count)];
            Debug.LogWarning($"[BirdsLogic] Selected audio file: {randomPath}");
            StartCoroutine(AttachRandomSound(newBird.gameObject, randomPath));
        }

        // Start coroutine to destroy bird after a random time
        float lifetime = Random.Range(10f, 20f);
        StartCoroutine(DestroyBirdAfterTime(newBird, lifetime));
    }

    IEnumerator DestroyBirdAfterTime(Transform bird, float delay)
    {
        yield return new WaitForSeconds(delay);

        int index = birds.IndexOf(bird);
        if (index >= 0)
        {
            birds.RemoveAt(index);
            orbitAngles.RemoveAt(index);
        }
        Destroy(bird.gameObject);
    }

    IEnumerator AttachRandomSound(GameObject bird, string filePath)
    {
        string url = "file://" + filePath.Replace("\\", "/");
        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.LogWarning($"[BirdsLogic] Entered audio thing");
                AudioClip clip = DownloadHandlerAudioClip.GetContent(www);
                AudioSource source = bird.GetComponent<AudioSource>();
                if (source == null)
                    source = bird.AddComponent<AudioSource>();
                source.clip = clip;
                source.playOnAwake = false;


                // Start coroutine to play audio every 5 seconds
                StartCoroutine(PlayAudioEvery5Seconds(source));
            }
            else
            {
                Debug.LogWarning($"[BirdsLogic] Failed to load audio: {filePath} - {www.error}");
            }
        }
    }

    IEnumerator PlayAudioEvery5Seconds(AudioSource source)
    {
        // Wait until the clip is ready
        while (source.clip == null)
            yield return null;

        while (true)
        {
            
            source.Play();
            Debug.LogWarning($"[BirdsLogic] play audio"+ source.clip);
            yield return new WaitForSeconds(5f);
        }
    }

    void AnimateBlendShape(SkinnedMeshRenderer renderer)
    {
        float t = Mathf.PingPong(Time.time / blendShapeDuration, 1f);
        float value = Mathf.Lerp(blendShapeMin, blendShapeMax, t);
        renderer.SetBlendShapeWeight(blendShapeIndex, value);
    }
}
