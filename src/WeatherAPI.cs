using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace WeatherApi
{
    public class WeatherAPI
    {
        private readonly string apiKey;
        private readonly string baseUrl;
        private readonly string forecastUrl;
        private readonly HttpClient httpClient;

        public WeatherAPI(string apiKey = Config.API_KEY, string baseUrl = Config.BASE_URL, string forecastUrl = Config.FORECAST_URL)
        {
            this.apiKey = apiKey;
            this.baseUrl = baseUrl;
            this.forecastUrl = forecastUrl;
            this.httpClient = new HttpClient();
        }

        public JObject GetWeather(string city)
        {
            var url = $"{baseUrl}?q={city}&appid={apiKey}&units=imperial";
            try
            {
                var response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                var data = JObject.Parse(json);
                WriteToFile(city, data.ToString());
                return data;
            }
            catch (HttpRequestException httpErr)
            {
                Console.WriteLine($"HTTP error occurred: {httpErr.Message}");
            }
            catch (Exception err)
            {
                Console.WriteLine($"Other error occurred: {err.Message}");
            }
            return null;
        }

        public JObject GetForecast(string city)
        {
            var url = $"{forecastUrl}?q={city}&appid={apiKey}&units=imperial";
            try
            {
                var response = httpClient.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();
                var json = response.Content.ReadAsStringAsync().Result;
                var data = JObject.Parse(json);
                WriteForecastToFile(city, data.ToString());
                return data;
            }
            catch (HttpRequestException httpErr)
            {
                Console.WriteLine($"HTTP error occurred: {httpErr.Message}");
            }
            catch (Exception err)
            {
                Console.WriteLine($"Other error occurred: {err.Message}");
            }
            return null;
        }

        public void WriteToFile(string city, string data)
        {
            var dir = Path.Combine("data");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, $"{city}_weather_data.txt");
            File.WriteAllText(filePath, data);
        }

        public void WriteForecastToFile(string city, string data)
        {
            var dir = Path.Combine("data");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, $"{city}_forecast_data.txt");
            File.WriteAllText(filePath, data);
        }
    }