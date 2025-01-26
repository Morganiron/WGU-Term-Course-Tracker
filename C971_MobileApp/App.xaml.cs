using C971_MobileApp.Models;

namespace C971_MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var window = new Window(new LoadingPage()); // Start with LoadingPage
            InitializeDatabaseAndSwitchToMainPageAsync(window); // Initialize database and transition
            return window;
        }

        private static async void InitializeDatabaseAndSwitchToMainPageAsync(Window window)
        {
            try
            {
                // Initialize the database
                await DatabaseService.InitializeAsync();

                // Switch to AppShell after the database is initialized
                window.Page = new AppShell();
            }
            catch (Exception ex)
            {
                // Handle any initialization errors
                Console.WriteLine($"Database initialization failed: {ex.Message}");

                var currentPage = Application.Current?.Windows.FirstOrDefault()?.Page;

                if (currentPage != null)
                {
                    await currentPage.DisplayAlert("Error", "Failed to initialize database.", "OK");
                }
                else
                {
                    Console.WriteLine("Failed to display alert: No active page found.");
                }
            }
        }
    }
}
