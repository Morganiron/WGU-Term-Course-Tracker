using System.Text.Json;
using SQLite;

namespace C971_MobileApp.Models
{
    internal class Note
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; } // Unique ID for each note
        public int CourseId { get; set; } // Link to the associated course
        public string SerializedNote { get; set; } = "{}"; // Serialized JSON of the note content

        // Gets or sets note content by serializing/deserializing the JSON string
        public NoteContent Content
        {
            get => JsonSerializer.Deserialize<NoteContent>(SerializedNote) ?? new NoteContent();
            set => SerializedNote = JsonSerializer.Serialize(value);
        }

        // Create a new Note instance
        public static Note Create(int courseId, string title, string content)
        {
            var noteContent = new NoteContent
            {
                Title = title,
                Content = content,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            return new Note
            {
                CourseId = courseId,
                Content = noteContent // Automatically serializes into SerializeNote
            };
        }

        // Update the content of the note
        public void UpdateContent(string title, string content)
        {
            var noteContent = Content;
            noteContent.Title = title;
            noteContent.Content = content;
            noteContent.LastUpdated = DateTime.Now;
            Content = noteContent; // Serialize back to JSON
        }

    }
}
