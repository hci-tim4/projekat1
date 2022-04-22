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
        public List<String> yAxisLabels { get; set; }
        public SeriesCollection lineSeriesCollection { get; set; }
        public SeriesCollection columnSeriesCollection { get; set; }

        public LineChartData()
        {
            lineSeriesCollection = new SeriesCollection();
            columnSeriesCollection = new SeriesCollection();
            xAxisLabels = new List<string> { DateTime.Now.ToString("dd. MM. yyyy.") };
            yAxisLabels = new List<string> { "10", "20", "30", "40"};
        }

        public void reset()
        {
            lineSeriesCollection.Clear();
            columnSeriesCollection.Clear();
            xAxisLabels.Clear();
            yAxisLabels.Clear();
        }

    }
}
