using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using NativeWebSocket;
using Config;

public class EspWebsocket : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    WebSocket websocket;

    public float ax, ay, az;
    public float gx, gy, gz;
    public bool buttonPressed;

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket($"ws://{LocalConfig.ESP32_IP}/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string msg = System.Text.Encoding.UTF8.GetString(bytes);
            ParseData(msg);
        };

        // waiting for messages
        await websocket.Connect();
    }


    void ParseData(string msg)
    {
        string[] raw = msg.Split(',');

        if (raw.Length != 7) return;

        ax = float.Parse(raw[0], CultureInfo.InvariantCulture);
        ay = float.Parse(raw[1], CultureInfo.InvariantCulture);
        az = float.Parse(raw[2], CultureInfo.InvariantCulture);

        gx = float.Parse(raw[3], CultureInfo.InvariantCulture);
        gy = float.Parse(raw[4], CultureInfo.InvariantCulture);
        gz = float.Parse(raw[5], CultureInfo.InvariantCulture);

        buttonPressed = raw[6] == "1";
    }


    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

}
