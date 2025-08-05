using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Essentials;
using Android.Database;
using Android.Net;
using Bank_Managing.Services;
using Bank_Managing.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(SmsReader))]
namespace Bank_Managing.Droid
{
    public class SmsReader : ISmsReader
    {
        public List<string> GetBankMessages()
        {
            var messages = new List<string>();
            var context = Android.App.Application.Context;

            // خواندن لیست ذخیره‌شده از بانک‌ها
            var senderRaw = Preferences.Get("bank_senders", "");
            if (string.IsNullOrWhiteSpace(senderRaw))
                return messages;

            // جداسازی فرستنده‌ها
            var senders = senderRaw.Split(',')
                                   .Select(s => s.Trim())
                                   .Where(s => !string.IsNullOrEmpty(s))
                                   .ToList();

            // خواندن پیامک‌ها از SMS inbox
            Android.Net.Uri uriSms = Android.Net.Uri.Parse("content://sms/inbox");
            ICursor cursor = context.ContentResolver.Query(uriSms, null, null, null, "date DESC");

            if (cursor != null && cursor.MoveToFirst())
            {
                int addressIndex = cursor.GetColumnIndex("address");
                int bodyIndex = cursor.GetColumnIndex("body");

                do
                {
                    string address = cursor.GetString(addressIndex);

                    // بررسی اینکه فرستنده شامل یکی از بانک‌ها باشه
                    if (senders.Any(sender => address.Contains(sender)))
                    {
                        string body = cursor.GetString(bodyIndex);
                        messages.Add(body);
                    }

                } while (cursor.MoveToNext());

                cursor.Close();
            }

            return messages;
        }
    }
}