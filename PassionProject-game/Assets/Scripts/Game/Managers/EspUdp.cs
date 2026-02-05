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
    public float timeout = 3f;
    private string latestMessage;
    private bool hasMessage;
    public GameObject MessageText;
    public GameObject InstructionsText;


    // data
    public float ax, ay, az;
    public float gx, gy, gz;

    [Header("ESP Data")]
    public EspData esp1 = new EspData();  // Player 1
    public EspData esp2 = new EspData();  // Player 2 (optional)

    [Serializable]
    public class EspData
    {
        public string deviceId;
        public float ax, ay, az, gx, gy, gz;
        public float lastMessageTime;
        public bool active;
        public bool noMPUData = false;
    }

    public GameManager gameManager;


    void Start()
    {
        esp1.lastMessageTime = Time.time;
        esp2.lastMessageTime = Time.time;

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


    void RouteMessage(string msg)
    {
        string[] parts = msg.Split(',');
        if (parts.Length != 7) return;  // Need ID + 6 values

        string deviceId = parts[6];  // 1 or 2

        if (deviceId == "1")
        {
            esp1.lastMessageTime = Time.time;
            ParseData(parts, esp1);  // parse ax,ay,...
        }
        else if (deviceId == "2")
        {
            esp2.lastMessageTime = Time.time;
            ParseData(parts, esp2);
        }
    }

    void ParseData(string[] parts, EspData target)
    {
        float.TryParse(parts[0], out target.ax);
        float.TryParse(parts[1], out target.ay);
        float.TryParse(parts[2], out target.az);
        float.TryParse(parts[3], out target.gx);
        float.TryParse(parts[4], out target.gy);
        float.TryParse(parts[5], out target.gz);

        target.active = true;
        target.deviceId = parts[6];
       // Debug.Log($" ax:{target.ax} ay:{target.ay} az:{target.az} gx:{target.gx} gy:{target.gy} gz:{target.gz}");
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
            RouteMessage(msg);
        }

        bool anyNoConnection = Time.time - esp1.lastMessageTime > timeout;
        bool anyNoMPU = esp1.noMPUData;

        if (gameManager.gameIsMultiplayer)
        {
            anyNoConnection |= Time.time - esp2.lastMessageTime > timeout;
            anyNoMPU |= esp2.noMPUData;
        }

        if (anyNoConnection)
        {
            // No connection, show connection error
            ErrorScreen.gameObject.SetActive(true);
            onError(false, true);
        }
        else if (anyNoMPU)
        {
            // Connected, but MPU not sending data, show MPU error
            ErrorScreen.gameObject.SetActive(true);
            onError(true, false);
        }
        else
        {
            // Everything ok, hide error
            ErrorScreen.gameObject.SetActive(false);
        }
    }

    void onError(bool noMPUData, bool noConnection)
    {
        if (noConnection)
        {
            MessageText.GetComponent<TextMeshProUGUI>().SetText("No connection to ESP device!");
            InstructionsText.GetComponent<TextMeshProUGUI>().SetText("Check wifi connection and reset the ESP32. Blue light = wifi connected");
        }
        else if (noMPUData)
        {
            MessageText.GetComponent<TextMeshProUGUI>().SetText("No data from MPU sensor!");
            InstructionsText.GetComponent<TextMeshProUGUI>().SetText("Ensure the MPU is powered on and sending data. Reset ESP");
        }
    }
}