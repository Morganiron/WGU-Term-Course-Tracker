using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace C971_MobileApp
{
    public partial class MainPage : ContentPage
    {
        private List<Term> _terms = new List<Term>();

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await LoadTermsAsync();
        }

        private async Task LoadTermsAsync()
        {
            try
            {
                _terms = await DatabaseService.GetTermsWithCoursesAsync();
                foreach (var term in _terms)
                {
                    term.IsExpanded = false; // Default to collapsed
                }

                TermCollectionView.ItemsSource = _terms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading terms: {ex.Message}");
                await DisplayAlert("Error", "Unable to load terms. Please try again later.", "OK");
            }
        }

        private async void OnAddTermClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TermInputPage());
            await LoadTermsAsync(); // Reload terms after adding a new one
        }

        private async void OnEditTermTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Term term)
            {
                await Navigation.PushAsync(new TermInputPage(term));
                await LoadTermsAsync(); // Reload terms after editing
            }
        }

        private async void OnDeleteTermTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Term term)
            {
                var confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the term '{term.Title}'?", "Yes", "No");
                if (confirm)
                {
                    await DatabaseService.DeleteTermWithCoursesAsync(term);
                    await LoadTermsAsync(); // Reload terms after deletion
                }
            }
        }

        private void OnToggleExpandTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Term term)
            {
                term.IsExpanded = !term.IsExpanded;

                // Refresh the UI manually if necessary
                TermCollectionView.ItemsSource = null;
                TermCollectionView.ItemsSource = _terms;
            }
        }

        private async void OnAddCourseTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Term term)
            {
                await Navigation.PushAsync(new CourseInputPage(term.ID));
                await LoadTermsAsync(); // Reload terms after adding a course
            }
        }

        private async void OnDeleteCourseTapped(Object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Course course)
            {
                //var confirm = await DisplayAlert("Confirm Delete", $"Are you sure you want to delete the course '{course.Title}'?", "Yes", "No");
                //if (confirm)
                //{
                //    //await DatabaseService.DeleteCourseAsync(course);
                //    await LoadTermsAsync(); // Reload terms after deletion
                //}
            }
        }

        private async void OnEditCourseTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Course course)
            {
                await Navigation.PushAsync(new CourseInputPage(course.TermID, course));
                await LoadTermsAsync(); // Reload terms after editing
            }
        }

        private async void OnShowCourseDetailsTapped(object sender, EventArgs e)
        {
            if (sender is Label label && label.BindingContext is Course course)
            {
                await Navigation.PushAsync(new CourseDetailsPage(course));
            }
        }
    }
}
