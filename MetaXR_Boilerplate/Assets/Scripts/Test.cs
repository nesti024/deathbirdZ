using UnityEngine;
using TMPro; // For TextMeshProUGUI
// Make sure to add 'using OscJack;' if OscJack is in your project

public class Test : MonoBehaviour
{
    // Assign this in the Inspector
    public TextMeshProUGUI textMeshPro;

    // OSC port and address
    public int oscPort = 6969;
    public string oscAddress = "/test";

    private OscJack.OscServer server;
    private float lastValue;
    private bool valueUpdated = false;
    private readonly object valueLock = new object();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start OSC server
        server = new OscJack.OscServer(oscPort);
        server.MessageDispatcher.AddCallback(oscAddress, OnOscMessageReceived);
    }

    void OnOscMessageReceived(string address, OscJack.OscDataHandle data)
    {
        lock (valueLock)
        {
            // Assume the value is a float
            lastValue = data.GetElementAsFloat(0);
            valueUpdated = true;
        }
    }

    void Update()
    {
        if (textMeshPro != null)
        {
            bool updated = false;
            float value = 0f;
            lock (valueLock)
            {
                if (valueUpdated)
                {
                    value = lastValue;
                    updated = true;
                    valueUpdated = false;
                }
            }
            if (updated)
            {
                textMeshPro.text = value.ToString("F2");
            }
        }
    }

    void OnDestroy()
    {
        if (server != null)
        {
            server.Dispose();
            server = null;
        }
    }
}
