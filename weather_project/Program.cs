using System;
using System.Collections.Generic;
using System.Net.Http;
//using System.Text.Json;
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
                        var forecastJSON = await response.Content.ReadAsStringAsync();
                        //var forecast = System.Text.Json.JsonSerializer.Deserialize<WeatherOfTheCity>(forecastJSON);
                        //IEnumerable<string> categories = await response.Content.ReadAsAsync<IEnumerable<string>>();

                    }
                    Console.WriteLine($"Погода в {answear} отличная");
                }
            }
        }
    }

    public class WeatherOfTheCity
    {
        public double latitude { get; set; }
        public double longitude { get; set; }
        public List<WeatherForAnHour> weatherForAnHourList { get; set; }
    }

    public class WeatherForAnHour
    {
        public DateTime time { get; set; }
        public double temperature { get; set; }
    }
}
