using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace sma_visualisation
{
    public class ApiProcessing
    {
        public static Data loadAPI(string symbol, string interval, string time_period, string series_type)
        {

            string QUERY_URL = "https://www.alphavantage.co/query?function=SMA&symbol=" + symbol + "&interval=" + interval + "&time_period=" + time_period + "&series_type=" + series_type + "&apikey=EFYWRGACKQN6I4T3";
            Uri queryUri = new Uri(QUERY_URL);


            using (WebClient client = new WebClient())
            {

                dynamic json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                string data = Convert.ToString(json_data["Meta Data"]);
                Console.WriteLine(data);
                data = data.Replace("{", "");
                data = data.Replace("}", "");
                Console.WriteLine(data);
                string[] data_lines = data.Split(",");
                string symbol_data = data_lines[0].Split(":")[2];
                string function_data = data_lines[1].Split(":")[2];
                DateTime last_refreshed_data = parseDate(data_lines[2].Split(":")[2]);
                string interval_data = data_lines[3].Split(":")[2];
                int  time_period_data = int.Parse(data_lines[4].Split(":")[2]);
                string series_type_data = data_lines[5].Split(":")[2];
                string time_zone_data = data_lines[6].Split(":")[2];
                List<SMAValue> sma_values = new List<SMAValue>();
                //values
                //if (json_data.ContainsKey("Meta Data")) { values = json_data.GetValue("Technical Analysis: SMA").ToString(); }
                string values =json_data["Technical Analysis: SMA"].ToString();
                values = values.Replace("\"", "");
                values = values.Replace("{", "");
                values = values.Replace("}", "");
                string[] value_lines = values.Split(",");
                foreach (string line in value_lines)
                {
                    string date_time = line.Split(":")[0];
                    double value = Double.Parse(line.Split(":")[2]);
                    DateTime date = parseDate(date_time);
                    SMAValue sma_value = new SMAValue { date = date,value=value };
                    sma_values.Add(sma_value);
                }
                
                Data meta_data = new Data { symbol = "", function = "", interval = "", interval_view = "", last_refreshed_date = DateTime.Now, series_type = "", time_period = 10, values = sma_values };
                return meta_data;
            }

    }

        private static DateTime parseDate(string date)
        {
            int year = int.Parse(date.Split("-")[0]);
            int month = int.Parse(date.Split("-")[1]);
            int day = int.Parse(date.Split("-")[2]);
            DateTime dateParsed = new DateTime(year, month, day);
            return dateParsed;
        }
}
}
