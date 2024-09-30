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

// Define Structs
typedef struct {
  char* measurement;
  int accumulated_value;
  String submeterId;
  String roomId;
} InfluxDBData;

InfluxDBData data{
  "Energy_Measurement",
  0,
  "",
  ""
};

typedef struct dataStruct {
  String meterId;
  int accumulatedValue;
} dataStruct;

dataStruct meters[] = {
  {"EB8D4250-D3E1-4302-A9A9-526BC223FD6E", 0}, // meter 1
  {"6424A2EE-2C46-4486-B8D9-7931CC6269C2", 0}, // meter 2
  {"BFE517CB-5A23-4786-B535-A733059FA959", 0}, // meter 3
  {"3946301E-1C59-4EE5-87FA-F8246F1DBEF1", 0}  // meter 4
};

struct ConfigurationStruct{
  bool wifiMode;
  String ssid;
  String password;
  String ip;
  String subnet;
  String gateway;
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
void IRAM_ATTR buttonTest();

void handlePoint(void *pvParameters);

void writePoint(InfluxDBData data);
void sendPoint();

InfluxDBClient setupInfluxDbClient();
bool setupInterrupts();
void setupRtosTasks();
void setupAccessMode();
bool setupWithWifi();
void setupWithEthernet();
void setupConfig();
void initLittleFS();

// setup starts here
void setup() {

  Serial.begin(115200);

  pinMode(impulsePin1, INPUT_PULLDOWN); // sets pin to input
  pinMode(impulsePin2, INPUT);
  pinMode(impulsePin3, INPUT);
  pinMode(impulsePin4, INPUT);
  pinMode(builtInBtn, PULLDOWN);

  initLittleFS();

  // read from littlefs to get values in config.json
  setupConfig();

  // if wifiMode is true, use wifi if not use ETH
  if (config.wifiMode)
  {
    // if setup with wifi fails, start access point mode
    if (!setupWithWifi())
    {
      setupAccessMode();
    }
  } 
  else {
    setupWithEthernet();
  }
  
  // Set time via NTP
  timeSync(TZ_INFO, "pool.ntp.org", "time.nis.gov");

  influxClient = setupInfluxDbClient();

  dataQueue = xQueueCreate(200, sizeof(int*));

  setupInterrupts();

  setupRtosTasks();

  // attachInterrupt(builtInBtn, buttonTest, FALLING);

  
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
  IPAddress localIP;
  localIP.fromString(config.ip);
  IPAddress gateway;
  gateway.fromString(config.gateway);
  IPAddress subnet;
  subnet.fromString(config.subnet);

  // configure wifi

  WiFi.mode(WIFI_STA);
  if(!WiFi.config(localIP, gateway, subnet)){
    Serial.println("STA Failed to configure");
    return false;
  }
  
  if(WiFi.begin(config.ssid, config.password)){
    Serial.println("STA Failed to configure");
    return false;
  }

  while (WiFi.status() != WL_CONNECTED)
  {
    delay(1000);
    Serial.println("Connecting to WiFi..");
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
  WiFi.mode(WIFI_AP);
  WiFi.softAP("EnergyMeter", "passw0rd");
  IPAddress IP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(IP);

  // start webserver
  AsyncWebServer server(80);


  server.on("/", HTTP_GET, [](AsyncWebServerRequest *request){
    request->send(LittleFS, "/index.html", "text/html");
  });

  server.serveStatic("/", LittleFS, "/");

  server.on("/", HTTP_POST, [](AsyncWebServerRequest *request){
    int params = request->params();
    for(int i=0;i<params;i++){
      const AsyncWebParameter* p = request->getParam(i);
      if(p->isPost()){
        Serial.print("POST: ");
        if(p->name() == "wifiMode"){
          config.wifiMode = p->value().toInt();
        }
        if (p->name() == "ssid")
        {
          config.ssid, p->value().c_str();
        }
        if (p->name() == "password")
        {
          config.password, p->value().c_str();
        }
        if (p->name() == "ip")
        {
          config.ip, p->value().c_str();
        }
        if (p->name() == "subnet")
        {
          config.subnet, p->value().c_str();
        }
        if (p->name() == "gateway")
        {
          config.gateway, p->value().c_str();
        }
        if (p->name() == "apiUrl")
        {
          config.apiUrl, p->value().c_str();
        }
        if (p->name() == "token")
        {
          config.token, p->value().c_str();
        }
        if (p->name() == "bucket")
        {
          config.bucket, p->value().c_str();
        }
        if (p->name() == "org")
        {
          config.org, p->value().c_str();
        }
        if (p->name() == "roomId")
        {
          config.roomId, p->value().c_str();
        }
        if (p->name() == "meterId1")
        {
          config.meterId[0], p->value().c_str();
        }
        if (p->name() == "meterId2")
        {
          config.meterId[1], p->value().c_str();
        }
        if (p->name() == "meterId3")
        {
          config.meterId[2], p->value().c_str();
        }
        if (p->name() == "meterId4")
        {
          config.meterId[3], p->value().c_str();
        }        
      }
      // convert the config to json
      JsonDocument confDoc;
      confDoc["wifiMode"] = config.wifiMode;
      confDoc["ssid"] = config.ssid;
      confDoc["password"] = config.password;
      confDoc["ip"] = config.ip;
      confDoc["subnet"] = config.subnet;
      confDoc["gateway"] = config.gateway;
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

      request->send(200, "text/plain", "Configuration saved, ESP will restart");
      delay(3000);
      ESP.restart();

    }
  });

  server.begin();

}


void setupConfig(){

  File configFile = LittleFS.open(configFilename, "r");

  // if file doesn't exist or is empty run setupAccessMode() to get values from user
  if (!configFile || configFile.size() == 0)
  {
    setupAccessMode();
    return;
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
  config.ip = doc["ip"].as<String>();
  config.subnet = doc["subnet"].as<String>();
  config.gateway = doc["gateway"].as<String>();
  config.apiUrl = doc["apiUrl"].as<String>();
  config.token = doc["token"].as<String>();
  config.bucket = doc["bucket"].as<String>();
  config.org = doc["org"].as<String>();
  config.roomId = doc["roomId"].as<String>();
  config.meterId[0] = doc["meterId1"].as<String>();
  config.meterId[1] = doc["meterId2"].as<String>();
  config.meterId[2] = doc["meterId3"].as<String>();
  config.meterId[3] = doc["meterId4"].as<String>();

  configFile.close();
  
}


InfluxDBClient setupInfluxDbClient(){

  InfluxDBClient client(config.apiUrl, config.org, config.bucket, config.token, InfluxDbCloud2CACert);
  
  // check for influxdb connection
  if (influxClient.validateConnection()) {
    Serial.print("Connected to InfluxDB: ");
    Serial.println(influxClient.getServerUrl());
  } else {
    Serial.print("InfluxDB connection failed: ");
    Serial.println(influxClient.getLastErrorMessage());
  }

  return client;
}


void setupRtosTasks(){

  xTaskCreate(
    handlePoint,
    "Handle Point",
    4096,
    NULL,
    1,
    NULL
  );

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


// for testing purposes
void IRAM_ATTR buttonTest(){
  int meter = 0;
  if(millis() - lastDebounceTime[meter] >= 80)
  {
    xQueueSendFromISR(dataQueue, &meter, 0);
    lastDebounceTime[meter] = millis();
  }
}


// RTOS Functions
void IRAM_ATTR handlePoint(void *pvParameters){
  vTaskDelay(10000 / portTICK_PERIOD_MS);
  while(1)
  {
    int meterIndex;
    if(xQueueReceive(dataQueue, &meterIndex, 0))
    {
      data.accumulated_value = ++meters[meterIndex].accumulatedValue;
      data.submeterId = meters[meterIndex].meterId;

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
  point.addField("accumulated_value", localData.accumulated_value);

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

