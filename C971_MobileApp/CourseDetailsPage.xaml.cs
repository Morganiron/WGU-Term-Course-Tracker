using System.Collections.ObjectModel;
using C971_MobileApp.Models;
using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971_MobileApp
{
    public partial class CourseDetailsPage : ContentPage
    {
        private readonly Course _course;

        private ObservableCollection<Note> _notes;

        private ObservableCollection<Assessment> _performanceAssessments;
        private ObservableCollection<Assessment> _objectiveAssessments;

        internal ObservableCollection<Assessment> PerformanceAssessments
        {
            get => _performanceAssessments;
            set
            {
                _performanceAssessments = value;
                OnPropertyChanged(nameof(PerformanceAssessments));
            }
        }

        internal ObservableCollection<Assessment> ObjectiveAssessments
        {
            get => _objectiveAssessments;
            set
            {
                _objectiveAssessments = value;
                OnPropertyChanged(nameof(ObjectiveAssessments));
            }
        }

        internal CourseDetailsPage(Course course)
        {
            InitializeComponent();
            _course = course;

            // Set course as the BindingContext for course-related bindings
            BindingContext = _course;

            // Initialize assessments
            PerformanceAssessments = new ObservableCollection<Assessment>();
            ObjectiveAssessments = new ObservableCollection<Assessment>();

            // Set ItemsSource for the CollectionViews
            PerformanceAssessmentsCollectionView.ItemsSource = PerformanceAssessments;
            ObjectiveAssessmentsCollectionView.ItemsSource = ObjectiveAssessments;

            // Initialize the notification toggle values
            StartDateNotificationToggle.IsToggled = _course.StartDateNotificationEnabled;
            EndDateNotificationToggle.IsToggled = _course.EndDateNotificationEnabled;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadNotesAsync();
        }

        private async Task LoadNotesAsync()
        {
            try
            {
                var notes = await DatabaseService.GetNotesByCourseIdAsync(_course.ID);
                Notes = new ObservableCollection<Note>(notes);
                NotesCollectionView.ItemsSource = Notes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading notes: {ex.Message}");
                await DisplayAlert("Error", "Failed to load notes.", "OK");
            }
        }

        internal ObservableCollection<Note> Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged(nameof(Notes));
            }
        }

        // Notification handler for course start date
        private async void OnStartDateNotificationToggled(object sender, ToggledEventArgs e)
        {
            bool isEnabled = e.Value;
            _course.StartDateNotificationEnabled = isEnabled;

            if (isEnabled)
            {
                // Adjust the StartDate to be set with a time of 8:00 AM
                DateTime notifyTime = _course.StartDate.Date.AddHours(8);
                

                // Schedule a notification for the course start date
                var notification = new NotificationRequest
                {
                    NotificationId = _course.ID * 10 + 1,
                    Title = "Course Start Reminder",
                    Description = $"Your course \"{_course.Title}\" starts on {_course.StartDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
            else
            {
                // Cancel the notification
                LocalNotificationCenter.Current.Cancel(_course.ID * 10 + 1);
            }

            // Save the updated state to the database
            await DatabaseService.Database.UpdateAsync(_course);
        }


        // Notification handler for course end date
        private async void OnEndDateNotificationToggled(object sender, ToggledEventArgs e)
        {
            bool isEnabled = e.Value;
            _course.EndDateNotificationEnabled = isEnabled;

            if (isEnabled)
            {
                // Adjust the EndDate to be set with a time of 8:00 AM
                DateTime notifyTime = _course.EndDate.Date.AddHours(8);

                // Schedule a notification for the course end date
                var notification = new NotificationRequest
                {
                    NotificationId = _course.ID * 10 + 2,
                    Title = "Course End Reminder",
                    Description = $"Your course \"{_course.Title}\" ends on {_course.EndDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = notifyTime
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
            else
            {
                // Cancel the notification
                LocalNotificationCenter.Current.Cancel(_course.ID * 10 + 2);
            }

            // Save the updated state to the database
            await DatabaseService.Database.UpdateAsync(_course);
        }


        private void OnPhoneTapped(object sender, EventArgs e)
        {
            if (sender is Label label && !string.IsNullOrEmpty(label.Text))
            {
                Launcher.OpenAsync(new Uri($"tel:{label.Text}"));
            }
        }


        private void OnEmailTapped(object sender, EventArgs e)
        {
            if (sender is Label label && !string.IsNullOrEmpty(label.Text))
            {
                Launcher.OpenAsync(new Uri($"mailto:{label.Text}"));
            }
        }


        private async void OnEditCourseClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CourseInputPage(_course.TermID, _course));
        }

        private async void OnDeleteCourseClicked(object sender, EventArgs e)
        {
            bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete {_course.Title}?", "Yes", "No");
            if (confirm)
            {
                await DatabaseService.Database.DeleteAsync(_course);
                await DisplayAlert("Success", "Course deleted successfully!", "OK");
                await Navigation.PopAsync(); // Go back to the previous page
            }
        }

        private async void OnAddNoteTapped(object sender, EventArgs e)
        {
            // Create a new note linked to the current course
            var newNote = Note.Create(_course.ID, string.Empty, string.Empty);

            // Navigate to the NoteEditorPage for the new note
            await Navigation.PushAsync(new NoteEditorPage(newNote, isNewNote: true));
        }

        private async void OnEditNoteTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Note noteToEdit)
            {
                // Navigate to the NoteEditorPage for the existing note
                await Navigation.PushAsync(new NoteEditorPage(noteToEdit, isNewNote: false));
            }
        }

        private async void OnDeleteNoteTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Note note)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the note \"{note.Content.Title}\"?", "Yes", "No");
                if (confirm)
                {
                    // Delete the note from the database
                    await DatabaseService.DeleteNoteAsync(note);

                    // Update the notes displayed on the page
                    var notes = await DatabaseService.GetNotesByCourseIdAsync(note.CourseId);
                    NotesCollectionView.ItemsSource = notes;

                    await DisplayAlert("Success", "Note deleted successfully!", "OK");
                }
            }
        }

        private async void OnShareNoteTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Note note)
            {
                // Serialize the note content for sharing
                var noteContent = note.Content;
                string shareText = $"Title: {noteContent.Title}\n\n{noteContent.Content}\n\nCreated: {noteContent.CreatedDate:MM/dd/yyyy}";

                // Use the Share API to share the note
                await Share.Default.RequestAsync(new ShareTextRequest
                {
                    Text = shareText,
                    Title = $"Share Note: {noteContent.Title}"
                });
            }
        }

        private void OnAddPerformanceAssessmentTapped(object sender, EventArgs e)
        {
            var newAssessment = new Assessment
            {
                Title = "Performance Assessment",
                Type = "Performance",
                CourseID = _course.ID,
                DueDate = DateTime.Now.AddDays(7) // Set a default due date
            };

            PerformanceAssessments.Add(newAssessment);

        }

        private void OnAddObjectiveAssessmentTapped(object sender, EventArgs e)
        {
            var newAssessment = new Assessment
            {
                Title = "Default Objective Assessment",
                Type = "Objective",
                CourseID = _course.ID,
                DueDate = DateTime.Now.AddDays(14) // Set a default due date
            };

            ObjectiveAssessments.Add(newAssessment);

        }
    }
}
