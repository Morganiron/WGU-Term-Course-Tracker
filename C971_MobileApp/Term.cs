using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C971_MobileApp
{
    internal class Term
    {
        internal int ID { get; set; }
        internal string Title { get; set; }
        internal DateTime StartDate { get; set; }
        internal DateTime EndDate { get; set; }

        internal Term() 
        {
            Title = string.Empty;
            StartDate = DateTime.MinValue;
            EndDate = DateTime.MaxValue;
        }
    }
}
