using System;
using LiveCharts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sma_visualisation
{
    public class LineChartData
    {
        public List<String> xAxisLabels { get; set; }
        public SeriesCollection lineSeriesCollection { get; set; }

        public LineChartData()
        {
            lineSeriesCollection = new SeriesCollection();
            xAxisLabels = new List<string> { DateTime.Now.ToString("dd. MM. yyyy.") };
        }

        public void reset()
        {
            lineSeriesCollection.Clear();
            xAxisLabels.Clear();
        }

    }
}
