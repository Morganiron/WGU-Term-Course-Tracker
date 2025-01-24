using C971_MobileApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace C971_MobileApp
{
    public partial class NoteEditorPage : ContentPage, INotifyPropertyChanged
    {
        private readonly Note _note;
        private readonly bool _isNewNote;

        private string _noteTitle = string.Empty;
        private string _noteContent = string.Empty;
        private DateTime _createdDate = DateTime.Now;
        private DateTime _lastUpdated = DateTime.Now;
        private bool _showDates = true;

        public event PropertyChangedEventHandler PropertyChanged;

        public string NoteTitle
        {
            get => _noteTitle;
            set => SetProperty(ref _noteTitle, value);
        }

        public string NoteContent
        {
            get => _noteContent;
            set => SetProperty(ref _noteContent, value);
        }

        public DateTime CreatedDate
        {
            get => _createdDate;
            set => SetProperty(ref _createdDate, value);
        }

        public DateTime LastUpdated
        {
            get => _lastUpdated;
            set => SetProperty(ref _lastUpdated, value);
        }

        public bool ShowDates
        {
            get => _showDates;
            set => SetProperty(ref _showDates, value);
        }

        internal NoteEditorPage(Note note, bool isNewNote)
        {
            InitializeComponent();

            _note = note;
            _isNewNote = isNewNote;

            // Initialize the local variables
            NoteTitle = _note.Content.Title;
            NoteContent = _note.Content.Content;
            CreatedDate = _note.Content.CreatedDate;
            LastUpdated = _note.Content.LastUpdated;

            // Show or hide dates based on new note status
            ShowDates = !_isNewNote;

            // Set BindingContext
            BindingContext = this;
        }

        private async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NoteTitle))
            {
                await DisplayAlert("Error", "Please enter a valid title.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(NoteContent))
            {
                await DisplayAlert("Error", "Please enter valid content.", "OK");
                return;
            }

            // Update the note content
            _note.Content = new NoteContent
            {
                Title = NoteTitle,
                Content = NoteContent,
                CreatedDate = _isNewNote ? DateTime.Now : _note.Content.CreatedDate,
                LastUpdated = DateTime.Now
            };

            // Save or update the note in the database
            if (_isNewNote)
            {
                await DatabaseService.AddNoteAsync(_note);
            }
            else
            {
                await DatabaseService.UpdateNoteAsync(_note);
            }

            await Navigation.PopAsync();
        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Cancel", "Are you sure you want to discard changes?", "Yes", "No"))
            {
                await Navigation.PopAsync();
            }
        }

        private void SetProperty<T>(ref T backingField, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(backingField, value))
            {
                backingField = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
