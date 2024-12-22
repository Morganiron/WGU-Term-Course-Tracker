namespace C971_MobileApp;

public partial class TermInputPage : ContentPage
{
    private Term _term; // Term to edit (null for adding a new term)

    internal TermInputPage(Term? term = null)
    {
        InitializeComponent();

        // Set up the page based on whether adding or editing
        _term = term;
        if (_term != null)
        {
            PageTitleLabel.Text = "Edit Term";
            TermTitleEntry.Text = _term.Title;
            StartDatePicker.Date = _term.StartDate;
            EndDatePicker.Date = _term.EndDate;
        }
        else
        {
            PageTitleLabel.Text = "New Term";
            StartDatePicker.Date = DateTime.Now;
            EndDatePicker.Date = DateTime.Now.AddMonths(6);
        }
    }

    private async void OnSaveChangesClicked(object sender, EventArgs e)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(TermTitleEntry.Text))
        {
            await DisplayAlert("Error", "Please enter a valid term title.", "OK");
            return;
        }

        if (EndDatePicker.Date <= StartDatePicker.Date)
        {
            await DisplayAlert("Error", "End date must be after start date.", "OK");
            return;
        }

        // Save or update term
        if (_term == null)
        {
            // Adding a new term
            _term = new Term
            {
                Title = TermTitleEntry.Text,
                StartDate = StartDatePicker.Date,
                EndDate = EndDatePicker.Date
            };
            // Add _term to your data source (e.g., database or list)
            await DisplayAlert("Success", "Term added successfully!", "OK");
        }
        else
        {
            // Updating an existing term
            _term.Title = TermTitleEntry.Text;
            _term.StartDate = StartDatePicker.Date;
            _term.EndDate = EndDatePicker.Date;
            // Update _term in your data source
            await DisplayAlert("Success", "Term updated successfully!", "OK");
        }

        // Navigate back
        await Navigation.PopAsync();
    }

    private async void OnDiscardChangesClicked(object sender, EventArgs e)
    {
        bool discard = await DisplayAlert("Discard Changes", "Are you sure you want to discard changes?", "Yes", "No");
        if (discard)
        {
            await Navigation.PopAsync();
        }
    }

    private async void OnAddCourseClicked(object sender, EventArgs e)
    {
        // Navigate to AddCoursePage (or similar functionality)
        //await Navigation.PushAsync(new AddCoursePage(_term));
    }
}
