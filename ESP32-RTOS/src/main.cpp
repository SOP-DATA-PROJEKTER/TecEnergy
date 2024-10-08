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


#define TZ_INFO "CET-1CEST"


AsyncWebServer server(80);
// NAO-Wifi
// NaoPassword
// 192.168.1.27
// 192.168.1.1
// http://10.233.134.124:8086
// G1R_pTOe4u5_jA8Q9uQwcfeVTgakaXYn4NJSB1f0y8I4QEypG8y7lBQPH1uLXeXoeP8FFi7x-vqwxCixzBtccw==
// Energy_Collection
// TecEnergy
// E131
// Meter1
// Meter2

// Define Structs
typedef struct {
  char* measurement;
  int value;
  String submeterId;
  String roomId;
} InfluxDBData;

InfluxDBData data{
  "Energy_Measurement",
  0,
  "",
  ""
};


struct ConfigurationStruct{
  bool wifiMode;
  String ssid;
  String password;
  String apiUrl;
  String token;
  String bucket;
  String org;
  String roomId;
  String meterId[4];
};

ConfigurationStruct config;

// Define Variables
xQueueHandle dataQueue;
SemaphoreHandle_t sdCardMutex;
volatile unsigned long lastDebounceTime[4] = {0, 0, 0, 0};


const String apiUrl = "";
InfluxDBClient influxClient;
Point point("Energy_Measurement");

const String configFilename = "/config.json";

// Define the pins
const int impulsePin1 = 16; // Impulse pin
const int impulsePin2 = 32; // Impulse pin
const int impulsePin3 = 33; // Impulse pin
const int impulsePin4 = 36; // Impulse pin

const int builtInBtn = 34; // bultin button to simmulate impulse -- testing purposes

// define functions
void IRAM_ATTR impulseDetected1();
void IRAM_ATTR impulseDetected2();
void IRAM_ATTR impulseDetected3();
void IRAM_ATTR impulseDetected4();


void handlePoint(void *pvParameters);
void checkButtonTask(void *pvParameters);


void writePoint(InfluxDBData data);
void sendPoint();

InfluxDBClient setupInfluxDbClient();
bool setupInterrupts();
void setupRtosTasks();
void setupAccessMode();
bool setupWithWifi();
void setupWithEthernet();
bool setupConfig();
void initLittleFS();

// setup starts here
void setup() {

  Serial.begin(115200);

  pinMode(impulsePin1, INPUT_PULLDOWN); // sets pin to input
  pinMode(impulsePin2, INPUT);
  pinMode(impulsePin3, INPUT);
  pinMode(impulsePin4, INPUT);
  pinMode(builtInBtn, INPUT);

  initLittleFS();

  // read from littlefs to get values in config.json
  if(!setupConfig()){
    // go into access point mode 
    setupAccessMode();
    return;
  }

  // if wifiMode is true, use wifi if not use ETH
  if (config.wifiMode)
  {
    // if setup with wifi fails, start access point mode
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
  timeSync(TZ_INFO, "0.europe.pool.ntp.org", "1.europe.pool.ntp.org");

  influxClient = setupInfluxDbClient();

  dataQueue = xQueueCreate(20, sizeof(int*));

  setupInterrupts();

  setupRtosTasks();
  
  Serial.println("Setup done");
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
            config.ssid = p->value().c_str();
        } else if (p->name() == "password") {
            config.password = p->value().c_str();
        } else if (p->name() == "ApiUrl") {
            config.apiUrl = p->value().c_str();
        } else if (p->name() == "Token") {
            config.token = p->value().c_str();
        } else if (p->name() == "Bucket") {
            config.bucket = p->value().c_str();
        } else if (p->name() == "Organization") {
            config.org = p->value().c_str();
        } else if (p->name() == "RoomNumber") {
            config.roomId = p->value().c_str();
        } else if (p->name() == "Submeter1") {
            config.meterId[0] = p->value().c_str();
        } else if (p->name() == "Submeter2") {
            config.meterId[1] = p->value().c_str();
        } else if (p->name() == "Submeter3") {
            config.meterId[2] = p->value().c_str();
        } else if (p->name() == "Submeter4") {
            config.meterId[3] = p->value().c_str();
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
  config.ssid = doc["ssid"].as<String>();
  config.password = doc["password"].as<String>();
  config.apiUrl = doc["apiUrl"].as<String>();
  config.token =  doc["token"].as<String>();
  config.bucket = doc["bucket"].as<String>();
  config.org = doc["org"].as<String>();
  config.roomId = doc["roomId"].as<String>();
  config.meterId[0] = doc["meterId1"].as<String>();
  config.meterId[1] = doc["meterId2"].as<String>();
  config.meterId[2] = doc["meterId3"].as<String>();
  config.meterId[3] = doc["meterId4"].as<String>();

  // if api url is empty or "" run setupAccessMode() to get values from user
  if(config.apiUrl == ""){
    setupAccessMode();
    return false;
  }

  configFile.close();

  return true;
  
}


InfluxDBClient setupInfluxDbClient(){

  InfluxDBClient client(config.apiUrl, config.org, config.bucket, config.token, InfluxDbCloud2CACert);

  if (client.validateConnection()) {
    Serial.print("Connected to InfluxDB: ");
    Serial.println(client.getServerUrl());
  } else {
    Serial.print("InfluxDB connection failed: ");
    Serial.println(client.getLastErrorMessage());

  }

  return client;
}


void setupRtosTasks(){

  xTaskCreate(
    handlePoint,
    "Handle Point",
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
  attachInterrupt(impulsePin1, impulseDetected1, RISING); // sets interrupt when pin goes from low to high

  attachInterrupt(impulsePin2, impulseDetected2, RISING);

  attachInterrupt(impulsePin3, impulseDetected3, RISING); 
    
  attachInterrupt(impulsePin4, impulseDetected4, RISING);
  

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
  vTaskDelay(10000 / portTICK_PERIOD_MS);
  while(1)
  {
    int meterIndex;
    if(xQueueReceive(dataQueue, &meterIndex, 0))
    {
      data.value = 1;
      data.submeterId = config.meterId[meterIndex];
      data.roomId = config.roomId;

      writePoint(data);
      sendPoint();

    }

    
    vTaskDelay(20 / portTICK_PERIOD_MS);
  }
}


void writePoint(InfluxDBData localData){

  point.clearFields();
  point.clearTags();

  point.addTag("submeter", localData.submeterId);
  point.addField("value", localData.value);

}


void sendPoint(){

  if(influxClient.writePoint(point)){
    Serial.println("Write point success");
  } else {
    Serial.println("Write point failed");
    influxClient.getLastErrorMessage();
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

