using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class EspUdp : MonoBehaviour
{
    public int listenPort = 5005; // has to match remote port in ino file
    UdpClient udpClient;
    Thread receiveThread;
    bool running = false;

    string latestMessage;
    bool hasMessage = false;

    // data
    public float ax, ay, az;
    public float gx, gy, gz;
    public int joystickDir;


    void Start()
    {
        udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
        udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, listenPort));

        running = true;

        receiveThread = new Thread(ReceiveLoop);
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("test");
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
        Debug.Log($" ax:{ax} ay:{ay} az:{az} gx:{gx} gy:{gy} gz:{gz}, dir:{joystickDir}");
    }

    // void OnApplicationQuit()
    // {
    //     running = false;
    //     if (receiveThread != null && receiveThread.IsAlive)
    //         receiveThread.Join();
    //     if (udpClient != null) udpClient.Close();
    // }

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

        if (msg != null) ParseData(msg);
    }
}