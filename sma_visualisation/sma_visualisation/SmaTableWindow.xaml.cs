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
using System.Windows.Shapes;

namespace sma_visualisation
{
    /// <summary>
    /// Interaction logic for SmaTableWindow.xaml
    /// </summary>
    public partial class SmaTableWindow : Window
    {
        int pageIndex = 1;
        private int numberOfRecPerPage = 5;
        private Data originalData;
        private Data currentData;
        private SearchOptions searchBy;
        private enum PagingMode
        {
            First = 1, Next = 2, Previous = 3, Last = 4, PageCountChange = 5
        }
        public SmaTableWindow(Data data)
        {
            InitializeComponent();
            originalData = data;
            currentData = new Data();
            copyValues();
            //currentData = data;

            this.DataContext = currentData;
            //cbNumberOfRecords.SelectedItem = 10;
            dataGrid.ItemsSource = currentData.Values.GetRange(0, numberOfRecPerPage);
            refreshPagination();
            searchBy = new SearchOptions();
            searchPanel.DataContext = searchBy;
        }

        private void copyValues()
        {
            currentData.Values = new List<SMAValue>();
            foreach (SMAValue v in originalData.Values)
            {
                currentData.addValue(v);
            }
        }

        private void refreshPagination()
        {
            try
            {
                btnFirst.IsEnabled = true;
                btnPrev.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnLast.IsEnabled = true;
                int count = (pageIndex * numberOfRecPerPage);
                if (count > currentData.Values.Count)
                {
                    count = currentData.Values.Count;
                }
                lblpageInformation.Content = count + " of " + currentData.Values.Count;

                int numPagination = currentData.Values.Count % numberOfRecPerPage;
                if (pageIndex == 1)
                {
                    btnFirst.IsEnabled = false;
                    btnPrev.IsEnabled = false;
                }
                if (count == currentData.Values.Count)
                {
                    btnNext.IsEnabled = false;
                    btnLast.IsEnabled = false;
                }
            }
            catch (Exception e)
            {

            }
        }

        private void btnFirst_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.First);
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Next);

        }

        private void btnPrev_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Previous);

        }

        private void btnLast_Click(object sender, System.EventArgs e)
        {
            Navigate((int)PagingMode.Last);
        }

        private void cbNumberOfRecords_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Navigate((int)PagingMode.PageCountChange);
        }

        private void Navigate(int mode)
        {
            int count = 0;
            Data data = currentData;
            switch (mode)
            {
                case (int)PagingMode.Next:
                    btnPrev.IsEnabled = true;
                    btnFirst.IsEnabled = true;
                    if (data.Values.Count >= (pageIndex * numberOfRecPerPage))
                    {
                        if (data.Values.Skip(pageIndex *
                        numberOfRecPerPage).Take(numberOfRecPerPage).Count() == 0)
                        {
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = data.Values.Skip((pageIndex *
                            numberOfRecPerPage) - numberOfRecPerPage).Take(numberOfRecPerPage);
                            count = (pageIndex * numberOfRecPerPage); //+
                            //(data.Values.Skip(pageIndex *
                            //numberOfRecPerPage).Take(numberOfRecPerPage)).Count();
                        }
                        else
                        {
                            dataGrid.ItemsSource = null;
                            dataGrid.ItemsSource = data.Values.Skip(pageIndex *
                            numberOfRecPerPage).Take(numberOfRecPerPage);
                            count = (pageIndex * numberOfRecPerPage);// +(data.Values.Skip(pageIndex *numberOfRecPerPage).Take(numberOfRecPerPage)).Count();
                            pageIndex++;
                        }

                        //lblpageInformation.Content = count + " of " + data.Values.Count;
                    }

                    else
                    {
                        count = (pageIndex * numberOfRecPerPage);
                        btnNext.IsEnabled = false;
                        btnLast.IsEnabled = false;
                    }

                    break;
                case (int)PagingMode.Previous:
                    btnNext.IsEnabled = true;
                    btnLast.IsEnabled = true;
                    if (pageIndex > 1)
                    {
                        pageIndex -= 1;
                        dataGrid.ItemsSource = null;
                        if (pageIndex == 1)
                        {
                            dataGrid.ItemsSource = data.Values.Take(numberOfRecPerPage);
                            count = data.Values.Take(numberOfRecPerPage).Count();
                            lblpageInformation.Content = count + " of " + data.Values.Count;
                        }
                        else
                        {
                            dataGrid.ItemsSource = data.Values.Skip
                            (pageIndex * numberOfRecPerPage).Take(numberOfRecPerPage);
                            count = Math.Min(pageIndex * numberOfRecPerPage, data.Values.Count);
                            lblpageInformation.Content = count + " of " + data.Values.Count;
                        }
                    }
                    else
                    {
                        btnPrev.IsEnabled = false;
                        btnFirst.IsEnabled = false;
                    }
                    break;

                case (int)PagingMode.First:
                    pageIndex = 2;
                    Navigate((int)PagingMode.Previous);
                    break;
                case (int)PagingMode.Last:
                    pageIndex = (data.Values.Count / numberOfRecPerPage);
                    Navigate((int)PagingMode.Next);
                    break;

                case (int)PagingMode.PageCountChange:
                    pageIndex = 1;
                    numberOfRecPerPage = Convert.ToInt32(((System.Windows.Controls.ComboBoxItem)cbNumberOfRecords.SelectedItem).Content as string);
                    dataGrid.ItemsSource = null;
                    try
                    {
                        dataGrid.ItemsSource = data.Values.Take(numberOfRecPerPage);
                        count = (data.Values.Take(numberOfRecPerPage)).Count();
                        lblpageInformation.Content = count + " of " + data.Values.Count;
                        btnNext.IsEnabled = true;
                        btnLast.IsEnabled = true;
                        btnPrev.IsEnabled = true;
                        btnFirst.IsEnabled = true;
                    }
                    catch (NullReferenceException e)
                    {

                    }
                    break;
            }
            refreshPagination();
        }


        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            startDate.DisplayDateEnd = endDate.SelectedDate;
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            endDate.DisplayDateStart = startDate.SelectedDate;
        }

        private void updateDataGrid()
        {
            pageIndex = 1;
            try
            {
                dataGrid.ItemsSource = null;
                dataGrid.ItemsSource = currentData.Values.GetRange(0, numberOfRecPerPage);
            }
            catch (Exception)
            {
                dataGrid.ItemsSource = currentData.Values;
            }
            refreshPagination();
        }

        private void sortBy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string chosedSortBy = ((System.Windows.Controls.ComboBoxItem)sortBy.SelectedItem).Content as string;
            switch (chosedSortBy)
            {
                case "Date":
                    sortByDate();
                    break;
                case "Value":
                    sortByValue();
                    break;
            }
            updateDataGrid();
        }

        private void sortByDate()
        {
            currentData.Values = (from values in currentData.Values
                                  orderby values.Date
                                  select values).ToList();
        }

        private void sortByValue()
        {
            currentData.Values = (from values in currentData.Values
                                  orderby values.Value
                                  select values).ToList();
            Console.WriteLine("......");
        }

        private void searchBtn_Click(object sender, RoutedEventArgs e)
        {
            applyDateIntervall();
            applyValueCriteria();
            updateDataGrid();
        }

        private void applyValueCriteria()
        {
            string relOp = ((System.Windows.Controls.ComboBoxItem)relOpValue.SelectedItem).Content as string;
            //no selected Value
            string valueStr = valueTextBox.Text;
            if (valueStr == "")
            {
                return;
            }
            double value = 0;
            double.TryParse(valueStr, out value);
            //not string

            getAdecvatValues(relOp, value);

        }

        private void applyDateIntervall()
        {
            searchBy.startSelectedDate = startDate.SelectedDate;
            searchBy.endSelectedDate = endDate.SelectedDate;
            if (null == searchBy.startSelectedDate & searchBy.endSelectedDate != null)
            {
                MessageBox.Show("If end Date is given, there has to be given a start Date too.");
                return;
            }
            if (searchBy.startSelectedDate != null & searchBy.endSelectedDate == null)
            {
                MessageBox.Show("If start Date is given, there has to be given an end Date too.");
                return;
            }
            if (searchBy.startSelectedDate == null & searchBy.endSelectedDate == null)
            {
                //nista
                return;
            }
            getAdecvatInterval(searchBy.startSelectedDate, searchBy.endSelectedDate);
            searchBy.endSelectedDate = default(DateTime);
            searchBy.startSelectedDate = null;
        }

        private void getAdecvatValues(string relOp, double value)
        {
            switch (relOp)
            {
                case "<":
                    currentData.Values = (from values in currentData.Values
                                          where values.Value < value
                                          select values).ToList();
                    break;
                case "<=":
                    currentData.Values = (from values in currentData.Values
                                          where values.Value <= value
                                          select values).ToList();
                    break;
                case ">":
                    currentData.Values = (from values in currentData.Values
                                          where values.Value > value
                                          select values).ToList();
                    break;
                case ">=":
                    currentData.Values = (from values in currentData.Values
                                          where values.Value >= value
                                          select values).ToList();
                    break;
                case "==":
                    currentData.Values = (from values in currentData.Values
                                          where values.Value == value
                                          select values).ToList();
                    break;
            }
        }

        private void getAdecvatInterval(DateTime? fromDate, DateTime? untilDate)
        {
            Console.WriteLine("");
            currentData.Values = (from values in currentData.Values
                                  where values.Date >= fromDate && values.Date <= untilDate
                                  select values).ToList();

        }

        private void resetBtn_Click(object sender, RoutedEventArgs e)
        {
            copyValues();
            updateDataGrid();
            refreshPagination();
        }

        private void currentDataBtn_Click(object sender, RoutedEventArgs e)
        {
            string msg = "Current data:";
            msg = msg + "\n Symbol: " + originalData.symbol;
            msg = msg + "\n Interval: " + originalData.interval;
            msg = msg + "\n Time period: " + originalData.time_period;
            msg = msg + "\n Function: " + originalData.function;
            msg = msg + "\n Last refreshed: " + originalData.last_refreshed_date.ToString("dd.MM.yyyy. HH:mm");
            msg = msg + "\n In the past: " + originalData.interval_view;
            msg = msg + "\n Series type: " + originalData.series_type;
            MessageBox.Show(msg, "Current data:");

        }
    }
}
