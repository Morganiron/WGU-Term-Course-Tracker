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
            await LoadAssessmentsAsync();
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

        private async Task LoadAssessmentsAsync()
        {
            try
            {
                var assessments = await DatabaseService.Database.Table<Assessment>().Where(a => a.CourseID == _course.ID).ToListAsync();

                // Separate assessments into performance and objective types
                PerformanceAssessments = new ObservableCollection<Assessment>(assessments.Where(a => a.Type == "Performance"));
                ObjectiveAssessments = new ObservableCollection<Assessment>(assessments.Where(a => a.Type == "Objective"));

                // Ensure the CollectionViews are updated
                PerformanceAssessmentsCollectionView.ItemsSource = PerformanceAssessments;
                ObjectiveAssessmentsCollectionView.ItemsSource = ObjectiveAssessments;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assessments: {ex.Message}");
                await DisplayAlert("Error", "Failed to load assessments.", "OK");
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

        private async void OnAddPerformanceAssessmentTapped(object sender, EventArgs e)
        {
            var newAssessment = new Assessment
            {
                Type = "Performance",
                CourseID = _course.ID,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(7)
            };
            await Navigation.PushAsync(new AssessmentInputPage(newAssessment));
        }

        private async void OnAddObjectiveAssessmentTapped(object sender, EventArgs e)
        {
            var newAssessment = new Assessment
            {
                Type = "Objective",
                CourseID = _course.ID,
                StartDate = DateTime.Now.Date,
                EndDate = DateTime.Now.Date.AddDays(7)
            };
            await Navigation.PushAsync(new AssessmentInputPage(newAssessment));
        }


        private async void OnEditAssessmentTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Assessment assessmentToEdit)
            {
                // Navigate to the AssessmentInputPage with the selected assessment
                await Navigation.PushAsync(new AssessmentInputPage(assessmentToEdit));
            }
        }

        private async void OnDeleteAssessmentTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Assessment assessmentToDelete)
            {
                bool confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the assessment \"{assessmentToDelete.Title}\"?", "Yes", "No");
                if (confirm)
                {
                    // Delete the assessment from the database
                    await DatabaseService.DeleteAssessmentAsync(assessmentToDelete);

                    // Update the CollectionView
                    if (assessmentToDelete.Type == "Performance")
                    {
                        PerformanceAssessments.Remove(assessmentToDelete);
                    }
                    else if (assessmentToDelete.Type == "Objective")
                    {
                        ObjectiveAssessments.Remove(assessmentToDelete);
                    }

                    await DisplayAlert("Success", "Assessment deleted successfully!", "OK");
                }
            }
        }

    }
}
