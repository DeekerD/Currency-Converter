using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurrencyConverter_Static
{
    internal class Root // This is the Main Class.API returns rates and it returns all currency name with value
    {
        public Rates rates { get; set; }
        public long timestamp;
        public string license;
    }
}
