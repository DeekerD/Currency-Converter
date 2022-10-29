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
using System.Data;
using System.Text.RegularExpressions;
using System.Net.Http;
using Newtonsoft.Json;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //creating object of type Root
        Root val = new Root();
        public MainWindow()
        {

            InitializeComponent();
            GetValue();
            ClearControls();
        }

        //creating a method to get value
        public async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=43ce397d6b004799a048cefc96594edd");//API key
            BindCurrency();
        } 

        private static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();

            try
            {
                using(var client = new HttpClient()) //HttpClient class provides a base class for
                                                 //send/ receiving the HTTP requests/response from URL
                {
                    client.Timeout = TimeSpan.FromMinutes(1);//The timespan to wait before request times out

                    HttpResponseMessage response = await client.GetAsync(url);
                    //HttpResponseMessage is a way of returning message/data from your action

                    if (response.StatusCode == System.Net.HttpStatusCode.OK)//Check if Api status is OK
                    {
                        //Serialize Http content to a string as an asynchronous operation
                        var ResponseString = await response.Content.ReadAsStringAsync();

                        //JsonConvert.DeserializeObject to deserialize Json to C#
                        var ResponseObject = JsonConvert.DeserializeObject<Root>(ResponseString);

                        MessageBox.Show("TimeStamp: " + ResponseObject.timestamp, "Information",
                            MessageBoxButton.OK, MessageBoxImage.Information);

                        return ResponseObject; //Return Api status
                    }
                }
            }
            catch 
            {

                return myRoot;
            }

            return myRoot;
        }

        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();

            //Add display column in DataTable
            dtCurrency.Columns.Add("Text");

            //Add value column in DataTable
            dtCurrency.Columns.Add("Value");

            //Add rows in DataTable with text and value
            dtCurrency.Rows.Add("--SELECT__", 0);
            dtCurrency.Rows.Add("NGN", val.rates.NGN);
            dtCurrency.Rows.Add("INR", val.rates.INR);
            dtCurrency.Rows.Add("JPY", val.rates.JPY);
            dtCurrency.Rows.Add("USD", val.rates.USD);
            dtCurrency.Rows.Add("NZD", val.rates.NZD);
            dtCurrency.Rows.Add("EUR", val.rates.EUR);
            dtCurrency.Rows.Add("CAD", val.rates.CAD);
            dtCurrency.Rows.Add("ISK", val.rates.ISK);
            dtCurrency.Rows.Add("PHP", val.rates.PHP);
            dtCurrency.Rows.Add("DKK", val.rates.DKK);
            dtCurrency.Rows.Add("CZK", val.rates.CZK);

            //The data to currency ComboBox is assigned from dataTable
            FromCurrencyCB.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath Property is used to display data in ComboBox
            FromCurrencyCB.DisplayMemberPath = "Text";

            //SelectedValuePath Property is used to set the value in ComboBox
            FromCurrencyCB.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind hint in the ConboBox.  The default value is Select.
            FromCurrencyCB.SelectedIndex = 0;

            //All properties are also set for ToCurrency ComboBox

            //The data to currency ComboBox is assigned from dataTable
            ToCurrencyCB.ItemsSource = dtCurrency.DefaultView;

            //DisplayMemberPath Property is used to display data in ComboBox
            ToCurrencyCB.DisplayMemberPath = "Text";

            //SelectedValuePath Property is used to set the value in ComboBox
            ToCurrencyCB.SelectedValuePath = "Value";

            //SelectedIndex property is used to bind hint in the ConboBox.  The default value is Select.
            ToCurrencyCB.SelectedIndex = 0;
        }

        //ClearControls used to clear all values
        private void ClearControls()
        {
            EnterAmountTB.Text = string.Empty;
            if (FromCurrencyCB.Items.Count > 0)
                FromCurrencyCB.SelectedIndex = 0;
            if (ToCurrencyCB.Items.Count > 0)
                ToCurrencyCB.SelectedIndex = 0;

            MyLabel.Content = " ";
            EnterAmountTB.Focus();
        }

        //Allow only the integer value in the Textbox
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        //Convert button
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            //Create a value as ConvertedValue to store converted value
            double ConvertedValue;

            //Check if amount Textbox is null or Blank
            if(EnterAmountTB.Text == null ||EnterAmountTB.Text.Trim() == "")
            {
                //It should show this message
                MessageBox.Show("Please Enter Currncy",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                EnterAmountTB.Focus();
                return;
            }

            //Else if the currency from is not selected  or it is default text SELECT
            else if(FromCurrencyCB.SelectedValue == null || FromCurrencyCB.SelectedIndex == 0)
            {
                //It should show this message
                MessageBox.Show("Please Select Currency From",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                FromCurrencyCB.Focus();
                return;
            }

            else if (ToCurrencyCB.SelectedValue == null || ToCurrencyCB.SelectedIndex == 0)
            {
                //It should show this message
                MessageBox.Show("Please Select Currency To",
                    "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                ToCurrencyCB.Focus();
                return;
            }

            if(FromCurrencyCB.Text == ToCurrencyCB.Text)
            {
                //The amount textbox value set in ConvertedValue.
                //double.parse is used to convert datatype String to Double
                //TextBox text have string and ConvertedValue is double datatype
                ConvertedValue = double.Parse(EnterAmountTB.Text);

                //Show in label converted currency and converted name
                //and ToString("N3") is used to place 000 after the(.)
                MyLabel.Content = ToCurrencyCB.Text + " "+ ConvertedValue.ToString("N3");
            }
            else
            {
                //Calculation for currency is From Currency value multiply by the amount in the textbox
                //value and then divided with To Currency value
                ConvertedValue = (double.Parse(ToCurrencyCB.SelectedValue.ToString())
                    * double.Parse(EnterAmountTB.Text))
                    / double.Parse(FromCurrencyCB.SelectedValue.ToString());

                //Show in label converted currency and converted currency name
                MyLabel.Content = ToCurrencyCB.Text + " " + ConvertedValue.ToString("N3");
            }
        }

        //Clear Button
        private void Clear_Button(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
    }
}
