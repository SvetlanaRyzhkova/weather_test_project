using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace weather_test_project
{

    class Program
    {
        static async Task Main(string[] args)
        {
            //Добавила юникод для вывода и ввода текста
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            //Обработала появление параметра для получения справки
            bool needHelp = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "-h" || args[i] == "--help")
                {
                    needHelp = true;
                }
            }

            Console.WriteLine("Forecast");
            while (true)
            {
                string answear = Console.ReadLine();
                if (answear == "-q" || answear == "--quit")
                {
                    return;
                }
                else if (needHelp || answear == "-h" || answear == "--help")
                {
                    Console.WriteLine("Help information\nWrite the name of the city and forecast for how many days you need including today.");
                    Console.WriteLine("Examples:\nmoscow -7d\t - 7 days forecast\nmoscow -d\t - today's forecast");
                    Console.WriteLine("If you want to close the program write -q or --quit.");
                    needHelp = false;
                }
                else
                {
                    //получаем город и количество дней для запроса
                    var answearParts = answear.Split(' ');
                    string city = answearParts[0];
                    int numberOfDays = 0;
                    try
                    {
                        numberOfDays = answearParts[1] == "-d" ? 0 : Convert.ToInt32(Regex.Matches(answearParts[1], @"^.*(?=d)")[0].Value.Substring(1));
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Couldn't understand your request. Read the help information.\nTo read help information send -h or --help.");
                        continue;
                    }

                    HttpClient client = new HttpClient();                    
                    // получаем координаты населённого пункта
                    HttpResponseMessage coordinatesResponse = await client.GetAsync($"https://geocoding-api.open-meteo.com/v1/search?name={city}");
                    if (!coordinatesResponse.IsSuccessStatusCode){
                        continue;
                    }
                    var coordinatesJSON = await coordinatesResponse.Content.ReadAsStringAsync();
                    var coordinatesJSONDocument = System.Text.Json.JsonDocument.Parse(coordinatesJSON);
                    JsonElement results;
                    bool isSuccesfull = coordinatesJSONDocument.RootElement.TryGetProperty("results", out results);
                    if (!isSuccesfull) { 
                        Console.WriteLine("Sorry, couldn't find your locality. Try again!");
                        continue;
                    }
                    string latitude = results[0].GetProperty("latitude").ToString(); 
                    string longitude = results[0].GetProperty("longitude").ToString();

                    string startDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
                    string endDate = DateTime.UtcNow.AddDays(numberOfDays).ToString("yyyy-MM-dd");

                    //получаем погодные значения
                    HttpResponseMessage forecastResponse = await client.GetAsync($"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&hourly=temperature_2m&start_date={startDate}&end_date={endDate}");
                    if (!forecastResponse.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Couldn't receive data. Try again!");
                        continue;
                    }
                    var forecastJSON = await forecastResponse.Content.ReadAsStringAsync();
                    var forecastJSONDocument = System.Text.Json.JsonDocument.Parse(forecastJSON);
                    var hours = forecastJSONDocument.RootElement.GetProperty("hourly").GetProperty("time");
                    var temperatures = forecastJSONDocument.RootElement.GetProperty("hourly").GetProperty("temperature_2m");
                    List<ForecastForAnHour> forecastForAnHours = new List<ForecastForAnHour>();
                    for (int i = 0; i < hours.GetArrayLength(); i++)
                    {
                        forecastForAnHours.Add(new ForecastForAnHour { time = hours[i].ToString(), temperature = temperatures[i].ToString() });
                    }
                    ForecastOfTheCity forecast = new ForecastOfTheCity
                    {
                        city = city,
                        latitude = latitude,
                        longitude = longitude,
                        forecastForAnHours = forecastForAnHours

                    };
                    prettyPrint(forecast);
                }
            }

            void prettyPrint(ForecastOfTheCity forecast)
            {
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine($"City - {forecast.city}");
                Console.WriteLine($"Latitude - {forecast.latitude}");
                Console.WriteLine($"Longitude - {forecast.longitude}");
                Console.WriteLine("-------------------------------------------------------------------------------");
                Console.WriteLine("\tDateTime\t\t\tTemperature");
                for (int i = 0; i < forecast.forecastForAnHours.Count(); i++)
                {
                    Console.WriteLine($"\t{forecast.forecastForAnHours[i].time}\t\t{forecast.forecastForAnHours[i].temperature}");
                }
                Console.WriteLine("-------------------------------------------------------------------------------");
            }
        }
    }

    public class ForecastOfTheCity
    {
        public string city { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public List<ForecastForAnHour> forecastForAnHours { get; set; }
    }

    public class ForecastForAnHour
    {
        public string time { get; set; }
        public string temperature { get; set; }
    }
}
