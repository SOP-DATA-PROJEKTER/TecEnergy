import RPi.GPIO as GPIO
import pytz
import time
from datetime import datetime
import json
import webbrowser
import os
import requests





first_pulse_detected = False

# Filsti til logfil
log_file_path = '/home/tec/Documents/Software/PulseLog.json'

# Læs eksisterende data eller opret ny fil
if not os.path.isfile(log_file_path):
    pulse_log = []
else:
    with open(log_file_path, 'r') as file:
        pulse_log = json.load(file)

GPIO.setmode(GPIO.BCM)

API_ENDPOINT = "http://10.233.134.112:2050/api/EnergyData"

def postToAPI():
    global pulse_log
    print(pulse_log)
    headers = {"accept": "text/plain","Content-Type": "application/json"}

    # Make the POST request
    try:
        response = requests.post(API_ENDPOINT, json=pulse_log, headers=headers)
        # Check the response status
        if response.status_code == 200:
            print("POST request successful.")
        else:
            print(f"POST request failed with status code {response.status_code}: {response.text}")

    except requests.exceptions.RequestException as e:
        print(f"Error making POST request: {e}")

# Definer dine måler-pins og deres ID'er her
meters = {
    'Blue': {'pin': 17,'count':0, 'Bimåler2': '184BDB2B-8355-4D7D-0E62-08DBE5B8372C'},
    'White': {'pin': 10,'count':0, 'Bimåler3': 'BB7750A1-60E8-409B-3134-08DBE5CA4CCD'},
    'Red': {'pin': 23,'count':0, 'Bimåler4': '270C991B-9FA9-47FA-3135-08DBE5CA4CCD'},
    'Green': {'pin': 27, 'count':0, 'Bimåler1': '5C03AAC4-779B-4AEB-3136-08DBE5CA4CCD'}
}

kWh_per_pulse = 1.0 / 1000

# Interrupt Service Rutine
def pulse_callback(channel):
    global first_pulse_detected

    if not first_pulse_detected:
        first_pulse_detected = True
        webbrowser.open('http://localhost:8000')

    meter_id = next((id for id, meter in meters.items() if meter['pin'] == channel), None)
    if meter_id:
        meters[meter_id]['count'] += 1
        current_time = datetime.now(pytz.utc).strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'
        #Der testet
        pulse_log.append({'EnergyMeterID': meters[meter_id]['Bimåler1'], 'DateTime': current_time})
        print(f"{current_time} - Stik {meter_id} Puls: {meters[meter_id]['count']}")
    postToAPI()

# Opsæt hver måler-pin
for meter in meters.values():
    GPIO.setup(meter['pin'], GPIO.IN, pull_up_down=GPIO.PUD_UP)
    GPIO.add_event_detect(meter['pin'], GPIO.FALLING, callback=pulse_callback, bouncetime=80)

try:
    while True:
        time.sleep(1)

except KeyboardInterrupt:
    with open(log_file_path, 'w') as file:
        json.dump(pulse_log, file, indent=4)
    print("Afslutter programmet...")

finally:
    GPIO.cleanup()