#include <pigpio.h>
#include <iostream>
#include <map>
#include <vector>
#include <chrono>
#include <thread>
#include <ctime>
#include <iomanip>
#include <fstream>
#include <algorithm> 
#include <iterator>   
#include </usr/include/jsoncpp/json/json.h>
#include <curl/curl.h>

// Boolean to test if the first pulse is detected
bool first_pulse_detected = false;

// File path to the log file
const std::string log_file_path = "/home/tec/Documents/C++/PulseLog.json";

// Read existing data or create a new file
std::vector<std::map<std::string, std::string>> pulse_log;

void postToAPI() {
    CURL* curl;
    CURLcode res;

    // Initialize libcurl
    curl = curl_easy_init();
    if (curl) {
        // Set the API endpoint
        curl_easy_setopt(curl, CURLOPT_URL, "http://10.233.134.112:2050/api/EnergyData");

        // Set the headers
        struct curl_slist* headers = NULL;
        headers = curl_slist_append(headers, "accept: text/plain");
        headers = curl_slist_append(headers, "Content-Type: application/json");
        curl_easy_setopt(curl, CURLOPT_HTTPHEADER, headers);

        // Set the JSON data
        Json::Value json_log;
        for (const auto& entry : pulse_log) {
            Json::Value json_entry;
            for (const auto& pair : entry) {
                json_entry[pair.first] = pair.second;
            }
            json_log.append(json_entry);
        }

        std::string post_data = Json::FastWriter().write(json_log);
        curl_easy_setopt(curl, CURLOPT_POSTFIELDS, post_data.c_str());

        // Perform the POST request
        res = curl_easy_perform(curl);
        if (res != CURLE_OK) {
            fprintf(stderr, "curl_easy_perform() failed: %s\n", curl_easy_strerror(res));
        }

        // Clean up
        curl_easy_cleanup(curl);
        curl_slist_free_all(headers);

        // Clear the pulse_log after successful POST
        pulse_log.clear();
    }
}

// GPIO pin configuration
struct Meter {
    int pin;
    int count;
    std::string bimayler_id;
};

std::map<std::string, Meter> meters = {
    {"Blue", {17, 0, "184BDB2B-8355-4D7D-0E62-08DBE5B8372C"}},
    {"White", {23, 0, "BB7750A1-60E8-409B-3134-08DBE5CA4CCD"}},
    {"Red", {22, 0, "270C991B-9FA9-47FA-3135-08DBE5CA4CCD"}},
    {"Green", {27, 0, "5C03AAC4-779B-4AEB-3136-08DBE5CA4CCD"}}
};

// Kilowatt-hours per pulse
const double kWh_per_pulse = 1.0 / 1000;

// Initialize last_post_time
std::chrono::system_clock::time_point last_post_time = std::chrono::system_clock::now();


// Callback function for GPIO interrupt
void pulse_callback(int gpio, int level, uint32_t tick) {
    if (!first_pulse_detected) {
        first_pulse_detected = true;
        // webbrowser.open('http://localhost:8000');
    }

    auto meter_it = std::find_if(meters.begin(), meters.end(), [gpio](const auto& meter_pair) {
        return meter_pair.second.pin == gpio;
    });

    if (meter_it != meters.end()) {
        meter_it->second.count++;
        auto current_time = std::chrono::system_clock::now();
        auto current_time_str = std::chrono::system_clock::to_time_t(current_time);
        auto tm_time = std::localtime(&current_time_str);
        auto milliseconds = std::chrono::duration_cast<std::chrono::milliseconds>(current_time.time_since_epoch()).count();

        std::ostringstream oss;
        oss << std::put_time(tm_time, "%Y-%m-%dT%H:%M:%S") << '.' << std::setw(3) << std::setfill('0') << milliseconds % 1000 << "Z";
        std::string current_time_formatted = oss.str();

        pulse_log.push_back({
            {"EnergyMeterID", meter_it->second.bimayler_id},
            {"DateTime", current_time_formatted}
        });

        std::cout << current_time_formatted << " - Socket " << meter_it->first
                  << " Pulse: " << meter_it->second.count << std::endl;
    }

    // Check if it's time to send a POST request
    auto now = std::chrono::system_clock::now();
    if (now >= last_post_time + std::chrono::seconds(10)) {
        postToAPI();
        last_post_time = now;  // Reset the timer
    }
}

// Function to set up GPIO pins
void setPins() {
    if (gpioInitialise() < 0) {
        std::cerr << "pigpio initialization failed." << std::endl;
        return;
    }

    // Set up each meter pin
    for (const auto& meter : meters) {
        gpioSetMode(meter.second.pin, PI_INPUT);
        gpioSetPullUpDown(meter.second.pin, PI_PUD_UP);
        gpioSetAlertFunc(meter.second.pin, pulse_callback);
    }
}

int main() {
    setPins();  // Don't forget to call this to set up your pins

    while (true) {
        std::this_thread::sleep_for(std::chrono::seconds(1));  // Prevent the loop from consuming too much CPU

        // Save the log to file periodically or on certain conditions
        std::ofstream file(log_file_path);
        if (file.is_open()) {
            Json::Value json_log;
            for (const auto& entry : pulse_log) {
                Json::Value json_entry;
                for (const auto& pair : entry) {
                    json_entry[pair.first] = pair.second;
                }
                json_log.append(json_entry);
            }
            file << json_log;
        }
    }

    gpioTerminate();  // Clean up GPIO
    return 0;
}
