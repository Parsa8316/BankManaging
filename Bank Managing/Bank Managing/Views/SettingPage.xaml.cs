using Android.Net.Wifi.Aware;
using Bank_Managing.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static Android.Renderscripts.ScriptGroup;

namespace Bank_Managing.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        ObservableCollection<string> BankSenders { get; set; }
        public SettingPage()
        {
            InitializeComponent();

            RefreshPage();
        }

        public void RefreshPage()
        {
            BankSenders = new ObservableCollection<string>();
            var saved = Preferences.Get("bank_senders", "");
            //string[] banks = saved.Split(',')
            //                            .Select(b => b.Trim())
            //                            .Where(b => !string.IsNullOrWhiteSpace(b))
            //                            .ToArray();

            //for (int i = 0; i < banks.Length; i++)
            //{
            //    BankSenders.Add(banks[i]);
            //}
            foreach (var i in saved.Split(',').Select(b => b.Trim()).Where(b => !string.IsNullOrWhiteSpace(b)))
            {
                BankSenders.Add(i);
            }
            MainListView.ItemsSource = BankSenders;
            //if (banks.Length > 0)
            //{
            //    SenderEntry.Text = banks[banks.Length - 1];
            //}
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            try
            {
                var input = SenderEntry.Text?.Trim();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    if (BankSenders.ToList().Any(i => i == input))
                    {
                        DisplayAlert("خطا", "این بانک قبلا اضافه شده است.", "باشه");
                        return;
                    }
                    input = input.Replace(" ", "");
                    if (input.StartsWith("0"))
                    {
                        input = "+98" + input.Substring(1);
                    }
                    BankSenders.Add(input);

                    var serialized = "";
                    foreach (string i in BankSenders)
                    {
                        serialized += i + ",";
                    }

                    Preferences.Set("bank_senders", serialized);
                    DisplayAlert("ذخیره شد", "لیست بانک‌ها ذخیره شد.", "باشه");
                    SenderEntry.Text = "";
                }
                else
                {
                    DisplayAlert("خطا", "نام بانک را وارد نکرده اید!", "باشه");
                }
            }
            catch
            {
                DisplayAlert("خطا", "خطایی در ذخیره فرستنده رخ داد.", "باشه");
            }
        }

        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            string bank = ((Button)sender)?.BindingContext?.ToString();
            if (bank != null)
            {
                if (await DisplayAlert("مطمعنی؟", $"آیا از حذف بانک {bank} مطمعن هستید ؟", "بله", "خیر"))
                {
                    BankSenders.Remove(bank);
                    //var serialized = string.Join(",", items);
                    var serialized = "";
                    foreach (string i in BankSenders)
                    {
                        serialized += i + ",";
                    }

                    Preferences.Set("bank_senders", serialized);
                    DependencyService.Get<IToast>().MakeToast($"بانک {bank} با موفقیت حذف شد");
                }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync(nameof(MainPage));
        }
    }
}