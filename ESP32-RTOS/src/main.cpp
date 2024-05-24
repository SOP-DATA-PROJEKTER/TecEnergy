#include <Arduino.h>
#include <SD_MMC.h>
#include <HTTPClient.h>
#include <ETH.h>
#include <ArduinoJson.h>
#include <esp_vfs_fat.h>
#include <esp_sntp.h>

// Define Constants for SNTP
const char *sntpServer = "pool.ntp.org";

// Define Structs
typedef struct dataStruct
{
  const char *meterId;
  int accumulatedValue;
  String datetime;
} dataStruct;

dataStruct meters[] = {
    {"00000000-1C59-4EE5-87FA-F8246F1DBEF1", 0, ""}, // Btn Test
    {"00000001-D3E1-4302-A9A9-526BC223FD6E", 0, ""}, // meter 01
    {"00000002-2C46-4486-B8D9-7931CC6269C2", 0, ""}, // meter 02
    {"00000003-5A23-4786-B535-A733059FA959", 0, ""}, // meter 03
    {"00000004-1C59-4EE5-87FA-F8246F1DBEF1", 0, ""}  // meter 04
};

// Define Variables
xQueueHandle dataQueue;
SemaphoreHandle_t sdCardMutex;
volatile unsigned long lastDebounceTime[5] = {0, 0, 0, 0, 0};
volatile bool pulseDetected[5] = {false, false, false, false, false};
const int debounceTime = 400;

// Define Constants
 const char *apiUrl = "http://10.233.134.113:2050/api/EnergyData"; // energymeter room laptop server
//const char *apiUrl = "http://192.168.5.131:5252/api/EnergyData"; // Local PC

const char *filename = "/EnergyData.csv";

// Define the pins
const int builtInBtn = 34;  // built-in button to simulate impulse
const int impulsePin1 = 16; // Impulse pin
const int impulsePin2 = 32; // Impulse pin
const int impulsePin3 = 33; // Impulse pin
const int impulsePin4 = 36; // Impulse pin

// Function prototypes
void IRAM_ATTR impulseDetected(int meter);
void IRAM_ATTR buttonTest();

void queueDataHandling(void *pvParameters);
void sendToApi(void *pvParameters);
void updateDatetime(int index);

bool setupSdCard();
bool setupMutex();
bool setupInterrupts();

// Setup starts here
void setup()
{
  Serial.begin(115200);
  pinMode(impulsePin1, INPUT_PULLDOWN);
  pinMode(impulsePin2, INPUT_PULLDOWN);
  pinMode(impulsePin3, INPUT_PULLDOWN);
  pinMode(impulsePin4, INPUT_PULLDOWN);
  pinMode(builtInBtn, INPUT_PULLDOWN);
  ETH.begin();
  setupSdCard();
  setupMutex();
  dataQueue = xQueueCreate(20, sizeof(int *));
  setupInterrupts();
  xTaskCreate(queueDataHandling, "Queue Data Handling", 4096, NULL, 2, NULL);
  xTaskCreate(sendToApi, "Api Call", 4096, NULL, 1, NULL);

  Serial.println("Setup done");

  // Initialize the SNTP service
  sntp_setoperatingmode(SNTP_OPMODE_POLL);
  sntp_setservername(0, sntpServer);
  sntp_init();

  // Wait for time to be set
  struct tm timeinfo;
  Serial.println("Trying to get time from NTP server.");
  while (sntp_get_sync_status() == SNTP_SYNC_STATUS_RESET)
  {
    Serial.print(".");
    delay(2000);
  }
  Serial.println("\nTime synchronized with NTP server.");

  // Set local timezone for Copenhagen, Denmark
  // With Daylight saving time
  setenv("TZ", "CET-1CEST,M3.5.0/2,M10.5.0/3", 1);
  tzset();
}

bool setupMutex()
{
  while (sdCardMutex == NULL)
  {
    sdCardMutex = xSemaphoreCreateMutex();
  }
  return true;
}

bool setupSdCard()
{
  SD_MMC.begin();
  SD_MMC.remove(filename);
  if (!SD_MMC.exists(filename))
  {
    File file = SD_MMC.open(filename, FILE_WRITE);
    if (file)
    {
      file.println("EnergyMeterID,AccumulatedValue,DateTime");
    }
    file.close();
  }
  return true;
}

bool setupInterrupts()
{
  attachInterrupt(builtInBtn, buttonTest, RISING);
  attachInterrupt(impulsePin1, []()
                  { impulseDetected(1); }, RISING);
  attachInterrupt(impulsePin2, []()
                  { impulseDetected(2); }, RISING);
  attachInterrupt(impulsePin3, []()
                  { impulseDetected(3); }, RISING);
  attachInterrupt(impulsePin4, []()
                  { impulseDetected(4); }, RISING);
  return true;
}
// Interrupt functions
void IRAM_ATTR impulseDetected(int meter)
{
  unsigned long currentMillis = millis();
  if ((currentMillis - lastDebounceTime[meter]) >= debounceTime)
  {
    lastDebounceTime[meter] = currentMillis;
    xQueueSendFromISR(dataQueue, &meter, NULL);
  }
}

void IRAM_ATTR buttonTest()
{
  impulseDetected(0);
}

void queueDataHandling(void *pvParameters)
{
  int meter;
  while (true)
  {
    if (xQueueReceive(dataQueue, &meter, portMAX_DELAY))
    {
      updateDatetime(meter);
      meters[meter].accumulatedValue++;
      pulseDetected[meter] = true;

      // Write to SD card
      if (xSemaphoreTake(sdCardMutex, portMAX_DELAY))
      {
        File file = SD_MMC.open(filename, FILE_APPEND);
        if (file)
        {
          file.printf("%s,%d,%s\n", meters[meter].meterId, meters[meter].accumulatedValue, meters[meter].datetime.c_str());
        }
        file.close();
        xSemaphoreGive(sdCardMutex);
      }
    }
  }
}

void updateDatetime(int index)
{
  struct timeval tv_now;
  gettimeofday(&tv_now, NULL);

  // Get milliseconds part
  unsigned long milliseconds = tv_now.tv_usec / 1000;

  // Format datetime string with milliseconds
  struct tm timeinfo;
  localtime_r(&tv_now.tv_sec, &timeinfo);

  char buffer[36];
  snprintf(buffer, sizeof(buffer), "%04d-%02d-%02d %02d:%02d:%02d.%03lu",
           timeinfo.tm_year + 1900,
           timeinfo.tm_mon + 1,
           timeinfo.tm_mday,
           timeinfo.tm_hour,
           timeinfo.tm_min,
           timeinfo.tm_sec,
           milliseconds);

  meters[index].datetime = String(buffer);
}

void sendToApi(void *pvParameters)
{
  vTaskDelay(10000 / portTICK_PERIOD_MS);
  while (1)
  {
    // Serial.println("sendToApi Started");

    // take mutex
    // mutex     // max delay
    if (xSemaphoreTake(sdCardMutex, portMAX_DELAY) != pdTRUE)
    {
      Serial.println("Mutex failed to be taken within max delay");
      xSemaphoreGive(sdCardMutex);
      return;
    }

    // read file from sd card
    File file = SD_MMC.open(filename, FILE_READ);

    if (!file)
    {
      xSemaphoreGive(sdCardMutex);
      return;
    }

    // Skip the first line (header)
    if (file.available())
    {
      file.readStringUntil('\n');
    }

    int httpResponseCode = 0;

    if (file.available())
    {

      String payload = "[";

      while (file.available())
      {
        String line = file.readStringUntil('\n');
        Serial.println(line);

        // Split the CSV line into values
        String name = line.substring(0, line.indexOf(','));
        String value = line.substring(line.indexOf(',') + 1, line.lastIndexOf(','));
        String datetime = line.substring(line.lastIndexOf(',') + 1);

        // Add data to the JSON document
        JsonDocument jsonDocument;
        jsonDocument["meterId"] = name;
        jsonDocument["accumulatedValue"] = value.toInt();
        jsonDocument["dateTime"] = datetime;

        String temp;
        serializeJson(jsonDocument, temp);
        payload += temp;
        if (file.available())
        {
          payload += ",";
        }
      }

      payload += "]";
      // Serial.println(payload);
      HTTPClient http;

      // send data to api
      http.begin(apiUrl);
      http.addHeader("Content-Type", "application/json");

      httpResponseCode = http.POST(payload);

      Serial.println(httpResponseCode);

      // close http.client
      http.end();
    }

    // close file
    file.close();
    if (httpResponseCode == 204)
    {
      // overwrite file
      file = SD_MMC.open(filename, FILE_WRITE);
      if (file)
      {
        file.println("meterId,accumulatedValue,dateTime");
      }
      file.close();
    }

    // give mutex
    xSemaphoreGive(sdCardMutex);

    vTaskDelay(10000 / portTICK_PERIOD_MS);
  }
}

void loop()
{
}
