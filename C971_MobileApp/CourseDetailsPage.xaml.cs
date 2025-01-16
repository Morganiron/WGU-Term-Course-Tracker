using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971_MobileApp
{
    public partial class CourseDetailsPage : ContentPage
    {
        private readonly Course _course;

        public bool StartDateNotificationEnabled
        {
            get => _course.StartDateNotificationEnabled;
            set
            {
                if (_course.StartDateNotificationEnabled != value)
                {
                    _course.StartDateNotificationEnabled = value;
                    HandleStartDateNotification(value);
                    OnPropertyChanged(nameof(StartDateNotificationEnabled));
                }
            }
        }

        public bool EndDateNotificationEnabled
        {
            get => _course.EndDateNotificationEnabled;
            set
            {
                if (_course.EndDateNotificationEnabled != value)
                {
                    _course.EndDateNotificationEnabled = value;
                    HandleEndDateNotification(value);
                    OnPropertyChanged(nameof(EndDateNotificationEnabled));
                }
            }
        }

        internal CourseDetailsPage(Course course)
        {
            InitializeComponent();
            _course = course;
            BindingContext = this;

            // Initialize the notification toggle values
            StartDateNotificationEnabled = _course.StartDateNotificationEnabled;
            EndDateNotificationEnabled = _course.EndDateNotificationEnabled;
        }

        // Notification handler for course start date
        private async void HandleStartDateNotification(bool isEnabled)
        {
            _course.StartDateNotificationEnabled = isEnabled;

            if (isEnabled)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = _course.ID * 10 + 1, // Unique ID
                    Title = "Course Start Reminder",
                    Description = $"Your course \"{_course.Title}\" starts on {_course.StartDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = _course.StartDate
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
            else
            {
                LocalNotificationCenter.Current.Cancel(_course.ID * 10 + 1);
            }

            // Update the database
            await DatabaseService.Database.UpdateAsync(_course);
        }


        // Notification handler for course end date
        private async void HandleEndDateNotification(bool isEnabled)
        {
            _course.EndDateNotificationEnabled = isEnabled;

            if (isEnabled)
            {
                var notification = new NotificationRequest
                {
                    NotificationId = _course.ID * 10 + 2, // Unique ID
                    Title = "Course End Reminder",
                    Description = $"Your course \"{_course.Title}\" ends on {_course.EndDate:MM/dd/yyyy}.",
                    Schedule = new NotificationRequestSchedule
                    {
                        NotifyTime = _course.EndDate
                    }
                };
                await LocalNotificationCenter.Current.Show(notification);
            }
            else
            {
                LocalNotificationCenter.Current.Cancel(_course.ID * 10 + 2);
            }

            // Update the database
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
    }
}
