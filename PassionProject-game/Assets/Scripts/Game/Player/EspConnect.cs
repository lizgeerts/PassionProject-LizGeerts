using UnityEngine;
using System.IO.Ports;
using System;

public class EspConnect : MonoBehaviour
{

    SerialPort serial = new SerialPort("/dev/cu.usbserial-10", 115200);

    private float ax;
    private float ay;
    private float az;

    private float gx;
    private float gy;
    private float gz;

    void Start()
    {
        serial.ReadTimeout = 100;
        serial.DtrEnable = true;  // ESP32 needs!
        serial.RtsEnable = true;
        try
        {
            serial.Open();
            Debug.Log("Serial OK!");
        }
        catch
        {
            Debug.LogError("Serial fail!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!serial.IsOpen) return;
        string line = "";

        try
        {
            line = serial.ReadLine().Trim();
            string[] raw = line.Split(',');

            ax = float.Parse(raw[0]);
            ay = float.Parse(raw[1]);
            az = float.Parse(raw[2]);

            gx = float.Parse(raw[3]);
            gy = float.Parse(raw[4]);
            gz = float.Parse(raw[5]);
        }
        catch (TimeoutException)
        {
            //  No time operation spam
        }
        catch (Exception e)
        {
            Debug.LogError("Real error: " + e.Message);
        }

        CalculateData();
    }

    void CalculateData()
    {
        // Pitch/Roll
        float pitch = Mathf.Atan2(ax, Mathf.Sqrt(ay * ay + az * az)) * Mathf.Rad2Deg - 12f;
        float roll = Mathf.Atan2(ay, Mathf.Sqrt(ax * ax + az * az)) * Mathf.Rad2Deg - 1f;

        // Swing speed/power
        float acc_mag = Mathf.Sqrt(ax * ax + ay * ay + az * az) - 9.8f;
        acc_mag = Mathf.Max(0, acc_mag);
        int power = Mathf.Min(10, (int)(acc_mag * 2));

        Debug.Log($"Pitch:{pitch:F1} Roll:{roll:F1} Power:{power}");
    }

    void OnApplicationQuit()
    {
        serial?.Close();
    }
}
