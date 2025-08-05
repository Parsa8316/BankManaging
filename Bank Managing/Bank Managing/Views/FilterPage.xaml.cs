using Bank_Managing.Models;
using Bank_Managing.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Java.Util.Jar.Attributes;
using static System.Net.Mime.MediaTypeNames;

namespace Bank_Managing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FilterPage : ContentPage
    {
        public List<string> banks { get; set; }

        public FilterPage()
        {
            InitializeComponent();

            TxtFromDate.Text = "1404/" + (PersianDateTime.Now.Month < 10 ? "0" + PersianDateTime.Now.Month : PersianDateTime.Now.Month.ToString()) + "/01";
            TxtToDate.Text = "1500/01/01";
            if (SendFilter.AllBanks == null && SendFilter.AllBanks.Count < 1)
            {
                SendFilter.AllBanks = new List<string>();
            }
            banks = SendFilter.AllBanks;
            //var saved = Preferences.Get("bank_senders", "");
            //foreach (var i in saved.Split(',').Select(b => b.Trim()).Where(b => !string.IsNullOrWhiteSpace(b)))
            //{
            //    banks.Add(i);
            //}
            //TxtBanks.ItemsSource = banks;
            foreach (string i in banks)
            {
                StackLayout stack = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    Padding = new Thickness(0, 0, 15, 5)
                };

                CheckBox checkBox = new CheckBox()
                {
                    VerticalOptions = LayoutOptions.Center,
                    StyleId = i,
                    IsChecked = true
                };
                Label label = new Label()
                {
                    Text = i,
                    VerticalOptions = LayoutOptions.Center,
                    LineBreakMode = LineBreakMode.NoWrap,
                    FontSize = 16,
                    TextColor = Color.Black
                };

                stack.Children.Add(label);
                stack.Children.Add(checkBox);
                BanksLayout.Children.Add(stack);
            }
            if (SendFilter.Filter != null)
            {
                var filter = SendFilter.Filter;
                RdIncome.IsChecked = filter.IsIncome;
                RdOutcome.IsChecked = filter.IsOutcome;
                RdNone.IsChecked = filter.IsNone;
                if (filter.Types != null && filter.Types.Length > 0)
                {
                    RdOnBox.IsChecked = filter.Types.Any(i => i.Contains("آن باکس"));
                    RdMoneyIn.IsChecked = filter.Types.Any(i => i.Contains("واریز پول"));
                    RdProfit.IsChecked = filter.Types.Any(i => i.Contains("سود شما"));
                    RdMoneyOut.IsChecked = filter.Types.Any(i => i.Contains("برداشت پول"));
                    RdInBox.IsChecked = filter.Types.Any(i => i.Contains("این باکس"));
                    RdPayBill.IsChecked = filter.Types.Any(i => i.Contains("پرداخت قبض"));
                    RdSimCharged.IsChecked = filter.Types.Any(i => i.Contains("شارژ شدی"));
                    RdNetBought.IsChecked = filter.Types.Any(i => i.Contains("آنلاین شدی"));
                }
                TxtMinPrice.Text = filter.MinPrice == 0 ? null : filter.MinPrice.ToString();
                TxtMaxPrice.Text = filter.MaxPrice == 0 ? null : filter.MaxPrice.ToString();
                TxtMinBalance.Text = filter.MinBalance == 0 ? null : filter.MinBalance.ToString();
                TxtMaxBalance.Text = filter.MaxBalance == 0 ? null : filter.MaxBalance.ToString();
                TxtFromDate.Text = filter.FromTime == null ? null : filter.FromTime.ToString();
                TxtToDate.Text = filter.ToTime == null ? null : filter.ToTime.ToString();
                TxtDescription.Text = filter.Description;
                //if (filter.Banks != null)
                //{

                //}
            }
        }


        private async void Button_Clicked(object sender, EventArgs e)
        {
            MessageFilter filter = new MessageFilter();

            filter.IsIncome = RdIncome.IsChecked;
            filter.IsOutcome = RdOutcome.IsChecked;
            filter.IsNone = RdNone.IsChecked;

            string[] types = new string[8];
            types[0] = RdOnBox.IsChecked ? "آن باکس" : null;
            types[1] = RdMoneyIn.IsChecked ? "واریز پول" : null;
            types[2] = RdProfit.IsChecked ? "سود شما" : null;
            types[3] = RdMoneyOut.IsChecked ? "برداشت پول" : null;
            types[4] = RdInBox.IsChecked ? "این باکس" : null;
            types[5] = RdPayBill.IsChecked ? "پرداخت قبض" : null;
            types[6] = RdSimCharged.IsChecked ? "شارژ شدی" : null;
            types[7] = RdNetBought.IsChecked ? "آنلاین شدی!" : null;
            types = types.Where(i => !string.IsNullOrWhiteSpace(i)).ToArray();
            filter.Types = types;

            var selectedBanks = BanksLayout.Children
                .OfType<StackLayout>()
                .SelectMany(sl => sl.Children.OfType<CheckBox>())
                .Where(cb => cb.IsChecked)
                .Select(cb => cb.StyleId)
                .ToList();
            string[] bank = selectedBanks.ToArray();
            filter.Banks = bank;
            string[] fromDate = TxtFromDate.Text.Split('/');
            fromDate[2] = fromDate[2].Split(' ')[0].Trim();
            for (int i = 0; i < fromDate.Length; i++)
            {
                if (Convert.ToInt32(fromDate[i]) < 10)
                {
                    fromDate[i] = "0" + Convert.ToInt32(fromDate[i]);
                }
            }
            string[] toDate = TxtToDate.Text.Split('/');
            toDate[2] = toDate[2].Split(' ')[0].Trim();
            for (int i = 0; i < toDate.Length; i++)
            {
                if (Convert.ToInt32(toDate[i]) < 10)
                {
                    toDate[i] = "0" + Convert.ToInt32(toDate[i]);
                }
            }
            bool back = false;
            try
            {
                filter.MinPrice = Convert.ToDecimal(TxtMinPrice.Text);
                filter.MaxPrice = Convert.ToDecimal(TxtMaxPrice.Text);
                filter.MinBalance = Convert.ToDecimal(TxtMinBalance.Text);
                filter.MaxBalance = Convert.ToDecimal(TxtMaxBalance.Text);
                filter.Description = TxtDescription.Text;
                filter.FromTime = PersianDateTime.Parse($"{fromDate[0]}/{fromDate[1]}/{fromDate[2]}");
                filter.ToTime = PersianDateTime.Parse($"{toDate[0]}/{toDate[1]}/{toDate[2]}");
                SendFilter.Filter = filter;
                back = true;
                await Shell.Current.GoToAsync($"{nameof(MainPage)}");
            }
            catch
            {
                await DisplayAlert("خطا", "لطفا در وارد کردن ورودی ها دقت کنید", "باشه");
            }

            //var filterJson = JsonConvert.SerializeObject(filter);
            //await Shell.Current.GoToAsync($"{nameof(MainPage)}?messageFilter={Uri.EscapeDataString(filterJson)}");
        }

        private void RdIncome_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RdOnBox.IsEnabled = !RdOnBox.IsEnabled;
            RdMoneyIn.IsEnabled = !RdMoneyIn.IsEnabled;
            RdProfit.IsEnabled = !RdProfit.IsEnabled;
            RdOnBox.IsChecked = true;
            RdMoneyIn.IsChecked = true;
            RdProfit.IsChecked = true;
            if (RdIncome.IsChecked)
            {
                TxtOnBox.TextColor = Color.DarkGray;
                TxtMoneyIn.TextColor = Color.DarkGray;
                TxtProfit.TextColor = Color.DarkGray;
            }
            else
            {
                TxtOnBox.TextColor = Color.Black;
                TxtMoneyIn.TextColor = Color.Black;
                TxtProfit.TextColor = Color.Black;
            }
            //RdMoneyOut.IsEnabled = !RdMoneyOut.IsEnabled;
            //RdInBox.IsEnabled = !RdInBox.IsEnabled;
            //RdPayBill.IsEnabled = !RdPayBill.IsEnabled;
            //RdSimCharged.IsEnabled = !RdSimCharged.IsEnabled;
            //RdNetBought.IsEnabled = !RdNetBought.IsEnabled;
            //RdMoneyOut.IsChecked = false;
            //RdInBox.IsChecked = false;
            //RdPayBill.IsChecked = false;
            //RdSimCharged.IsChecked = false;
            //RdNetBought.IsChecked = false;
            //if (RdIncome.IsChecked)
            //{
            //    TxtMoneyOut.TextColor = Color.DarkGray;
            //    TxtInBox.TextColor = Color.DarkGray;
            //    TxtPayBill.TextColor = Color.DarkGray;
            //    TxtSimCharged.TextColor = Color.DarkGray;
            //    TxtNetBought.TextColor = Color.DarkGray;
            //}
            //else
            //{
            //    TxtMoneyOut.TextColor = Color.Black;
            //    TxtInBox.TextColor = Color.Black;
            //    TxtPayBill.TextColor = Color.Black;
            //    TxtSimCharged.TextColor = Color.Black;
            //    TxtNetBought.TextColor = Color.Black;
            //}
        }

        private void RdOutcome_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            RdMoneyOut.IsEnabled = !RdMoneyOut.IsEnabled;
            RdInBox.IsEnabled = !RdInBox.IsEnabled;
            RdPayBill.IsEnabled = !RdPayBill.IsEnabled;
            RdSimCharged.IsEnabled = !RdSimCharged.IsEnabled;
            RdNetBought.IsEnabled = !RdNetBought.IsEnabled;
            RdMoneyOut.IsChecked = true;
            RdInBox.IsChecked = true;
            RdPayBill.IsChecked = true;
            RdSimCharged.IsChecked = true;
            RdNetBought.IsChecked = true;
            if (RdOutcome.IsChecked)
            {
                TxtMoneyOut.TextColor = Color.DarkGray;
                TxtInBox.TextColor = Color.DarkGray;
                TxtPayBill.TextColor = Color.DarkGray;
                TxtSimCharged.TextColor = Color.DarkGray;
                TxtNetBought.TextColor = Color.DarkGray;
            }
            else
            {
                TxtMoneyOut.TextColor = Color.Black;
                TxtInBox.TextColor = Color.Black;
                TxtPayBill.TextColor = Color.Black;
                TxtSimCharged.TextColor = Color.Black;
                TxtNetBought.TextColor = Color.Black;
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync($"{nameof(MainPage)}");
        }

        private void RdNone_CheckedChanged(object sender, CheckedChangedEventArgs e)
        {

        }
    }
}