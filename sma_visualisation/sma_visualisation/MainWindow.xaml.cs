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
            
        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            List<string> hasToBeAdded = new List<string>();
            string interval = "";
            string series_type = "";
            if (!ValidationEntry.validateComboBox(interval_cb))
            {
                hasToBeAdded.Add("interval");
            }


            //tekst box i slider provera??
            int time_period = (int)timePeriodSlider.Value;
            if (!ValidationEntry.validateComboBox(series_type_cb))
            {
                hasToBeAdded.Add("period");
            }

            if (!ValidationEntry.emptyTextBox(symbol_tb))
            {
                hasToBeAdded.Add("simbol");
                //symbol_validate_label.Content = "Unesite simbol!"; //PROVERIITI!!

            }

            if (!ValidationEntry.validateComboBox(interval_view_cb))
            {
                hasToBeAdded.Add("interval of years");
            }

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

            ComboBoxItem icbi = (ComboBoxItem)interval_cb.SelectedItem;
            interval = icbi.Content.ToString();
            ComboBoxItem cbi = (ComboBoxItem)series_type_cb.SelectedItem;
            series_type = cbi.Content.ToString();

            string symbol = symbol_tb.Text;

            string interval_view = ((System.Windows.Controls.ComboBoxItem)interval_view_cb.SelectedItem).Content as string;
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

            try
            {
                Data loaded_data = ApiProcessing.loadAPI(symbol, interval, Convert.ToString(time_period), series_type);
                currentData = loaded_data;
                if (interval_view_days == "all")
                {
                    //tabele i grafik
                    symbol_label.Content = loaded_data.symbol;
                    interval_label.Content = loaded_data.interval;
                    function_label.Content = loaded_data.function;
                    last_refreshed_date_label.Content = Convert.ToString(loaded_data.last_refreshed_date);
                    series_type_label.Content = loaded_data.series_type;
                    time_period_label.Content = loaded_data.time_period;
                    interval_view_label.Content = interval_view;

                }
                else
                {
                    Data filtered_data = filter_data(loaded_data, interval_view_days);
                    symbol_label.Content = loaded_data.symbol;
                    interval_label.Content = loaded_data.interval;
                    function_label.Content = loaded_data.function;
                    last_refreshed_date_label.Content = Convert.ToString(loaded_data.last_refreshed_date);
                    series_type_label.Content = loaded_data.series_type;
                    time_period_label.Content = loaded_data.time_period;
                    interval_view_label.Content = interval_view;
                    currentData = filtered_data;
                }





            }
            catch(NoDataException exeption)
            {
                MessageBox.Show(exeption.message);
            }
            catch(InvalidApiCallException exception)
            {
                MessageBox.Show(exception.message);
            }
            PrepareGraph();
//            PrepareBarsGraph();

        }

        private void PrepareBarsGraph()
        {
            ChartValues<double> values = new ChartValues<double>();

            foreach (SMAValue sma in currentData.Values)
            {
                values.Add(sma.Value);
                lineChartData.xAxisLabels.Add(sma.Date.ToString("dd. MM. yyyy."));
                lineChartData.yAxisLabels.Add(sma.Value.ToString());
            }


            barChartData.columnSeriesCollection.Add(new ColumnSeries()
            {
                Title = "Bar Chart",
                Values = values,
          //      Configuration = new CartesianMapper<double>().Y(value => value).Stroke(value => (value == values.Max()) ? Brushes.LightGreen : (value == values.Min()) ? Brushes.LightPink : Brushes.CornflowerBlue).Fill(value => (value == values.Max()) ? Brushes.LightGreen : (value == values.Min()) ? Brushes.LightPink : Brushes.AliceBlue),
                PointGeometry = DefaultGeometries.Diamond

            });

            var barChartObject = (CartesianChart)this.FindName("barChart");
            barChartObject.HideLegend();


            /*  barChartData.columnSeriesCollection = new SeriesCollection
              {
                  new ColumnSeries
                  {
                      Values = new ChartValues<double> { 10, 50, 39, 50 }//REDEFINISATI
                  }
              };

              //adding series will update and animate the chart automatically
              barChartData.lineSeriesCollection.Add(new ColumnSeries
              {
                  Values = new ChartValues<double> { 11, 56, 42 }//REDEFINISATI
              });

              //also adding values updates and animates the chart automatically
              barChartData.lineSeriesCollection[0].Values.Add(48d);
            */


        }

    

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

                barChartData.xAxisLabels.Add(sma.Date.ToString("dd. MM. yyyy."));
                barChartData.yAxisLabels.Add(sma.Value.ToString());

            }

            lineChartData.lineSeriesCollection.Add(new LineSeries()
            {
                Title = "Line chart",
                Values = values,
            //    Configuration = new CartesianMapper<double>().Y(value => value).Stroke(value => (value == values.Max())),
                PointGeometry = DefaultGeometries.Diamond,
                PointGeometrySize = 8,
            });

            barChartData.columnSeriesCollection.Add(new ColumnSeries()
            {
                Title = "Bar Chart",
                Values = values,
                //      Configuration = new CartesianMapper<double>().Y(value => value).Stroke(value => (value == values.Max()) ? Brushes.LightGreen : (value == values.Min()) ? Brushes.LightPink : Brushes.CornflowerBlue).Fill(value => (value == values.Max()) ? Brushes.LightGreen : (value == values.Min()) ? Brushes.LightPink : Brushes.AliceBlue),
                PointGeometry = DefaultGeometries.Diamond

            });

            DataContext = this;

            var lineChartObject = (CartesianChart)this.FindName("lineChart");
            lineChartObject.HideLegend();


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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in openedWindows)
            {
                w.Close();
            }
        }

    }
}
