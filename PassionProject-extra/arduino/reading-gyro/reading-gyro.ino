/*********
  gyroscope code from: https://RandomNerdTutorials.com/esp32-mpu-6050-web-server/
*********/

#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <AsyncTCP.h>
#include <ESPAsyncWebServer.h>
#include <Wire.h>
#include <WiFi.h>
#include <WiFiUdp.h>

const char* ssid = "phoneLiz";
const char* password = "03056366855804472133";

WiFiUDP udp;
const char* remoteIP = "10.124.62.36"; // wifi ip adress
const int remotePort = 5005;  

Adafruit_MPU6050 mpu; //create sensor

const int buttonPin = 4;  // the number of the pushbutton pin
int buttonState = 0;

sensors_event_t a, g, temp;

float gyroX, gyroY, gyroZ;
float accX, accY, accZ;

//Gyroscope sensor deviation
float gyroXerror = 0.07;
float gyroYerror = 0.03;
float gyroZerror = 0.01;

//all the other serial prints are for debugging so they are now put in comments so they don't keep causing errors in the unity console.

unsigned long lastSend = 0;
const int sendInterval = 20;

//Initialize WiFi
void initWiFi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  WiFi.setSleep(false);
  Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED) {
  Serial.print('.');
    delay(1000);
  }
  Serial.println(WiFi.localIP());
}

void InitMPU(){
  Wire.begin(21, 22);
  pinMode(21, INPUT_PULLUP);  // Internal pull-ups
  pinMode(22, INPUT_PULLUP);
  Wire.setClock(100000);  // Slow I2C: 100kHz

  int retries = 5;
  while (!mpu.begin() && retries > 0) {
    //Serial.println("MPU6050 not found, retrying...");
    delay(500);
    retries--;
  }

  if (retries == 0) {
    //Serial.println("MPU6050 broken? check wiring");
    while(1);
  }

  mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
  mpu.setGyroRange(MPU6050_RANGE_500_DEG);
  //Serial.println("MPU6050 READY!");
}


void setup() {
  Serial.begin(115200);
  while (!Serial) delay(10);

  initWiFi();
  InitMPU();
  scanI2C();

  pinMode(buttonPin, INPUT);
}

void loop() {
  unsigned long now = millis();
  if (now - lastSend >= sendInterval) {
    lastSend = now;

    mpu.getEvent(&a, &g, &temp);

    accX = a.acceleration.x;
    accY = a.acceleration.y;
    accZ = a.acceleration.z;

    if (abs(g.gyro.x) > gyroXerror) gyroX += g.gyro.x / 50.0;
    if (abs(g.gyro.y) > gyroYerror) gyroY += g.gyro.y / 70.0;
    if (abs(g.gyro.z) > gyroZerror) gyroZ += g.gyro.z / 90.0;

    int button = digitalRead(buttonPin) == LOW ? 0 : 1;

    char csv[96];
    snprintf(csv, sizeof(csv), "%.3f,%.3f,%.3f,%.3f,%.3f,%.3f,%d",
                 accX, accY, accZ, gyroX, gyroY, gyroZ, button);

    udp.beginPacket(remoteIP, remotePort);
    udp.write((uint8_t*)csv, strlen(csv));
    udp.endPacket();
  }
}

void scanI2C() { //I2C address detector -> automatically finds mpu
  //Serial.println("Scanning I2C...");
  for (byte i = 8; i < 120; i++) {
    Wire.beginTransmission(i);
    if (Wire.endTransmission() == 0) {
      //Serial.printf("Device at 0x%02X\n", i);
    }
  }
  //Serial.println("Scan done");
}

