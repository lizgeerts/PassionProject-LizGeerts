/*********
  gyroscope code from: https://RandomNerdTutorials.com/esp32-mpu-6050-web-server/
*********/

#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>
#include <WiFi.h>

const char* ssid = "Studeka-D";
const char* password = "Throttle0-Thing3-Dollop0-Shakily5";

Adafruit_MPU6050 mpu; //create sensor

//all the other serial prints are for debugging so they are now put in comments so they don't keep causing errors in the unity console.

// Initialize WiFi
void initWiFi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  //Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED) {
  //Serial.print('.');
    delay(1000);
  }
 // Serial.println(WiFi.localIP());
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
}

void loop() {
  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  // all mpu data
  Serial.print(a.acceleration.x, 3); Serial.print(",");
  Serial.print(a.acceleration.y, 3); Serial.print(",");
  Serial.print(a.acceleration.z, 3); Serial.print(",");
  Serial.print(g.gyro.x, 3); Serial.print(",");
  Serial.print(g.gyro.y, 3); Serial.print(",");
  Serial.print(g.gyro.z, 3);
  Serial.println();

  delay(10);
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

