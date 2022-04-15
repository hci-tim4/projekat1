using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualization
{
    public class Data
    {
        public string symbol {get;set;}
        public string function { get; set; }
        public DateTime last_refreshed_date { get; set; }
        public string interval { get; set; }
        public int time_period { get; set; }
        public string series_type { get; set; }
        public string interval_view { get; set; } //jos ne znamo sta je, 1 gdoina, 2 godine
        public List<SMAValue> values { get; set; }    
    }
}
