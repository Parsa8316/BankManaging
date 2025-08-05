using Bank_Managing.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Internals;
using static Android.Renderscripts.ScriptGroup;

namespace Bank_Managing.Models
{
    public class BankData
    {
        public int id { get; set; }
        public bool IsIncome { get; set; }
        public bool IsOutcome { get; set; }
        public string Type { get; set; }
        public decimal Price { get; set; }
        public PersianDateTime Time { get; set; }
        public string Description { get; set; }
        public decimal Balance { get; set; }
        public string Bank { get; set; }

        public BankData(int id, bool isIncome, string type, decimal price, PersianDateTime time, string description, decimal balance, string bank)
        {
            this.id = id;
            IsIncome = isIncome;
            Bank = bank;
            Type = type;
            Price = price;
            Balance = balance;
            Time = time;
            Description = description;
        }
        public BankData() { }
        public BankData(string message, int id)
        {
            string[] rows = message.Split('\n').Where(row => !string.IsNullOrWhiteSpace(row)).ToArray();
            bool isIncome = rows[1].Contains("آن باکس") || rows[1].Contains("واریز پول") || rows[1].Contains("سود شما");
            bool isOutcome = rows[1].Contains("برداشت پول") || rows[1].Contains("این باکس") || rows[1].Contains("پرداخت قبض") ||
                rows[1].Contains("شارژ شدی") || rows[1].Contains("آنلاین شدی");

            this.id = id;
            IsIncome = isIncome;
            IsOutcome = isOutcome;
            Bank = rows[0].Trim();
            if (!SendFilter.AllBanks.Any(i => i == Bank) && Bank.Length < 16)
            {
                if (SendFilter.AllBanks == null && SendFilter.AllBanks.Count < 1)
                {
                    SendFilter.AllBanks = new List<string>();
                }
                SendFilter.AllBanks.Add(Bank);
            }
            Type = rows[1];
            Description = message;
            try
            {
                if ((isIncome && !isOutcome) || (!isIncome && isOutcome))
                {
                    string[] r2 = rows[2].Split(' ');
                    string x = r2[r2.IndexOf("ریال") - 1].Replace(",", "");
                    Price = Convert.ToDecimal(x) / 10;
                    try
                    {
                        Balance = Convert.ToDecimal(rows[3].Replace("موجودی", "").Replace("ریال", "").Replace(",", "").Replace(":", "").Replace("حساب", "").Trim()) / 10;
                    }
                    catch
                    {
                        Balance = 0;
                    }

                    string[] persianDigits = { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };
                    int l = rows.Length;
                    for (int i = 0; i < 10; i++)
                    {
                        rows[l - 2] = rows[l - 2].Replace(persianDigits[i], i.ToString());
                        rows[l - 1] = rows[l - 1].Replace(persianDigits[i], i.ToString());
                    }
                    try
                    {
                        int[] date = rows[l - 1].Split('.').Select(i => Convert.ToInt32(i)).ToArray();
                        int[] time = rows[l - 2].Split(':').Select(i => Convert.ToInt32(i)).ToArray();
                        Time = new PersianDateTime(date[0], date[1], date[2], time[0], time[1], 0);
                    }
                    catch
                    {
                        Time = new PersianDateTime(1371, 1, 1, 0, 0, 0);
                    }
                }
                else if (!isIncome && !isOutcome)
                {
                    if (Type.Contains("بفرمایید رمز پویا"))
                    {
                        Price = Convert.ToDecimal(rows[4].Replace("مبلغ", "").Replace("ریال", "").Replace(",", "").Replace(":", "").Trim()) / 10;
                        int l = rows.Length;
                        try
                        {
                            int[] time = rows[l - 1].Split(':').Select(i => Convert.ToInt32(i)).ToArray();

                            Time = new PersianDateTime(1, 1, 1, time[0], time[1], 0);
                        }
                        catch
                        {
                            Time = new PersianDateTime(1, 1, 1, 0, 0, 0);
                        }
                    }
                    else
                    {
                        Price = 0;
                        Time = new PersianDateTime(1, 1, 1, 0, 0, 0);
                    }
                }
                else
                {
                    string x = message;
                }
            }
            catch (Exception e)
            {
                Description += "\n" + e.Message;
            }
        }
    }
}
