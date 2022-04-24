using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.Json;

namespace sma_visualisation
{
    public class ApiProcessing
    {
        public static Data loadAPI(string symbol, string interval, string time_period, string series_type, string interval_view)
        {          
            string QUERY_URL = "https://www.alphavantage.co/query?function=SMA&symbol=" + symbol + "&interval=" + interval + "&time_period=" + time_period + "&series_type=" + series_type + "&apikey=EFYWRGACKQN6I4T3";
            Uri queryUri = new Uri(QUERY_URL);


            using (WebClient client = new WebClient())
            {
                dynamic json_data;
                try
                {
                    json_data = JsonSerializer.Deserialize<Dictionary<string, dynamic>>(client.DownloadString(queryUri));
                }catch(Exception e)
                {
                    throw new NoConnectionException("There is not internet connection. Please check your connection");
                }
                if (json_data.Count == 0)
                {
                    throw new NoDataException( "No data for this entry.");
                   
                    
                }
                else if(json_data.ContainsKey("Error Message"))// == "{\"Error Message\": \"Invalid API call. Please retry or visit the documentation (https://www.alphavantage.co/documentation/) for SMA.\"}")
                {
                    throw new InvalidApiCallException("Invalid API call");
                   
                }
                else { 

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


                string interval_data = data_lines[3].Split(":")[2];
                interval_data = interval_data.Replace("\"", "").Trim();
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

                DateTime date;
                double value;
                DateTime last_refreshed_data = new DateTime();
                foreach (string line in value_lines)
                {
      
                    if (interval_data == "1min" || interval_data == "5min" || interval_data == "15min" || interval_data == "30min" || interval_data == "60min")
                    {
                        string [] tokens = line.Split(":");
                        value = Double.Parse(tokens[3].Trim(), CultureInfo.InvariantCulture);
                        string date_time = line.Split(":")[0] +":"+ line.Split(":")[1];
                        date = parseDateTime(date_time.Trim());
                        string dateTimeSeconds = data_lines[2].Split(":")[2] + ":"+ data_lines[2].Split(":")[3] + ":" + data_lines[2].Split(":")[4];
                        last_refreshed_data = parseDateTimeSeconds(dateTimeSeconds.Replace("\"", "").Trim());
                    }
                    else
                    {
                        string[] tokens = line.Split(":");
                        value = Double.Parse(tokens[2].Trim(), CultureInfo.InvariantCulture);
                        date = parseDate(line.Split(":")[0]);
                        last_refreshed_data = parseDate(data_lines[2].Split(":")[2].Replace("\"", ""));

                    }

                    SMAValue sma_value = new SMAValue { Date = date, Value=value };
                    sma_values.Add(sma_value);

                }
                
                Data meta_data = new Data { 
                    symbol = symbol_data, 
                    function = function_data, 
                    interval = interval_data, 
                    last_refreshed_date = last_refreshed_data, 
                    series_type = series_type_data, 
                    time_period = time_period_data,
                    interval_view = interval_view,
                    Values = sma_values };
                return meta_data;
                }
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

        private static DateTime parseDateTime(string dateTime)
        {
            string [] tokens = dateTime.Split(" ");
            int year = int.Parse(tokens[0].Split("-")[0]);
            int month = int.Parse(tokens[0].Split("-")[1]);
            int day = int.Parse(tokens[0].Split("-")[2]);
            int hour = int.Parse(tokens[1].Split(":")[0]);
            int min = int.Parse(tokens[1].Split(":")[1]);

            DateTime dateParsed = new DateTime(year, month, day, hour, min, 0);
            return dateParsed;


        }

        private static DateTime parseDateTimeSeconds(string date) {
            string[] tokens = date.Split(" ");
            int year = int.Parse(tokens[0].Split("-")[0]);
            int month = int.Parse(tokens[0].Split("-")[1]);
            int day = int.Parse(tokens[0].Split("-")[2]);
            int hour = int.Parse(tokens[1].Split(":")[0]);
            int min = int.Parse(tokens[1].Split(":")[1]);
            int sec = int.Parse(tokens[1].Split(":")[2]);

            DateTime dateParsed = new DateTime(year, month, day, hour, min, sec);
            return dateParsed;
        }

}
}
