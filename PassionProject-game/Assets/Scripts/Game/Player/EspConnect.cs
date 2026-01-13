using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections;

public class EspConnect : MonoBehaviour
{

    SerialPort serial = new SerialPort("/dev/cu.usbserial-10", 115200);

    private float ax;
    private float ay;
    private float az;

    public float gx;
    public float gy;
    public float gz;

    public float pitch;
    public float roll;
    public int power;
    private bool dataReady = false;
    private string lastLine = "";

    IEnumerator ReadSerialCoroutine()
    {
        while (true)
        {
            if (serial.IsOpen)
            {
                try
                {
                    string line = serial.ReadLine().Trim();
                    if (line.Contains(","))
                    {
                        lastLine = line;
                        dataReady = true;
                    }
                }
                catch (TimeoutException) { }
            }
            yield return new WaitForSeconds(0.05f);
        }
    }

    void Start()
    {
        serial.ReadTimeout = 100;
        serial.DtrEnable = true;
        serial.RtsEnable = true;
        try
        {
            serial.Open();
            Debug.Log("Serial OK!");
            StartCoroutine(ReadSerialCoroutine());
        }
        catch
        {
            Debug.LogError("Serial fail!");
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (dataReady)
        {
            dataReady = false;
            if (!serial.IsOpen) return;

            string[] raw = lastLine.Split(',');
            if (raw.Length < 6) return;  // Skip incomplete lines

            ax = float.Parse(raw[0]);
            ay = float.Parse(raw[1]);
            az = float.Parse(raw[2]);

            gx = float.Parse(raw[3]);
            gy = float.Parse(raw[4]);
            gz = float.Parse(raw[5]);

            CalculateData();
        }
    }

    void CalculateData()
    {
        // Pitch/Roll
        pitch = Mathf.Atan2(ax, Mathf.Sqrt(ay * ay + az * az)) * Mathf.Rad2Deg - 12f;
        roll = Mathf.Atan2(ay, Mathf.Sqrt(ax * ax + az * az)) * Mathf.Rad2Deg - 1f;

        // Swing speed/power
        float acc_mag = Mathf.Sqrt(ax * ax + ay * ay + az * az) - 9.8f;
        acc_mag = Mathf.Max(0, acc_mag);
        power = Mathf.Min(10, (int)(acc_mag * 2));

        // Debug.Log($"Pitch:{pitch:F1} Roll:{roll:F1} Power:{power}");
    }

    void OnApplicationQuit()
    {
        serial?.Close();
    }
}
