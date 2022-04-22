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
                if(json_data == "{\"Error Message\": \"Invalid API call. Please retry or visit the documentation (https://www.alphavantage.co/documentation/) for SMA.\"}")
                {
                    //kako prikazatii??
                }
                string data = Convert.ToString(json_data["Meta Data"]);
                Console.WriteLine(data);
                data = data.Replace("\n", "");
                data = data.Replace("{", "");
                data = data.Replace("}", "");
                Console.WriteLine(data);
                string[] data_lines = data.Split(",");
                string symbol_data = data_lines[0].Split(":")[2];
                symbol_data = symbol_data.Replace("\"", "");
                string function_data = data_lines[1].Split(":")[2];
                function_data = function_data.Replace("\"", "");
                DateTime last_refreshed_data = parseDate(data_lines[2].Split(":")[2].Replace("\"",""));
                string interval_data = data_lines[3].Split(":")[2];
                interval_data = interval_data.Replace("\"", "");
                int  time_period_data = int.Parse(data_lines[4].Split(":")[2].Replace("\"", ""));
                string series_type_data = data_lines[5].Split(":")[2];
                series_type_data = series_type_data.Replace("\"", "");
                string time_zone_data = data_lines[6].Split(":")[2];
                time_zone_data = time_zone_data.Replace("\"", "");
                List<SMAValue> sma_values = new List<SMAValue>();
                //Values
                //if (json_data.ContainsKey("Meta Data")) { Values = json_data.GetValue("Technical Analysis: SMA").ToString(); }
                string values =json_data["Technical Analysis: SMA"].ToString();
                values = values.Replace("\"", "");
                values = values.Replace("{", "");
                values = values.Replace("}", "");
                values = values.Replace("\n", "");
                string[] value_lines = values.Split(",");
                foreach (string line in value_lines)
                {
                    string date_time = line.Split(":")[0];
                    double value = Double.Parse(line.Split(":")[2]);
                    DateTime date = parseDate(date_time);
                    SMAValue sma_value = new SMAValue { Date = date,Value=value };
                    sma_values.Add(sma_value);
                }
                
                Data meta_data = new Data { symbol = symbol_data, function = function_data, interval = interval_data, last_refreshed_date = last_refreshed_data, series_type = series_type_data, time_period = time_period_data, Values = sma_values };
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
