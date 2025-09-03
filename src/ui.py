import tkinter as tk
from tkinter import messagebox
from weather_api import WeatherAPI
import os

class WeatherApp:
    def __init__(self, root):
        self.root = root
        self.root.title("Weather App")
        self.api = WeatherAPI()

        self.city_label = tk.Label(root, text="Enter city name:")
        self.city_label.pack()

        self.city_entry = tk.Entry(root)
        self.city_entry.pack()

        self.get_weather_button = tk.Button(root, text="Get Weather", command=self.get_weather)
        self.get_weather_button.pack()

        self.get_forecast_button = tk.Button(root, text="Get Forecast", command=self.get_forecast)
        self.get_forecast_button.pack()

        self.save_weather_button = tk.Button(root, text="Save Weather to File", command=self.save_weather)
        self.save_weather_button.pack()

        self.weather_info = tk.Text(root, height=10, width=50)
        self.weather_info.pack()

    def get_weather(self):
        city = self.city_entry.get()
        if not city:
            messagebox.showerror("Error", "Please enter a city name")
            return

        weather_data = self.api.get_weather(city)
        if weather_data is None:
            messagebox.showerror("Error", "Failed to get weather data")
            return

        if weather_data.get("cod") != 200:
            messagebox.showerror("Error", weather_data.get("message", "Failed to get weather data"))
            return

        self.display_weather(weather_data)
        self.weather_data = weather_data  # Store the weather data for later use

    def get_forecast(self):
        city = self.city_entry.get()
        if not city:
            messagebox.showerror("Error", "Please enter a city name")
            return

        forecast_data = self.api.get_forecast(city)
        if forecast_data is None:
            messagebox.showerror("Error", "Failed to get forecast data")
            return

        if forecast_data.get("cod") != "200":
            messagebox.showerror("Error", forecast_data.get("message", "Failed to get forecast data"))
            return

        self.display_forecast(forecast_data)

    def display_weather(self, weather_data):
        self.weather_info.delete(1.0, tk.END)
        weather_desc = weather_data['weather'][0]['description']
        temp = weather_data['main']['temp']
        humidity = weather_data['main']['humidity']
        wind_speed = weather_data['wind']['speed']
        pressure = weather_data['main']['pressure']
        visibility = weather_data.get('visibility', 'N/A')  # Visibility might not always be available
        cloudiness = weather_data['clouds']['all']
        sunrise = weather_data['sys']['sunrise']
        sunset = weather_data['sys']['sunset']

        weather_info = (
            f"Weather: {weather_desc}\n"
            f"Temperature: {temp}°F\n"
            f"Humidity: {humidity}%\n"
            f"Wind Speed: {wind_speed} m/s\n"
            f"Pressure: {pressure} hPa\n"
            f"Visibility: {visibility} meters\n"
            f"Cloudiness: {cloudiness}%\n"
            f"Sunrise: {sunrise}\n"
            f"Sunset: {sunset}\n"
        )
        self.weather_info.insert(tk.END, weather_info)

    def display_forecast(self, forecast_data):
        self.weather_info.delete(1.0, tk.END)
        forecast_list = forecast_data['list']
        forecast_info = ""
        for forecast in forecast_list:
            dt_txt = forecast['dt_txt']
            weather_desc = forecast['weather'][0]['description']
            temp = forecast['main']['temp']
            humidity = forecast['main']['humidity']
            wind_speed = forecast['wind']['speed']
            forecast_info += (
                f"Date/Time: {dt_txt}\n"
                f"Weather: {weather_desc}\n"
                f"Temperature: {temp}°F\n"
                f"Humidity: {humidity}%\n"
                f"Wind Speed: {wind_speed} m/s\n"
                "-------------------------\n"
            )
        self.weather_info.insert(tk.END, forecast_info)

    def save_weather(self):
        if hasattr(self, 'weather_data'):
            city = self.city_entry.get()
            self.api.write_to_file(city, self.weather_data)
            messagebox.showinfo("Success", "Weather data saved to file")
        else:
            messagebox.showerror("Error", "No weather data to save")

    def save_weather_to_file(self, city, weather_data):
        weather_desc = weather_data['weather'][0]['description']
        temp = weather_data['main']['temp']
        humidity = weather_data['main']['humidity']
        wind_speed = weather_data['wind']['speed']
        pressure = weather_data['main']['pressure']
        visibility = weather_data.get('visibility', 'N/A')
        cloudiness = weather_data['clouds']['all']
        sunrise = weather_data['sys']['sunrise']
        sunset = weather_data['sys']['sunset']

        weather_info = (
            f"City: {city}\n"
            f"Weather: {weather_desc}\n"
            f"Temperature: {temp}°F\n"
            f"Humidity: {humidity}%\n"
            f"Wind Speed: {wind_speed} m/s\n"
            f"Pressure: {pressure} hPa\n"
            f"Visibility: {visibility} meters\n"
            f"Cloudiness: {cloudiness}%\n"
            f"Sunrise: {sunrise}\n"
            f"Sunset: {sunset}\n"
            "-------------------------\n"
        )

        with open(os.path.join("data", "weather_data.txt"), "a") as file:
            file.write(weather_info)