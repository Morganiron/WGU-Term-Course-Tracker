using System;
using Microsoft.Maui.Controls;

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

            // Set up the page based on whether adding or editing
            if (_course != null)
            {
                PageTitleLabel.Text = "Edit Course";
                CourseNameEntry.Text = _course.Title;
                StartDatePicker.Date = _course.StartDate;
                EndDatePicker.Date = _course.EndDate;
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

            // Save the course to the database
            var course = new Course
            {
                TermID = _termId,
                Title = courseName,
                StartDate = startDate,
                EndDate = endDate
            };

            await DatabaseService.Database.InsertAsync(course);

            await DisplayAlert("Success", "Course added successfully.", "OK");
            await Navigation.PopAsync();

        }

        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
