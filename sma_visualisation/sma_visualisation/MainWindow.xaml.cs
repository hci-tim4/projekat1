using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using LiveCharts;
using LiveCharts.Wpf;
using LiveCharts.Configurations;
using Image = System.Windows.Controls.Image;

namespace sma_visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Window> openedWindows = new List<Window>();
        List<string> hasToBeAdded = new List<string>();
        Data currentData;
        public LineChartData lineChartData { get; set; }
        public LineChartData barChartData { get; set; }
       
        public MainWindow()
        {
            InitializeComponent();
            object wantedNode = app.FindName("DisplaySection");
            if (wantedNode is StackPanel){
                StackPanel panel = wantedNode as StackPanel;
                panel.DataContext = new Data();
            }
            
            lineChartData = new LineChartData();
            lineChart.DataContext = this;
            barChartData = new LineChartData();
            barChartPanel.DataContext = this;
            Formatter = value => value.ToString("N");
        }

        private string read_interval_cb()
        {
            if (!ValidationEntry.validateComboBox(interval_cb))
            {
                hasToBeAdded.Add("interval");
            }
            ComboBoxItem icbi = (ComboBoxItem)interval_cb.SelectedItem;
            string interval = icbi.Content.ToString();
            return interval;
        }

        private string read_series_type_cb()
        {
            
            if (!ValidationEntry.validateComboBox(series_type_cb))
            {
                hasToBeAdded.Add("period");
            }
            ComboBoxItem cbi = (ComboBoxItem)series_type_cb.SelectedItem;
            string series_type = cbi.Content.ToString();
            return series_type;
        }

        private string read_interval_view_cb()
        {
            
            if (!ValidationEntry.validateComboBox(interval_view_cb))
            {
                hasToBeAdded.Add("interval of years");
            }
            string interval_view = ((System.Windows.Controls.ComboBoxItem)interval_view_cb.SelectedItem).Content as string;
            return interval_view;
        }

        private string read_symbol_tb()
        {
            if (!ValidationEntry.emptyTextBox(symbol_tb))
            {
                hasToBeAdded.Add("simbol");
            }
            string symbol = symbol_tb.Text;
            return symbol;
        }

        private string return_interval_view_days(string interval_view)
        {
            string interval_view_days = "all";

            if (interval_view == "one year")
            {
                interval_view_days = "366";
            }
            else if (interval_view == "two years")
            {
                interval_view_days = "732";
            }
            else if (interval_view == "three years")
            {
                interval_view_days = "1098";
            }
            return interval_view_days;
        }

        private int read_time_period_value()
        {
            return (int)timePeriodSlider.Value; 
        }
        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            string symbol = read_symbol_tb();
            int time_period = read_time_period_value();
            string series_type = read_series_type_cb();
            string interval = read_interval_cb();
            string interval_view = read_interval_view_cb();
            string interval_view_days = return_interval_view_days(interval_view);

            if (hasToBeAdded.Count != 0)
            {
                string notify = "You have to give the following parameters: ";
                foreach(string str in hasToBeAdded)
                {
                    notify = notify + str ;
                    if (str != hasToBeAdded[hasToBeAdded.Count - 1])
                    {
                        notify = notify + " ,";
                    }
                    else
                    {
                        notify = notify + "!";
                    }
                }
                MessageBox.Show(notify);
                return;
            }

           
          
            
            Window ibox = new InformationBox();
            try
            {
                ibox.Show();
                Data loaded_data = ApiProcessing.loadAPI(symbol, interval, Convert.ToString(time_period), series_type, interval_view_days);
                currentData = loaded_data;
                if (interval_view_days != "all")
                {

                    Data filtered_data = filter_data(loaded_data, interval_view_days);
                    currentData = filtered_data;
                }
                page.Content = new InformationDisplay(currentData);

                PrepareGraph();

            }
            catch(NoDataException exeption)
            {
                MessageBox.Show(exeption.message);
            }
            catch(InvalidApiCallException exception)
            {
                MessageBox.Show(exception.message);
            }
            catch (NoConnectionException noConnection)
            {
                MessageBox.Show(noConnection.message);
            }
            finally
            {
                ibox.Close();
            }
        }
        public Func<double, string> Formatter { get; set; }


        private void PrepareGraph()
        {

            lineChartData.reset();
            barChartData.reset();

            ChartValues<double> values = new ChartValues<double>();

            foreach (SMAValue sma in currentData.Values)
            {
                values.Add(sma.Value);
                lineChartData.xAxisLabels.Add(sma.Date.ToString("dd. MM. yyyy."));
                lineChartData.yAxisLabels.Add(sma.Value.ToString());


            }

            lineChartData.lineSeriesCollection.Add(new LineSeries()
            {
                Title = "Line chart",
                Values = values,
                PointGeometry = DefaultGeometries.Diamond,
                PointGeometrySize = 8,
            });

            ChartValues<double> barChartValues = prepareValuesForBarChart();

            barChartData.lineSeriesCollection.Add(new ColumnSeries()
            {
                Title = "Bar Chart",
                Values = barChartValues,
                PointGeometry = DefaultGeometries.Diamond

            });

            DataContext = this;

            var lineChartObject = (CartesianChart)this.FindName("lineChart");
            lineChartObject.HideLegend();
            Formatter = value => value.ToString("N");

        }

        private ChartValues<double> prepareValuesForBarChart()
        {
            List<DateTime> dates = (from allValues in currentData.Values
                                    orderby allValues.Date
                                    select allValues.Date).ToList();

            ChartValues<double> values = new ChartValues<double>();

            if (currentData.Values.Count < 100)
            {
                foreach (SMAValue sma in currentData.Values)
                {
                    values.Add(sma.Value);
                    barChartData.xAxisLabels.Add(sma.Date.ToString("dd. MM. yyyy."));
                }
                barChartX.Title = "Period (dd. MM. yyyy.)";
            }
            else if (currentData.interval_view == "all")
            {
                DateTime minDate = dates.First().Date;
                DateTime maxDate = dates.Last().Date;
                for (; minDate < maxDate; minDate = minDate.AddYears(1))
                {
                    List<double> valuesInYear = (from allValues in currentData.Values
                                                 where allValues.Date.Year == minDate.Year
                                                 select allValues.Value).ToList();
                    double avgValue = valuesInYear.Count > 0 ? valuesInYear.Average() : 0.0;
                    values.Add(avgValue);
                    barChartData.xAxisLabels.Add(minDate.ToString("yyyy."));
                }
                barChartX.Title = "Period (yyyy.)";
                //grupisi po godinama
                //lineChartData.xAxisLabels.Add(); // ubaci godine
            }
            else
            {
                DateTime minDate = dates.First().Date;
                DateTime maxDate = dates.Last().Date;
                for (; minDate < maxDate; minDate = minDate.AddMonths(1))
                {
                    List<double> valuesInMonth = (from allValues in currentData.Values
                                                  where allValues.Date.Year == minDate.Year && allValues.Date.Month == minDate.Month
                                                  select allValues.Value).ToList();
                    double avgValue = valuesInMonth.Count > 0 ? valuesInMonth.Average() : 0.0;
                    values.Add(avgValue);
                    barChartData.xAxisLabels.Add(minDate.ToString("MM.yyyy."));
                }
                barChartX.Title = "Period (MM. yyyy.)";
                //grupisi po mesecima
                //lineChartData.xAxisLabels.Add(); // ubaci mesece
            }

            return values;
        }


        private Data filter_data(Data loaded_data, string interval_view_days)
        {
            List<SMAValue> filtered_list = new List<SMAValue>();
            foreach(SMAValue value in loaded_data.Values){
                DateTime date = value.Date;
                DateTime date_now = DateTime.Now;
                var difference_dates = date_now - date;
                if(difference_dates.Days < int.Parse(interval_view_days))
                {
                    filtered_list.Add(value);
                }
            }
            loaded_data.Values = filtered_list;
            
            return loaded_data;
        }

        private void show_table_Click(object sender, RoutedEventArgs e)
        {
            string symbol = read_symbol_tb();
            int time_period = read_time_period_value();
            string series_type = read_series_type_cb();
            string interval = read_interval_cb();
            string interval_view = read_interval_view_cb();
            string interval_view_days = return_interval_view_days(interval_view);

            if (hasToBeAdded.Count != 0)
            {
                string notify = "You have to give the following parameters: ";
                foreach (string str in hasToBeAdded)
                {
                    notify = notify + str;
                    if (str != hasToBeAdded[hasToBeAdded.Count - 1])
                    {
                        notify = notify + " ,";
                    }
                    else
                    {
                        notify = notify + "!";
                    }
                }
                MessageBox.Show(notify);
                return;
            }
           
            try
            {
                
                Data loaded_data = ApiProcessing.loadAPI(symbol, interval, Convert.ToString(time_period), series_type, interval_view_days);
                currentData = loaded_data;
                if (interval_view_days != "all")
                {

                    Data filtered_data = filter_data(loaded_data, interval_view_days);
                    currentData = filtered_data;
                }
                if (currentData == null)
                {
                    MessageBox.Show("There has to be selected values");
                    return;
                }
                if (currentData.Values.Count == 0)
                {
                    MessageBox.Show("There are no reults for the chosen data");
                    return;
                }
                Window tableWindow = new SmaTableWindow(currentData);
                openedWindows.Add(tableWindow);
                tableWindow.Show();
            }
            catch(NoDataException exeption)
            {
                MessageBox.Show(exeption.message);
            }
            catch(InvalidApiCallException exception)
            {
                MessageBox.Show(exception.message);
            }
            catch (NoConnectionException noConnection)
            {
                MessageBox.Show(noConnection.message);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in openedWindows)
            {
                w.Close();
            }
        }

    }
}
