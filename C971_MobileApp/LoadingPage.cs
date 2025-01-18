using Microsoft.Maui.Controls;

namespace C971_MobileApp
{
    internal partial class LoadingPage : ContentPage
    {
        public LoadingPage()
        {
            // Create a grid layout
            var grid = new Grid
            {
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto }, // Spinner
                new RowDefinition { Height = GridLength.Auto }  // Text
            }
            };

            // Add the ActivityIndicator
            var activityIndicator = new ActivityIndicator
            {
                IsRunning = true,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };
            grid.Add(activityIndicator, 0, 0); // Add spinner to the first row

            // Add the Label
            var label = new Label
            {
                Text = "Loading...",
                FontSize = 20,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Margin = new Thickness(0, 10, 0, 0) // Add margin for spacing below the spinner
            };
            grid.Add(label, 0, 1); // Add label to the second row

            // Set grid content to the page
            Content = grid;

            // Optional background color for better visibility
            BackgroundColor = Colors.White;
        }
    }
}
