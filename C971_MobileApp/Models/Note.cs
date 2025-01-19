using SQLite;

namespace C971_MobileApp.Models
{
    internal class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Unique ID for each note
        public int CourseId { get; set; } // Link to the associated course
        public required string JSONContent { get; set; } // Serialized JSON of the note content

    }
}
