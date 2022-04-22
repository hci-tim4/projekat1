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
            string interval = "";
            string series_type = "";
            while(!ValidationEntry.validateComboBox(interval_cb))
            {
                interval_validate_label.Content = "Izaberite neki od ponudjenih intervala!";
            }
            
            ComboBoxItem icbi = (ComboBoxItem)interval_cb.SelectedItem;
            interval = icbi.Content.ToString();
            
            interval_validate_label.Content = "";
            
            
            //tekst box i slider provera??
            int time_period = (int)timePeriodSlider.Value;
            while (!ValidationEntry.validateComboBox(series_type_cb))
            {
                series_type_validate_label.Content = "Izaberite neki od ponudjenih perioda!";
            }
            series_type_validate_label.Content = "";
            ComboBoxItem cbi = (ComboBoxItem)series_type_cb.SelectedItem;
            series_type = cbi.Content.ToString();

            while (!ValidationEntry.emptyTextBox(symbol_tb))
            {
                symbol_validate_label.Content = "Unesite simbol!"; //PROVERIITI!!
                
            }
            symbol_validate_label.Content = "";
            string symbol = symbol_tb.Text;
            while (!ValidationEntry.validateComboBox(interval_view_cb))
            {
                view_interval_validate_label.Content = "Izaberite neki od ponudjenih intervala!";
            }
            view_interval_validate_label.Content = "";
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
            Data loaded_data = ApiProcessing.loadAPI(symbol, interval, Convert.ToString(time_period), series_type);
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
            }
        }
  

        private Data filter_data(Data loaded_data, string interval_view_days)
        {
            List<SMAValue> filtered_list = new List<SMAValue>();
            foreach(SMAValue value in loaded_data.values){
                DateTime date = value.date;
                DateTime date_now = DateTime.Now;
                var difference_dates = date_now - date;
                if(difference_dates.Days < int.Parse(interval_view_days))
                {
                    filtered_list.Add(value);
                }
            }
            loaded_data.values = filtered_list;
            
            return loaded_data;
        }

    }
}
