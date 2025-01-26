using System;
using C971_MobileApp.Models;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Internals;

namespace C971_MobileApp
{
    public partial class CourseInputPage : ContentPage
    {
        private readonly int _termId;
        private readonly Course? _course; // Course to edit (null for adding a new course)

        internal CourseInputPage(int termId, Course? course = null)
        {
            InitializeComponent();

            _termId = termId;
            _course = course;

            // Populate the status picker with options
            StatusPicker.ItemsSource = new List<string>
            {
                "Enrolled",
                "Not Passed",
                "Passed",
                "Started"
            };

            // Set up the page based on whether adding or editing
            if (_course != null)
            {
                PageTitleLabel.Text = "Edit Course";
                CourseNameEntry.Text = _course.Title;
                StartDatePicker.Date = _course.StartDate;
                EndDatePicker.Date = _course.EndDate;
                StatusPicker.SelectedItem = _course.Status;
                InstructorNameEntry.Text = _course.InstructorName;
                InstructorPhoneEntry.Text = _course.InstructorPhone;
                InstructorEmailEntry.Text = _course.InstructorEmail;

            }
            else
            {
                PageTitleLabel.Text = "New Course";
                StartDatePicker.Date = DateTime.Now;
                EndDatePicker.Date = DateTime.Now.AddMonths(1);
            }
        }

        private async void OnSaveCourseClicked(object sender, EventArgs e)
        {
            string courseName = CourseNameEntry.Text;
            DateTime startDate = StartDatePicker.Date;
            DateTime endDate = EndDatePicker.Date;
            string status = StatusPicker.SelectedItem?.ToString();
            string instructorName = InstructorNameEntry.Text;
            string instructorPhone = InstructorPhoneEntry.Text;
            string instructorEmail = InstructorEmailEntry.Text;


            // Validate inputs
            if (string.IsNullOrWhiteSpace(courseName))
            {
                await DisplayAlert("Error", "Course Name cannot be empty.", "OK");
                return;
            }

            if (startDate >= endDate)
            {
                await DisplayAlert("Error", "Start Date must be earlier than End Date.", "OK");
                return;
            }
            if (status == null)
            {
                await DisplayAlert("Error", "Please choose a course status.", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(instructorName))
            {
                await DisplayAlert("Error", "Please enter an instructor name.", "OK");
                return;
            }
            if (!IsValidPhone(instructorPhone))
            {
                await DisplayAlert("Error", "Please enter a valid phone number." +
                    "\nValid formats:" +
                    "\n'1-555-123-4567'" +
                    "\n'12-555-123-4567'" +
                    "\n'555-123-4567'", "OK");
                return;
            }
            if(!IsValidEmail(instructorEmail))
            {
                await DisplayAlert("Error", "Please enter a valid email address." +
                    "\nMust include an '@' symbol and \'.\'\nex. janeDoe@school.edu", "OK");
                return;
            }

            if (_course == null)
            {
                // Adding a new course
                // Save the course to the database
                var course = new Course
                {
                    TermID = _termId,
                    Title = courseName,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = status,
                    InstructorName = instructorName,
                    InstructorPhone = instructorPhone,
                    InstructorEmail = instructorEmail
                };

                await DatabaseService.Database.InsertAsync(course);

                await DisplayAlert("Success", "Course added successfully!", "OK");
            }
            else
            {
                //Updating an existing course
                _course.Title = courseName;
                _course.StartDate = startDate;
                _course.EndDate = endDate;
                _course.Status = status;
                _course.InstructorName = instructorName;
                _course.InstructorPhone = instructorPhone;
                _course.InstructorEmail = instructorEmail;

                await DatabaseService.Database.UpdateAsync(_course);
                await DisplayAlert("Success", "Course updated successfully!", "OK");
            }

            await Navigation.PopAsync();
        }

        private bool IsValidPhone(string phone)
        {
            // First validate if null or whitespace
            if (string.IsNullOrWhiteSpace(phone)) return false;

            // A simple regex to match phone numbers
            var phoneRegex = new System.Text.RegularExpressions.Regex(@"^(\d{1,2|-)?\d{3}-\d{3}-\d{4}$");
            return phoneRegex.IsMatch(phone);
        }


        private bool IsValidEmail(string email)
        {
            // First validate if null or whitespace
            if (string.IsNullOrWhiteSpace (email)) return false;

            // A regex to validate common email formats
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }


        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
