using UnityEngine;
using System.IO.Ports;
using System;
using System.Collections;

//The code to connect with esp32 via cable (not wifi!)
//at the moment not in use
//to use this, attach to the player prefab 
public class EspConnect : MonoBehaviour
{

    SerialPort serial = new SerialPort("/dev/cu.usbserial-110", 115200);
    //check port each time you switch locations

    private float ax;
    private float ay;
    private float az;

    public float gx;
    public float gy;
    public float gz;
    private bool dataReady = false;
    private string lastLine = "";

    public bool buttonPressed;

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

            buttonPressed = raw[6] == "0";

        }
    }

    void OnApplicationQuit()
    {
        serial?.Close();
    }
}
