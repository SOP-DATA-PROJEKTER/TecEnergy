import RPi.GPIO as GPIO
import pytz
import time
from datetime import datetime, timedelta
import json
import webbrowser
import os
import requests

# Boolean til at teste om første puls er fundet
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

# Connecction til API
API_ENDPOINT = "http://10.233.134.112:2050/api/EnergyData"

# Initialize last_post_time
last_post_time = datetime.now()

# Funktion til at poste data til API
def postToAPI():
    global pulse_log
    #print(pulse_log)
    headers = {"accept": "text/plain","Content-Type": "application/json"}

    # Make the POST request
    try:
        response = requests.post(API_ENDPOINT, json=pulse_log, headers=headers)
        # Check the response status
        if response.status_code == 200:
            print("POST request successful.")
            # Clear the pulse_log after successful POST
            pulse_log = []
        else:
            print(f"POST request failed with status code {response.status_code}: {response.text}")

    except requests.exceptions.RequestException as e:
        print(f"Error making POST request: {e}")

# Definer dine måler-pins og deres ID'er her
meters = {
    'Blue': {'pin': 17,'count':0, 'Bimåler17': '184BDB2B-8355-4D7D-0E62-08DBE5B8372C'},
    'White': {'pin': 23,'count':0, 'Bimåler23': 'BB7750A1-60E8-409B-3134-08DBE5CA4CCD'},
    'Red': {'pin': 22,'count':0, 'Bimåler22': '270C991B-9FA9-47FA-3135-08DBE5CA4CCD'},
    'Green': {'pin': 27, 'count':0, 'Bimåler27': '5C03AAC4-779B-4AEB-3136-08DBE5CA4CCD'}
}

# Formel for at måle Kilowat per puls
kWh_per_pulse = 1.0 / 1000



# Interrupt Service Rutine
def pulse_callback(channel):
    global first_pulse_detected, last_post_time, pulse_log

    # # Hvis det er første puls åbner den er localhost side, som vil være vores configuration
    # if not first_pulse_detected:
    #     first_pulse_detected = True
    #     webbrowser.open('http://localhost:8000')

    # Finder målerens ID ved at matche den udløste GPIO pin med 'meters' dictionary
    meter_id = next((id for id, meter in meters.items() if meter['pin'] == channel), None)
    if meter_id:
        meters[meter_id]['count'] += 1 # Opdaterer pulstælleren for den pågældende måler

        # Registrerer det aktuelle tidspunkt i UTC og formaterer det
        current_time = datetime.now(pytz.utc).strftime('%Y-%m-%dT%H:%M:%S.%f')[:-3] + 'Z'

        # Tilføjer det aktuelle tidspunkt og målerens ID til loggen
        # pulse_log.append({'EnergyMeterID': meters[meter_id]['Bimåler1'], 'DateTime': current_time})
        pulse_log.append({'EnergyMeterID': meters[meter_id][f'Bimåler{meters[meter_id]["pin"]}'], 'DateTime': current_time})


        print(f"{current_time} - Stik {meter_id} Puls: {meters[meter_id]['count']}")

    #Check if it's time to send a POST request
    if datetime.now() >= last_post_time + timedelta(seconds=10):
        postToAPI()
        last_post_time = datetime.now()  # Reset the timer


def setPins():
    # Opsæt hver måler-pin
    for meter in meters.values():
        GPIO.setup(meter['pin'], GPIO.IN, pull_up_down=GPIO.PUD_UP)
        GPIO.add_event_detect(meter['pin'], GPIO.FALLING, callback=pulse_callback, bouncetime=80)

# Main loop
try:
    setPins()  # Don't forget to call this to set up your pins
    while True:
        time.sleep(1)  # This sleep is to prevent the loop from consuming too much CPU

        # Save the log to file periodically or on certain conditions
        with open(log_file_path, 'w') as file:
            json.dump(pulse_log, file, indent=4)

# Når en tast bliver trykket bliver programmet afbrudt og printer
except KeyboardInterrupt:
    with open(log_file_path, 'w') as file:
        json.dump(pulse_log, file, indent=4)
    print("Afslutter programmet...")

finally:
    GPIO.cleanup()