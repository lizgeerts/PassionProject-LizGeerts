/*********
  gyroscope code from: https://RandomNerdTutorials.com/esp32-mpu-6050-web-server/
*********/

#include <Adafruit_MPU6050.h>
#include <Adafruit_Sensor.h>
#include <Wire.h>
#include <WiFi.h>

// const char* ssid = "Studeka-D";
// const char* password = "Throttle0-Thing3-Dollop0-Shakily5";

const char* ssid = "Howest-IoT";
const char* password = "LZe5buMyZUcDpLY";

NetworkServer server(80);

Adafruit_MPU6050 mpu; //create sensor

const int buttonPin = 4;  // the number of the pushbutton pin
int buttonState = 0;

//all the other serial prints are for debugging so they are now put in comments so they don't keep causing errors in the unity console.

//Initialize WiFi
void initWiFi() {
  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);
  //Serial.print("Connecting to WiFi ..");
  while (WiFi.status() != WL_CONNECTED) {
  //Serial.print('.');
    delay(1000);
  }
  Serial.println(WiFi.localIP());

  server.begin();
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

  sensors_event_t a, g, temp;
  mpu.getEvent(&a, &g, &temp);

  int button = digitalRead(buttonPin) == LOW ? 1 : 0;

  NetworkClient client = server.accept();
    
  if (client) {                     // if you get a client,
      Serial.println("New Client."); 
      String currentLine = "";      
      while (client.connected()) {  
        if (client.available()) {
          char c = client.read(); 
          Serial.write(c); 
          if (c == '\n') {    
            if (currentLine.length() == 0) {

            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/html");
            client.println();

            // all mpu data
            client.print(a.acceleration.x, 3); client.print(",");
            client.print(a.acceleration.y, 3); client.print(",");
            client.print(a.acceleration.z, 3); client.print(",");
            client.print(g.gyro.x, 3); client.print(",");
            client.print(g.gyro.y, 3); client.print(",");
            client.print(g.gyro.z, 3); client.print(",");
            client.println(button);

            client.println();
            
            break;
            } else {  // if you got a newline, then clear currentLine:
            currentLine = "";
            } 
          } else if (c != '\r') {  // if you got anything else but a carriage return character,
          currentLine += c;      // add it to the end of the currentLine
          }
        }
      }

    client.stop();
    Serial.println("Client Disconnected.");
  }
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

