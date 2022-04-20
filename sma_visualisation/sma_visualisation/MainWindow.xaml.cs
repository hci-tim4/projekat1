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

namespace sma_visualisation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            object wantedNode = app.FindName("DisplaySection");
            if (wantedNode is StackPanel){
                StackPanel panel = wantedNode as StackPanel;
                panel.DataContext = new Data();
            }
            

        }

        private void show_btn_Click(object sender, RoutedEventArgs e)
        {
            string interval = interval_cb.SelectedItem.ToString();
            int time_period = (int)timePeriodSlider.Value;
            string series_type = (string)series_type_cb.SelectedItem.ToString();
            string symbol = symbol_tb.Text;
            string interval_view = (string)interval_view_cb.SelectedItem.ToString();
            string interval_view_days = "all";
            if(interval_view == "one year")
            {
                interval_view_days = "366";
            }
            else if(interval_view == "two years")
            {
                interval_view_days = "732";
            }
            else if(interval_view == "three years")
            {
                interval_view_days = "1098";
            }
            Data loaded_data = ApiProcessing.loadAPI(symbol, interval, Convert.ToString(time_period), series_type);
            if (interval_view_days == "all")
            {
                result_tb.Text = loaded_data.symbol;
            }
            else
            {
                Data filtered_data = filter_data(loaded_data, interval_view_days);
                result_tb.Text = filtered_data.symbol;
            }
        }

        private Data filter_data(Data loaded_data, string interval_view_days)
        { 
            foreach(SMAValue value in loaded_data.values){
                DateTime date = value.date;
                DateTime date_now = DateTime.Now;
                var difference_dates = date_now - date;
                if(difference_dates.Days > int.Parse(interval_view_days))
                {
                    loaded_data.values.Remove(value);
                }
            }
            return loaded_data;
        }

        private void show_btn_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
