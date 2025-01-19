using Microsoft.Maui.Controls;
using Plugin.LocalNotification;

namespace C971_MobileApp
{
    public partial class CourseDetailsPage : ContentPage
    {
        private readonly Course _course;

        internal CourseDetailsPage(Course course)
        {
            InitializeComponent();
            _course = course;
            BindingContext = _course;

            // Initialize the notification toggle values
            StartDateNotificationToggle.IsToggled = _course.StartDateNotificationEnabled;
            EndDateNotificationToggle.IsToggled = _course.EndDateNotificationEnabled;
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
                Console.WriteLine($"Scheduled startDate notification date and time{notifyTime}");
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
                Console.WriteLine($"Scheduled endDate notification date and time{_course.EndDate}");
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

        private void OnAddNoteTapped(object sender, EventArgs e)
        {

        }
    }
}
