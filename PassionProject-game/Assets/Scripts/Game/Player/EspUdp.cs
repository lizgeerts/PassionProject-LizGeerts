using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EspUdp : MonoBehaviour
{
    public int listenPort = 5005; // has to match remote port in ino file
    UdpClient udpClient;
    Thread receiveThread;
    bool running = false;
    public Image ErrorScreen;
    private float lastMessageTime = 0f;
    public float timeout = 2.0f;
    public GameObject MessageText;
    public GameObject InstructionsText;

    string latestMessage;
    bool hasMessage = false;

    // data
    public float ax, ay, az;
    public float gx, gy, gz;
    public int joystickDir;
    bool noMPUData = false;


    void Start()
    {
        lastMessageTime = Time.time;

        Debug.Log("test");
        udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, listenPort));

        running = true;

        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    void ReceiveLoop()
    {
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, listenPort);

        while (running)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEP);
                string msg = Encoding.UTF8.GetString(data);

                lock (this)
                {
                    latestMessage = msg;
                    hasMessage = true;
                }
            }
            catch (Exception)
            {
                // socket closed → exit thread
            }
        }
    }


    void ParseData(string msg)
    {
        string[] parts = msg.Split(',');
        if (parts.Length != 7) return;

        float.TryParse(parts[0], out ax);
        float.TryParse(parts[1], out ay);
        float.TryParse(parts[2], out az);
        float.TryParse(parts[3], out gx);
        float.TryParse(parts[4], out gy);
        float.TryParse(parts[5], out gz);

        int.TryParse(parts[6], out joystickDir);

        //Debug.Log($"timestamp:{Time.time} ax:{ax} ay:{ay} az:{az} gx:{gx} gy:{gy} gz:{gz}, dir:{joystickDir}");
        // Debug.Log($" ax:{ax} ay:{ay} az:{az} gx:{gx} gy:{gy} gz:{gz}, dir:{joystickDir}");
    }


    void OnDisable()
    {
        running = false;

        if (udpClient != null)
        {
            udpClient.Close();
            udpClient = null;
        }

        if (receiveThread != null)
        {
            receiveThread.Abort();
            receiveThread = null;
        }
    }


    void Update()
    {
        string msg = null;
        lock (this)
        {
            if (hasMessage)
            {
                msg = latestMessage;
                hasMessage = false;
            }
        }

        if (msg != null)
        {
            lastMessageTime = Time.time;
            ParseData(msg);
            noMPUData = ax==0 && ay ==0 && az == 0;
        }

        bool noConnection = Time.time - lastMessageTime > timeout;

        if (noConnection)
        {
            // No connection → show connection error
            ErrorScreen.gameObject.SetActive(true);
            onError(false, true);
        }
        else if (noMPUData)
        {
            // Connected, but MPU not sending data → show MPU error
            ErrorScreen.gameObject.SetActive(true);
            onError(true, false);
        }
        else
        {
            // Everything ok → hide error
            ErrorScreen.gameObject.SetActive(false);
        }
    }

    void onError(bool noMPUData, bool noConnection)
    {
        if (noConnection)
        {
            MessageText.GetComponent<TextMeshProUGUI>().SetText("No connection to ESP device!");
            InstructionsText.GetComponent<TextMeshProUGUI>().SetText("Check USB/Bluetooth and restart the game. Blue light = wifi connected");
        }
        else if (noMPUData)
        {
            MessageText.GetComponent<TextMeshProUGUI>().SetText("No data from MPU sensor!");
            InstructionsText.GetComponent<TextMeshProUGUI>().SetText("Ensure the MPU is powered on and sending data.");
        }
    }
}