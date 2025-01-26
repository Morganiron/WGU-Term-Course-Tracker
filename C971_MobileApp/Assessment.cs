using SQLite;

namespace C971_MobileApp
{
    internal class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Primary Key
        public int CourseID { get; set; } // Foreign Key
        public string Type { get; set; } = string.Empty; // "Performance" or "Objective"
        public string Title { get; set; } = string.Empty; // Defaults to either Performance or Objective
        public DateTime DueDate { get; set; } // Due date for the assessment
    }
}
