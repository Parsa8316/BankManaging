using Android.Content;
using Android.Widget;
using Bank_Managing.Models;
using Bank_Managing.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Xaml;

namespace Bank_Managing.Views
{
    //[QueryProperty(nameof(messageFilter), nameof(messageFilter))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public MessageFilter messageFilter { get; set; }
        ObservableCollection<BankData> Messages { get; set; }

        public MainPage()
        {
            InitializeComponent();

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (SendFilter.Filter != null)
            {
                messageFilter = SendFilter.Filter;
            }
            SendFilter.AllBanks = new List<string>();

            var smsReader = DependencyService.Get<ISmsReader>();
            Messages = new ObservableCollection<BankData>();
            List<string> messages = smsReader?.GetBankMessages();
            int id = messages.Count;

            if (messages != null && messages.Any())
            {
                decimal sumIncome = 0;
                decimal sumOutcome = 0;
                foreach (string message in messages)
                {
                    Messages.Add(new BankData(message, id--));
                }
                //foreach (var msg in messages)
                //{
                //    Console.WriteLine(msg); // یا نمایش در ListView
                //}
                Messages = FilterBank(Messages);
                MainListView.ItemsSource = Messages;
                foreach (BankData i in Messages)
                {
                    if (i.IsIncome && !i.IsOutcome)
                    {
                        sumIncome += i.Price;
                    }
                    else if (i.IsOutcome && !i.IsIncome)
                    {
                        sumOutcome += i.Price;
                    }
                }
                LblIncome.Text = sumIncome.ToString();
                LblOutcome.Text = sumOutcome.ToString();
                LblBalance.Text = (sumIncome - sumOutcome).ToString();
                //MainListView.ItemTemplate.
            }
            else
            {
                //DisplayAlert("هشدار", "هیچ پیامک بانکی پیدا نشد", "باشه");
                DependencyService.Get<IToast>().MakeToast("هیچ پیامک بانکی پیدا نشد");
            }
        }

        public ObservableCollection<BankData> FilterBank(ObservableCollection<BankData> bd)
        {
            MessageFilter filter = SendFilter.Filter;
            List<List<BankData>> bList = new List<List<BankData>>();
            List<BankData> b = bd.ToList();
            List<BankData> a = bd.ToList();
            if (filter != null)
            {
                //if (filter.IsIncome)
                //{
                //    bList.Add(b.Where(i => i.IsIncome).ToList());
                //}
                //if (filter.IsOutcome)
                //{
                //    bList.Add(b.Where(i => i.IsOutcome).ToList());
                //}
                if (filter.IsNone)
                {
                    bList.Add(b.Where(i => !i.IsIncome && !i.IsOutcome).ToList());
                }
                //if (bList.Count > 0)
                //{
                //    b = new List<BankData>();

                //}
                if (filter.Types != null && filter.Types.Length > 0)
                {
                    b = b.Where(i => filter.Types.Contains(i.Type)).ToList();
                }
                if (bList.Count > 0)
                {
                    for (int i = 0; i < bList.Count; i++)
                    {
                        for (int j = 0; j < bList[i].Count; j++)
                        {
                            if (!b.Any(k => k == bList[i][j]))
                            {
                                b.Add(bList[i][j]);
                            }
                        }
                    }
                }
                //b = b.OrderBy(i => i.Time).ToList();
                //b = b.OrderByDescending(x => x.Time?.ToDateTime() ?? new DateTime(2, 1, 1)).ToList();
                b = b.OrderByDescending(i => i.id).ToList();

                //foreach (BankData i in a)
                //{
                //    if (b.Any(x => x != i))
                //    {
                //        b.Add(i);
                //    }
                //}
                if (filter.MinPrice > 0)
                {
                    b = b.Where(i => i.Price >= filter.MinPrice).ToList();
                }
                if (filter.MaxPrice > filter.MinPrice)
                {
                    b = b.Where(i => i.Price <= filter.MaxPrice).ToList();
                }
                if (filter.MinBalance > 0)
                {
                    b = b.Where(i => i.Balance >= filter.MinBalance).ToList();
                }
                if (filter.MaxBalance > filter.MinBalance)
                {
                    b = b.Where(i => i.Balance <= filter.MaxBalance).ToList();
                }
                if (!string.IsNullOrEmpty(filter.Description))
                {
                    b = b.Where(i => i.Description.Contains(filter.Description)).ToList();
                }
                if (filter.FromTime != null)
                {
                    //b = b.Where(i =>
                    //    i.Time != null &&
                    //    (
                    //        i.Time.Year <= 1300 ||
                    //        i.Time >= filter.FromTime
                    //    )
                    //).ToList();
                    int id = 0;
                    BankData _ = b.OrderBy(i => i.id).FirstOrDefault(i => i.Time != null && i.Time >= filter.FromTime);
                    if (b != null)
                    {
                        id = _.id;
                    }
                    b = b.Where(i => i.id >= id).ToList();
                }
                if (filter.ToTime != null)
                {
                    b = b.Where(i => i.Time != null && i.Time <= filter.ToTime).ToList();
                }
                if (filter.Banks != null && filter.Banks.Length > 0)
                {
                    b = b.Where(i => filter.Banks.Contains(i.Bank)).ToList();
                }
                bd = new ObservableCollection<BankData>();
                foreach (BankData i in b)
                {
                    bd.Add(i);
                }

            }
            return bd;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(SettingPage));
        }

        private void MainListView_ItemTapped(object sender, SelectedItemChangedEventArgs e)
        {
            BankData bd = (BankData)MainListView.SelectedItem;
            DisplayAlert(" متن" + bd?.id, bd?.Description, "باشه");
        }

        private void MainListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {

        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(FilterPage));
        }
    }
}