using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace C971_MobileApp
{
    internal class Term
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; } = DateTime.MinValue;
        public DateTime EndDate { get; set; } = DateTime.MinValue;

        // List to hold associated courses
        [Ignore] // Prevents this property from being stored in the database
        public List<Course> Courses { get; set; } = new List<Course>();

        // Property to handle expanded/collapsed state
        [Ignore]
        public bool IsExpanded { get; set; } = false;

        public Term() { }
    }
}
