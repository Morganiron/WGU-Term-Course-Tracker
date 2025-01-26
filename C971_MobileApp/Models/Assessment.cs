using SQLite;

namespace C971_MobileApp
{
    internal class Assessment
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Primary Key

        public int CourseID { get; set; } // Foreign Key

        public string Type { get; set; } = string.Empty; // "Performance" or "Objective"

        public string Title { get; set; } = string.Empty;

        public DateTime StartDate { get; set; } = DateTime.Now; // Start date for the assessment

        public DateTime EndDate { get; set; } = DateTime.Now; // End date for the assessment

        public bool StartDateNotificationEnabled { get; set; } = false; // Start date notification

        public bool EndDateNotificationEnabled { get; set; } = false; // End date notification
    }
}
