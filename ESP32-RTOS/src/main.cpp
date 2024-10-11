#include <Arduino.h>
#include <SD_MMC.h>
#include <HTTPClient.h>
#include <ETH.h> 

#include <WiFi.h>
#include <ESPAsyncWebServer.h>
#include <AsyncTCP.h>
#include "LittleFS.h"

#include <ArduinoJson.h> // need to install this library ArduinoJson by Benoit Blanchona
#include <InfluxDbClient.h>
#include <InfluxDbCloud.h>
#include <time.h>


// #define TZ_INFO "CET-1CEST"
#define TZ_INFO "UTC2"


AsyncWebServer server(80);
// NAO-Wifi
// NaoPassword
// 192.168.1.27
// 192.168.1.1
// http://10.233.134.109:8086
// G1R_pTOe4u5_jA8Q9uQwcfeVTgakaXYn4NJSB1f0y8I4QEypG8y7lBQPH1uLXeXoeP8FFi7x-vqwxCixzBtccw==
// Energy_Collection
// TecEnergy
// E131
// Meter1
// Meter2

// Define Structs
typedef struct {
  char* measurement;
  int impulse;
  String submeterId;
  String roomId;
} InfluxDBData;

// InfluxDBData data{
//   "Energy_Measurement",
//   0,
//   "",
//   ""
// };

struct ConfigurationStruct{
  bool wifiMode;
  char* ssid;
  char* password;
  char* apiUrl;
  char* token;
  char* bucket;
  char* org;
  char* roomId;
  char* meterId[4];
};

ConfigurationStruct config;

// Define Variables
xQueueHandle dataQueue;
SemaphoreHandle_t sdCardMutex;
volatile unsigned long lastDebounceTime[4] = {0, 0, 0, 0};

InfluxDBClient influxClient;
Point point("Energy_Measurement");


const String configFilename = "/config.json";

// Define the pins
const int impulsePin1 = 16; // Impulse pin
const int impulsePin2 = 32; // Impulse pin
const int impulsePin3 = 33; // Impulse pin
const int impulsePin4 = 36; // Impulse pin

const int builtInBtn = 34; // bultin button to reset

// define functions
void IRAM_ATTR impulseDetected1();
void IRAM_ATTR impulseDetected2();
void IRAM_ATTR impulseDetected3();
void IRAM_ATTR impulseDetected4();

void IRAM_ATTR buttonTest();
void checkButtonTask(void *pvParameters);
void handleButtonPress(void *pvParameters);

void handlePoint(void *pvParameters);

void sendPoint(int meterIndex);

void setupInfluxDbClient();
bool setupInterrupts();
void setupRtosTasks();
void setupAccessMode();
bool setupWithWifi();
void setupWithEthernet();
bool setupConfig();
void initLittleFS();

char* copyString(const String& str);
void freeConfig();

// setup starts here
void setup() {

  Serial.begin(115200);

  // pinMode(impulsePin1, INPUT_PULLDOWN); // sets pin to input
  // pinMode(impulsePin2, INPUT_PULLDOWN);
  // pinMode(impulsePin3, INPUT_PULLDOWN);
  // pinMode(impulsePin4, INPUT_PULLDOWN);
  pinMode(builtInBtn, PULLDOWN);

  initLittleFS();

  // for debugging
  // setupAccessMode();
  // return;

  // read from littlefs to get values in config.json
  if(!setupConfig()){
    // go into accesspoint mode 
    setupAccessMode();
    return;
  }

  // if wifiMode is true, use wifi if not use ETH
  if (config.wifiMode)
  {
    // if setup with wifi fails, start accesspoint mode
    if (!setupWithWifi())
    {
      Serial.println("Failed to connect to wifi");
      setupAccessMode();
      return;
    }
  } 
  else {
    setupWithEthernet();
  }
  
  // Set time via NTP
  // timeSync(TZ_INFO, "0.europe.pool.ntp.org", "1.europe.pool.ntp.org");
  timeSync(TZ_INFO, "pool.ntp.org", "time.nis.gov");

  delay(200);

  setupInfluxDbClient();

  dataQueue = xQueueCreate(20, sizeof(int*));

  setupInterrupts();

  setupRtosTasks();
  
  Serial.println("Setup done");

  Serial.println(ESP.getFreeHeap());

}


// setup functions
void initLittleFS() {
  if (!LittleFS.begin(true)) {
    Serial.println("An error has occurred while mounting LittleFS");
  }
  Serial.println("LittleFS mounted successfully");
}


bool setupWithWifi(){
  // run after setupConfig()
  // should be called if wifiMode is true
  // should have values from config.json inside config struct

  // start by getting values from config struct
  WiFi.mode(WIFI_STA);

  // configure wifi

  if(!WiFi.config(0u, 0u, 0u)){
    Serial.println("STA Failed to configure");
  }



  WiFi.begin(config.ssid, config.password);
  Serial.println("Connecting to WiFi...");

  unsigned long currentMillis = millis();
  unsigned long previousMillis = 0;
  previousMillis = currentMillis;


  while (WiFi.status() != WL_CONNECTED)
  {
    currentMillis = millis();
    if (currentMillis - previousMillis >= 10000)
    {
      Serial.println("Failed to connect to WiFi");
      return false;
    }
    delay(100);
    Serial.print(".");
  }

  Serial.println("Connected to WiFi");
  Serial.println(WiFi.localIP());

  return true;
}


void setupWithEthernet(){
  ETH.begin();
}


void setupAccessMode(){

  // assume no or empty config file / values
  // start in access mode
  // WiFi.mode(WIFI_AP);
  WiFi.softAP("EnergyMeter", NULL);
  IPAddress IP = WiFi.softAPIP();
  // print SSID and IP address
  Serial.print("AP SSID: ");
  Serial.println(WiFi.softAPSSID());
  Serial.print("AP IP address: ");
  Serial.println(IP);



  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send(LittleFS, "/index.html", "text/html");
  });

  server.serveStatic("/", LittleFS, "/");

  server.on("/", HTTP_POST, [](AsyncWebServerRequest *request){
    int params = request->params();
    Serial.println(params);
    for(int i=0;i<params;i++){
      const AsyncWebParameter* p = request->getParam(i);
      if (p->isPost()) {
        Serial.print("POST: ");
        Serial.println(p->name().c_str());
        Serial.println(p->value().c_str());

        if (p->name() == "wifiMode") {
            config.wifiMode = p->value().equalsIgnoreCase("true"); // Convert to boolean
        } else if (p->name() == "ssid") {
            free(config.ssid);
            config.ssid = copyString(p->value());
        } else if (p->name() == "password") {
            free(config.password);
            config.password = copyString(p->value());
        } else if (p->name() == "ApiUrl") {
            free(config.apiUrl);
            config.apiUrl = copyString(p->value());
        } else if (p->name() == "Token") {
            free(config.token);
            config.token = copyString(p->value());
        } else if (p->name() == "Bucket") {
            free(config.bucket);
            config.bucket = copyString(p->value());
        } else if (p->name() == "Organization") {
            free(config.org);
            config.org = copyString(p->value());
        } else if (p->name() == "RoomNumber") {
            free(config.roomId);
            config.roomId = copyString(p->value());
        } else if (p->name() == "Submeter1") {
            free(config.meterId[0]);
            config.meterId[0] = copyString(p->value());
        } else if (p->name() == "Submeter2") {
            free(config.meterId[1]);
            config.meterId[1] = copyString(p->value());
        } else if (p->name() == "Submeter3") {
            free(config.meterId[2]);
            config.meterId[2] = copyString(p->value());
        } else if (p->name() == "Submeter4") {
            free(config.meterId[3]);
            config.meterId[3] = copyString(p->value());
        }     
    
      }

    }

    request->send(200, "text/plain", "Configuration saved, ESP will restart");

    delay(3000);

    // convert the config to json
    JsonDocument confDoc;
    confDoc["wifiMode"] = config.wifiMode;
    confDoc["ssid"] = config.ssid;
    confDoc["password"] = config.password;
    confDoc["apiUrl"] = config.apiUrl;
    confDoc["token"] = config.token;
    confDoc["bucket"] = config.bucket;
    confDoc["org"] = config.org;
    confDoc["roomId"] = config.roomId;
    confDoc["meterId1"] = config.meterId[0];
    confDoc["meterId2"] = config.meterId[1];
    confDoc["meterId3"] = config.meterId[2];
    confDoc["meterId4"] = config.meterId[3];

    // write to config.json
    File configFile = LittleFS.open(configFilename, "w");
    if(!configFile){
      Serial.println("Failed to open config file for writing");
    }

    serializeJson(confDoc, configFile);
    configFile.close();
    Serial.println("Restarting ESP");
    delay(3000);
    ESP.restart();

  });

  server.begin();

}


bool setupConfig(){

  File configFile = LittleFS.open(configFilename, "r");

  // if file doesn't exist or is empty run setupAccessMode() to get values from user
  if (!configFile || configFile.size() == 0)
  {
    setupAccessMode();
    return false;
  }

  if(!configFile){
    Serial.println("Failed to open config file");
  }

  JsonDocument doc;
  DeserializationError error = deserializeJson(doc, configFile);

  if(error){
    Serial.println("Failed to read file, using default configuration");
  }


  config.wifiMode = doc["wifiMode"].as<bool>();
  config.ssid = copyString(doc["ssid"].as<String>());
  config.password = copyString(doc["password"].as<String>());
  config.apiUrl = copyString("http://192.168.1.26:8086");  // copyString(doc["apiUrl"].as<char*>());
  config.token = "qzwvdxVyxy0bKfvvYD0o8_sUPyis8hNFI4CDtRLdB4VcMnhbbDmLxp-ZYaM_1xPChBPUHyooBq3WOXzuxcxxvQ==";
  config.bucket = "Energy_Collection";
  config.org = "0abeb7a850e0a023";
  // config.apiUrl = copyString(doc["apiUrl"].as<char*>());
  // config.token = copyString(doc["token"].as<String>());
  // config.bucket = copyString(doc["bucket"].as<String>());
  // config.org = copyString(doc["org"].as<String>());
  config.roomId = copyString(doc["roomId"].as<String>());
  config.meterId[0] = copyString(doc["meterId1"].as<String>());
  config.meterId[1] = copyString(doc["meterId2"].as<String>());
  config.meterId[2] = copyString(doc["meterId3"].as<String>());
  config.meterId[3] = copyString(doc["meterId4"].as<String>());

  if (!config.ssid || !config.password || !config.apiUrl || !config.token || !config.bucket || !config.org || !config.roomId) {
      Serial.println("Memory allocation error!");
  }


  // print the doc
  // prettyfi
  serializeJsonPretty(doc, Serial);

  // data.roomId = config.roomId;


  // if api url is empty or "" run setupAccessMode() to get values from user
  if(config.apiUrl == ""){
    setupAccessMode();
    return false;
  }

  configFile.close();

  return true;
  
}


void setupInfluxDbClient(){

  influxClient = InfluxDBClient(config.apiUrl, config.org, config.bucket, config.token, InfluxDbCloud2CACert);

  if (influxClient.validateConnection()) {
    Serial.print("Connected to InfluxDB: ");
    Serial.println(influxClient.getServerUrl());
  } else {
    Serial.print("InfluxDB connection failed: ");
    Serial.println(influxClient.getLastErrorMessage());

    delay(3000);
    ESP.restart();

  }

  // Serial.print("Influx client address in setup: ");
  // Serial.println((uint32_t)influxClient);

}


void setupRtosTasks(){

  xTaskCreate(
    handlePoint,
    "Handle Point",
    8192,
    NULL,
    2,
    NULL
  );

  xTaskCreate(
    handleButtonPress,
    "Handle Button",
    8192,
    NULL,
    1,
    NULL
  );

  // xTaskCreate(
  //   checkButtonTask,
  //   "Check Button",
  //   4096,
  //   NULL,
  //   2,
  //   NULL
  // );

}


bool setupInterrupts(){
  // attachInterrupt(impulsePin1, impulseDetected1, RISING); // sets interrupt when pin goes from low to high

  // attachInterrupt(impulsePin2, impulseDetected2, RISING);

  // attachInterrupt(impulsePin3, impulseDetected3, RISING); 
    
  // attachInterrupt(impulsePin4, impulseDetected4, RISING);

  attachInterrupt(builtInBtn, buttonTest, FALLING);
  

  return true;
}


// Interrupt functions

// interrupt function for when an impulse is detected
// debounce time to avoid noise
// sends data to queue on interrupt
void IRAM_ATTR impulseDetected1() {
  // index of meter
  int meter = 0;
  if(millis() - lastDebounceTime[meter] >= 80)
  {
    xQueueSendFromISR(dataQueue, &meter, 0);
    lastDebounceTime[meter] = millis();
  }
}


void IRAM_ATTR impulseDetected2() {
  int meter = 1;
  if(millis() - lastDebounceTime[meter] >= 80)
  {
    xQueueSendFromISR(dataQueue, &meter, 0);
    lastDebounceTime[meter] = millis();
  }
}


void IRAM_ATTR impulseDetected3() {
  int meter = 2;
  if(millis() - lastDebounceTime[meter] >= 80)
  {
    xQueueSendFromISR(dataQueue, &meter, 0);
    lastDebounceTime[meter] = millis();
  }
}


void IRAM_ATTR impulseDetected4() {
  int meter = 3;
  if(millis() - lastDebounceTime[meter] >= 80)
  {
    xQueueSendFromISR(dataQueue, &meter, 0);
    lastDebounceTime[meter] = millis();
  }

}


// RTOS Functions
void handlePoint(void *pvParameters){
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  while(1)
  {
    int meterIndex;
    if(xQueueReceive(dataQueue, &meterIndex, 0))
    {

      sendPoint(meterIndex);

    }
    
    vTaskDelay(200 / portTICK_PERIOD_MS);
  }
}


void sendPoint(int meterIndex){

  point.clearFields();
  point.clearTags();

  point.addTag("submeter", config.meterId[meterIndex]);
  point.addTag("room", config.roomId);
  point.addField("impulse", 1);


  if (!influxClient.validateConnection()) {
    Serial.println("InfluxDB connection lost!");
    Serial.println(influxClient.getLastErrorMessage());
    return;  // Return early if the connection is lost
  }
  else{
    Serial.println("InfluxDB connection still valid");
  }

  if(influxClient.writePoint(point)){
    Serial.println("Write point success");
  } 
  else{
    Serial.println("Write point failed");
    Serial.println(influxClient.getLastErrorMessage());
  }

  Serial.println(influxClient.pointToLineProtocol(point));

}



void loop(){

}


void checkButtonTask(void * paramter) {
  vTaskDelay(200 / portTICK_PERIOD_MS);
  while (true)
  {

    if (digitalRead(builtInBtn) == LOW) {
      // suspend other tasks
      vTaskSuspendAll();

      // stop interrupts
      detachInterrupt(impulsePin1);
      detachInterrupt(impulsePin2);
      detachInterrupt(impulsePin3);
      detachInterrupt(impulsePin4);

      Serial.println("Button pressed, resetting config...");
      
      // Delete the config file
      if (LittleFS.exists(configFilename)) {
        LittleFS.remove(configFilename);
        Serial.println("Config file deleted");
      } else {
        Serial.println("Config file does not exist");
      }
      
      // Wait a bit before restarting
      delay(1000);
      
      // Restart the ESP32
      ESP.restart();

    vTaskDelay(1000 / portTICK_PERIOD_MS);
    }
  }
}


volatile bool buttonPressed = false;


void IRAM_ATTR buttonTest(){
    buttonPressed = true;  // Set a flag instead of handling millis() here
}


void handleButtonPress(void *pvParameters){
  vTaskDelay(1000 / portTICK_PERIOD_MS);
  int meter = 0;
  int *meterPointer = &meter;
  while (true)
  {
    if(buttonPressed) {
        buttonPressed = false;
 
        if(millis() - lastDebounceTime[meter] >= 80) {
            xQueueSend(dataQueue, meterPointer, 0); 
            lastDebounceTime[meter] = millis();
        }
      }
    vTaskDelay(500 / portTICK_PERIOD_MS);
  }
  
}


char* copyString(const String& str) {
    char* buffer = (char*)malloc(str.length() + 1);  // Allocate memory
    if (buffer) {
        strcpy(buffer, str.c_str());  // Copy String to char*
    }
    return buffer;  // Return pointer to the allocated memory
}


void freeConfig() {
    if (config.ssid) free(config.ssid);
    if (config.password) free(config.password);
    if (config.apiUrl) free(config.apiUrl);
    if (config.token) free(config.token);
    if (config.bucket) free(config.bucket);
    if (config.org) free(config.org);
    if (config.roomId) free(config.roomId);

    for (int i = 0; i < 4; i++) {
        if (config.meterId[i]) free(config.meterId[i]);
    }
}

