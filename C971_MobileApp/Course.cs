using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971_MobileApp
{
    internal class Course
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public int TermID { get; set; } // Foreign key to Term
        public string Title { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        public Course()
        {

        }
    }
}
