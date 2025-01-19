namespace C971_MobileApp.Models
{
    internal class NoteContent
    {
        public string Title { get; set; } = string.Empty; // Title of the note
        public string Content { get; set; } = string.Empty; // Main content of the note
        public DateTime CreatedDate { get; set; } // Timestamp when the note was created
    }
}
