using UnityEngine;
using TMPro; // For TextMeshProUGUI
// Make sure to add 'using OscJack;' if OscJack is in your project

public class Test : MonoBehaviour
{
    // Assign this in the Inspector
    public TextMeshProUGUI textMeshPro;

    // OSC port and address
    public int oscPort = 6969;
    public string oscAddress = "/velocity";

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

            // Assume the value is a float
            lastValue = data.GetElementAsFloat(0);
            
            
        
    }

    void Update()
    {
        if (textMeshPro != null)
        {

            textMeshPro.text = lastValue.ToString("F2");
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
