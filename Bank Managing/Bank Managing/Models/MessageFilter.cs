using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_Managing.Models
{
    public class MessageFilter
    {
        public int id { get; set; }
        public bool IsIncome { get; set; }
        public bool IsOutcome { get; set; }
        public bool IsNone { get; set; }
        public string[] Types { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public PersianDateTime FromTime { get; set; }
        public PersianDateTime ToTime { get; set; }
        public string Description { get; set; }
        public decimal MinBalance { get; set; }
        public decimal MaxBalance { get; set; }
        public string[] Banks { get; set; }
    }
}
