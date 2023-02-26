using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            Console.WriteLine("Погода");
            while (true)
            {
                string answear = Console.ReadLine();
                if (answear == "-q" || answear == "--quit")
                {
                    return;
                }
                else if (needHelp || answear == "-h" || answear == "--help")
                {
                    Console.WriteLine("Вспомогательная информация");
                    needHelp = false;
                }
                else
                {
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync("https://api.open-meteo.com/v1/forecast?latitude=57.63&longitude=39.87&hourly=temperature_2m&start_date=2023-02-05&end_date=2023-02-08");

                    if (response.IsSuccessStatusCode)
                    {
                        //получаем погодные значения
                        var forecastJSON = await response.Content.ReadAsStringAsync();
                        var forecastJSONDocument = System.Text.Json.JsonDocument.Parse(forecastJSON);
                        string latitude = forecastJSONDocument.RootElement.GetProperty("latitude").ToString();
                        string longitude = forecastJSONDocument.RootElement.GetProperty("longitude").ToString();
                        var hours = forecastJSONDocument.RootElement.GetProperty("hourly").GetProperty("time");
                        var temperatures = forecastJSONDocument.RootElement.GetProperty("hourly").GetProperty("temperature_2m");
                        List<ForecastForAnHour> forecastForAnHours = new List<ForecastForAnHour>();
                        for (int i = 0; i < hours.GetArrayLength(); i++)
                        {
                            forecastForAnHours.Add(new ForecastForAnHour { time = hours[i].ToString(), temperature = temperatures[i].ToString() });
                        }
                        ForecastOfTheCity forecast = new ForecastOfTheCity
                        {
                            city = answear,
                            latitude = latitude,
                            longitude = longitude,
                            forecastForAnHours = forecastForAnHours

                        };
                        Console.WriteLine($"Погода в {answear} отличная");

                    }
                    Console.WriteLine($"Погода в {answear} отличная");
                }
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
