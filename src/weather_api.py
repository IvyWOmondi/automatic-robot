import requests
from config import API_KEY, BASE_URL, FORECAST_URL
import os

class WeatherAPI:
    def __init__(self, api_key=API_KEY, base_url=BASE_URL, forecast_url=FORECAST_URL):
        self.api_key = api_key
        self.base_url = base_url
        self.forecast_url = forecast_url

    def get_weather(self, city):
        params = {
            'q': city,
            'appid': self.api_key,
            'units': 'imperial'  # Change to 'imperial' for Fahrenheit
        }
        try:
            response = requests.get(self.base_url, params=params)
            response.raise_for_status()  # Raise an HTTPError for bad responses
            data = response.json()
            print(data)  # Print the response for debugging

            # Write the response to an external file
            self.write_to_file(city, data)

            return data
        except requests.exceptions.HTTPError as http_err:
            print(f"HTTP error occurred: {http_err}")  # Print HTTP error
        except Exception as err:
            print(f"Other error occurred: {err}")  # Print other errors
        return None

    def get_forecast(self, city):
        params = {
            'q': city,
            'appid': self.api_key,
            'units': 'imperial'  # Change to 'imperial' for Fahrenheit
        }
        try:
            response = requests.get(self.forecast_url, params=params)
            response.raise_for_status()  # Raise an HTTPError for bad responses
            data = response.json()
            print(data)  # Print the response for debugging

            # Write the response to an external file
            self.write_forecast_to_file(city, data)

            return data
        except requests.exceptions.HTTPError as http_err:
            print(f"HTTP error occurred: {http_err}")  # Print HTTP error
        except Exception as err:
            print(f"Other error occurred: {err}")  # Print other errors
        return None

    def write_to_file(self, city, data):
        file_path = os.path.join("data", f"{city}_weather_data.txt")
        with open(file_path, "w") as file:
            file.write(str(data))

    def write_forecast_to_file(self, city, data):
        file_path = os.path.join("data", f"{city}_forecast_data.txt")
        with open(file_path, "w") as file:
            file.write(str(data))