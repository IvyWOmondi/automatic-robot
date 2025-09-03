using System;
using System.IO;
using System.Windows.Forms;

namespace WeatherApi
{
    public class WeatherApp : Form
    {
        private Label cityLabel;
        private TextBox cityEntry;
        private Button getWeatherButton;
        private Button getForecastButton;
        private Button saveWeatherButton;
        private TextBox weatherInfo;
        private WeatherAPI api;
        private dynamic weatherData;

        public WeatherApp()
        {
            this.Text = "Weather App";
            api = new WeatherAPI();

            cityLabel = new Label { Text = "Enter city name:", Top = 10, Left = 10, Width = 120 };
            cityEntry = new TextBox { Top = 35, Left = 10, Width = 200 };
            getWeatherButton = new Button { Text = "Get Weather", Top = 65, Left = 10, Width = 100 };
            getForecastButton = new Button { Text = "Get Forecast", Top = 65, Left = 120, Width = 100 };
            saveWeatherButton = new Button { Text = "Save Weather to File", Top = 95, Left = 10, Width = 210 };
            weatherInfo = new TextBox { Top = 130, Left = 10, Width = 350, Height = 200, Multiline = true, ScrollBars = ScrollBars.Vertical };

            getWeatherButton.Click += GetWeather;
            getForecastButton.Click += GetForecast;
            saveWeatherButton.Click += SaveWeather;

            Controls.Add(cityLabel);
            Controls.Add(cityEntry);
            Controls.Add(getWeatherButton);
            Controls.Add(getForecastButton);
            Controls.Add(saveWeatherButton);
            Controls.Add(weatherInfo);

            this.Width = 400;
            this.Height = 400;
        }

        private void GetWeather(object sender, EventArgs e)
        {
            string city = cityEntry.Text;
            if (string.IsNullOrWhiteSpace(city))
            {
                MessageBox.Show("Please enter a city name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = api.GetWeather(city);
            if (data == null)
            {
                MessageBox.Show("Failed to get weather data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (data.cod != 200)
            {
                MessageBox.Show(data.message ?? "Failed to get weather data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DisplayWeather(data);
            weatherData = data;
        }

        private void GetForecast(object sender, EventArgs e)
        {
            string city = cityEntry.Text;
            if (string.IsNullOrWhiteSpace(city))
            {
                MessageBox.Show("Please enter a city name", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var data = api.GetForecast(city);
            if (data == null)
            {
                MessageBox.Show("Failed to get forecast data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (data.cod != "200")
            {
                MessageBox.Show(data.message ?? "Failed to get forecast data", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DisplayForecast(data);
        }

        private void DisplayWeather(dynamic data)
        {
            weatherInfo.Clear();
            string info =
                $"Weather: {data.weather[0].description}\r\n" +
                $"Temperature: {data.main.temp}°F\r\n" +
                $"Humidity: {data.main.humidity}%\r\n" +
                $"Wind Speed: {data.wind.speed} m/s\r\n" +
                $"Pressure: {data.main.pressure} hPa\r\n" +
                $"Visibility: {(data.visibility != null ? data.visibility.ToString() : "N/A")} meters\r\n" +
                $"Cloudiness: {data.clouds.all}%\r\n" +
                $"Sunrise: {data.sys.sunrise}\r\n" +
                $"Sunset: {data.sys.sunset}\r\n";
            weatherInfo.Text = info;
        }

        private void DisplayForecast(dynamic data)
        {
            weatherInfo.Clear();
            string info = "";
            foreach (var forecast in data.list)
            {
                info +=
                    $"Date/Time: {forecast.dt_txt}\r\n" +
                    $"Weather: {forecast.weather[0].description}\r\n" +
                    $"Temperature: {forecast.main.temp}°F\r\n" +
                    $"Humidity: {forecast.main.humidity}%\r\n" +
                    $"Wind Speed: {forecast.wind.speed} m/s\r\n" +
                    "-------------------------\r\n";
            }
            weatherInfo.Text = info;
        }

        private void SaveWeather(object sender, EventArgs e)
        {
            if (weatherData != null)
            {
                string city = cityEntry.Text;
                api.WriteToFile(city, weatherData);
                MessageBox.Show("Weather data saved to file", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No weather data to save", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}