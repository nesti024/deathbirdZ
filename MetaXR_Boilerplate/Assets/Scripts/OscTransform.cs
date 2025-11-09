using TMPro;
using UnityEngine;

public class OscTransform : MonoBehaviour
{
    public float scaleAmount = 0.3f;
    private float scale = 0.0f;
    private Vector3 scale0;

    public TextMeshProUGUI textMeshPro;

    private Vector3 p0; // original position

   void Start()
    {
        // Store the starting local position once
        p0 = transform.localPosition;
        scale0 = transform.localScale;
    }

    public void SetScale(float v)
    {
        Debug.Log("osc received, set scale: " + v);
        //scale = v;
        textMeshPro.text = v.ToString("F2");
    }

    void Update()
    {
           // transform.localScale = scale0 * (1.0f + scale*scaleAmount);

    }
}

