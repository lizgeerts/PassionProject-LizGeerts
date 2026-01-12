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

// Initialize WiFi
void initWiFi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
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
    Serial.println("MPU6050 not found, retrying...");
    delay(1000);
    retries--;
  }

  if (retries == 0) {
    Serial.println("MPU6050 broken? check wiring");
    while(1);
  }

  mpu.setAccelerometerRange(MPU6050_RANGE_8_G);
  mpu.setGyroRange(MPU6050_RANGE_500_DEG);

  Serial.println("MPU6050 READY!");
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

//   float pitch = atan2(a.acceleration.x, 
//                       sqrt(a.acceleration.y*a.acceleration.y + 
//                            a.acceleration.z*a.acceleration.z)) * 180 / PI;
//   float roll  = atan2(a.acceleration.y, 
//                       sqrt(a.acceleration.x*a.acceleration.x + 
//                            a.acceleration.z*a.acceleration.z)) * 180 / PI;

//   Serial.printf("Acc: %.1f %.1f %.1f | ", //format specifiers, control decimal 
//                 a.acceleration.x, a.acceleration.y, a.acceleration.z);
//                 //forward tilt, side tilt, less gravity
//                 //0, 0, 9.8 = normal
//                 //9, 0, 9.8 = forward tilt
//   Serial.printf("Temp: %.1f°C\n", temp.temperature);
//                 //room temperature
//   Serial.printf("gyro °: %.3f %.3f %.3f\n", 
//               g.gyro.x*57.3, g.gyro.y*57.3, g.gyro.z*57.3);
//               //How fast it's spinning right now
//               //rolling left, rolling down, 
//  Serial.printf("Pitch: %.1f° Roll: %.1f° | ", pitch, roll);

// Arduino: Send THESE 6 numbers (JSON/simple CSV)
  float pitch = atan2(a.acceleration.x, sqrt(a.acceleration.y*a.acceleration.y + a.acceleration.z*a.acceleration.z))*180/PI - 13.0;  // Zero flat!
  float roll  = atan2(a.acceleration.y, sqrt(a.acceleration.x*a.acceleration.x + a.acceleration.z*a.acceleration.z))*180/PI - 1.0;
  float acc_mag = sqrt(a.acceleration.x*a.acceleration.x + a.acceleration.y*a.acceleration.y + a.acceleration.z*a.acceleration.z);  // SWING SPEED!
  float gyro_mag = sqrt(g.gyro.x*g.gyro.x + g.gyro.y*g.gyro.y + g.gyro.z*g.gyro.z);  // ROTATION SPEED!

  Serial.printf("P:%.1f R:%.1f Acc:%.1f Gyr:%.3f\n", pitch, roll, acc_mag, gyro_mag);
  
  delay(200);
}

void scanI2C() { //I2C address detector -> automatically finds mpu
  Serial.println("Scanning I2C...");
  for (byte i = 8; i < 120; i++) {
    Wire.beginTransmission(i);
    if (Wire.endTransmission() == 0) {
      Serial.printf("Device at 0x%02X\n", i);
    }
  }
  Serial.println("Scan done");
}

