using C971_MobileApp.Models;
using Plugin.LocalNotification;

namespace C971_MobileApp
{
    public partial class AssessmentInputPage : ContentPage
    {
        private readonly Assessment _assessment;
        private readonly bool _isEditing;

        internal AssessmentInputPage(Assessment assessment)
        {
            InitializeComponent();

            // Initialize the assessment object
            _assessment = assessment;

            // Check if the assessment is new or being edited
            _isEditing = _assessment.ID != 0;

            // Populate fields based on the provided assessment object
            PageTitleLabel.Text = $"{_assessment.Type} Assessment";
            AssessmentTitleEntry.Text = _assessment.Title;

            // Set default dates if not set
            StartDatePicker.Date = _assessment.StartDate == DateTime.MinValue ? DateTime.Now.Date : _assessment.StartDate;
            EndDatePicker.Date = _assessment.EndDate == DateTime.MinValue ? DateTime.Now.Date.AddDays(7) : _assessment.EndDate;

            // Populate toggles
            StartDateNotificationToggle.IsToggled = _assessment.StartDateNotificationEnabled;
            EndDateNotificationToggle.IsToggled = _assessment.EndDateNotificationEnabled;
        }

        private void OnStartDateNotificationToggled(object sender, ToggledEventArgs e)
        {
            _assessment.StartDateNotificationEnabled = e.Value;

            if (e.Value)
            {
                var notifyTime = StartDatePicker.Date.AddHours(8); // Ensure valid DatePicker value
                if (notifyTime > DateTime.Now)
                {
                    var notification = new NotificationRequest
                    {
                        NotificationId = _assessment.ID * 10 + 1,
                        Title = "Assessment Start Reminder",
                        Description = $"Your {_assessment.Type} \"{_assessment.Title}\" starts on {_assessment.StartDate:MM/dd/yyyy}.",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyTime
                        }
                    };
                    LocalNotificationCenter.Current.Show(notification);
                }
                else
                {
                    Console.WriteLine("NotifyTime is in the past. Notification ignored.");
                }
            }
            else
            {
                LocalNotificationCenter.Current.Cancel(_assessment.ID * 10 + 1);
            }
        }

        private void OnEndDateNotificationToggled(object sender, ToggledEventArgs e)
        {
            _assessment.EndDateNotificationEnabled = e.Value;

            if (e.Value)
            {
                var notifyTime = EndDatePicker.Date.AddHours(8); // Ensure valid DatePicker value
                if (notifyTime > DateTime.Now)
                {
                    var notification = new NotificationRequest
                    {
                        NotificationId = _assessment.ID * 10 + 2,
                        Title = "Assessment End Reminder",
                        Description = $"Your {_assessment.Type} \"{_assessment.Title}\" ends on {_assessment.EndDate:MM/dd/yyyy}.",
                        Schedule = new NotificationRequestSchedule
                        {
                            NotifyTime = notifyTime
                        }
                    };
                    LocalNotificationCenter.Current.Show(notification);
                }
                else
                {
                    Console.WriteLine("NotifyTime is in the past. Notification ignored.");
                }
            }
            else
            {
                LocalNotificationCenter.Current.Cancel(_assessment.ID * 10 + 2);
            }
        }

        private async void OnSaveAssessmentTapped(object sender, EventArgs e)
        {
            string assessmentTitle = AssessmentTitleEntry.Text;
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date;

            // Validate inputs
            if (string.IsNullOrWhiteSpace(assessmentTitle))
            {
                await DisplayAlert("Error", "Title cannot be empty.", "OK");
                return;
            }
            if (startDate > endDate)
            {
                await DisplayAlert("Error", "End date cannot be before start date.", "OK");
                return;
            }

            // Update the assessment object
            _assessment.Title = assessmentTitle;
            _assessment.StartDate = startDate;
            _assessment.EndDate = endDate;
            _assessment.StartDateNotificationEnabled = StartDateNotificationToggle.IsToggled;
            _assessment.EndDateNotificationEnabled = EndDateNotificationToggle.IsToggled;

            // Save changes to the database
            if (_isEditing)
            {
                await DatabaseService.UpdateAssessmentAsync(_assessment);
            }
            else
            {
                await DatabaseService.AddAssessmentAsync(_assessment);
            }

            // Navigate back
            await Navigation.PopAsync();
        }

        private async void OnCancelTapped(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
