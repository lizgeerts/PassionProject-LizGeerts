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

#define LED_PIN 2

//all for joystick:
#define VRX_PIN 34 // pin D34 is analog input
#define VRY_PIN 35 

#define LEFT_THRESHOLD  1000
#define RIGHT_THRESHOLD 3000
#define UP_THRESHOLD    1000
#define DOWN_THRESHOLD  3000

int valueX = 0; // to store the X-axis value
int valueY = 0; // to store the Y-axis value

//wifi:
const char* ssid = "name";
const char* password = "password";

WiFiUDP udp;
const char* remoteIP = "ip"; // wifi ip adress from pc
const int remotePort = 5005;  

//mpu:
Adafruit_MPU6050 mpu; //create sensor

sensors_event_t a, g, temp;

float gyroX, gyroY, gyroZ;
float accX, accY, accZ;
int direction; //joystick

//Gyroscope sensor deviation
float gyroXerror = 0.07;
float gyroYerror = 0.03;
float gyroZerror = 0.01;

//all the other serial prints are for debugging so they are now put in comments so they don't keep causing errors in the unity console.

unsigned long lastSend = 0;
const int sendInterval = 30;

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
  digitalWrite(LED_PIN, HIGH);
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
  analogSetAttenuation(ADC_11db);

  pinMode(LED_PIN, OUTPUT);
  digitalWrite(LED_PIN, LOW);

  initWiFi();
  delay(1000);

  udp.begin(remotePort);

  InitMPU();
  scanI2C();
}

void loop() {
    // Reconnect WiFi if dropped
   if (WiFi.status() != WL_CONNECTED) {
      initWiFi();
      udp.stop();
      delay(500);
      udp.begin(remotePort);
   }

  joyStick();

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

    char csv[96];
    snprintf(csv, sizeof(csv), "%.3f,%.3f,%.3f,%.3f,%.3f,%.3f,%d",
                 accX, accY, accZ, gyroX, gyroY, gyroZ, direction);

    udp.beginPacket(remoteIP, remotePort);
    udp.write((uint8_t*)csv, strlen(csv));
    udp.endPacket();
  }
}

void joyStick(){
  // read X and Y analog values
  valueX = analogRead(VRX_PIN);
  valueY = analogRead(VRY_PIN);

  // reset 
  direction = 0;

  if (valueX < LEFT_THRESHOLD)
    direction = 1; //forward 
  else if (valueX > RIGHT_THRESHOLD)
    direction = 2; //backwards

  // check up/down commands
  if (valueY < UP_THRESHOLD)
   direction = 3; //right
  else if (valueY > DOWN_THRESHOLD)
    direction = 4; //left
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

