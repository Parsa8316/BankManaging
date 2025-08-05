using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_Managing.Services
{
    public interface ISmsReader
    {
        List<string> GetBankMessages();
    }
}
