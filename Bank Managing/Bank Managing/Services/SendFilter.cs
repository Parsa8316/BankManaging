using Bank_Managing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_Managing.Services
{
    public static class SendFilter
    {
        public static MessageFilter Filter { get; set; }
        public static List<string> AllBanks { get; set; }
    }
}
