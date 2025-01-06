namespace C971_MobileApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            InitializeDatabase(); // Call the database initialization
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }

        private async void InitializeDatabase()
        {
            try
            {
                // Initialize the database
                await DatabaseService.InitializeAsync();
            }
            catch (Exception ex)
            {
                // Handle any errors during initialization
                Console.WriteLine($"Database initialization failed: {ex.Message}");
            }
        }
    }
}