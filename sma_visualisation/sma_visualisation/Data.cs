﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualisation
{
    public class Data
    {
        public string symbol { get; set; } = "DIn";
        public string function { get; set; } = "fun";
        public DateTime last_refreshed_date { get; set; } = DateTime.Now;
        public string interval { get; set; } = "close";
        public int time_period { get; set; } = 350;
        public string series_type { get; set; } = "open";
        public string interval_view { get; set; } = "month"; //jos ne znamo sta je, 1 gdoina, 2 godine 
        public List<SMAValue> values { get; set; }    
    }
}