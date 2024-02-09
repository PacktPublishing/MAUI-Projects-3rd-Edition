namespace Calculator
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnHandlerChanging(HandlerChangingEventArgs args)
        {
            base.OnHandlerChanging(args);
            MainPage = args.NewHandler.MauiContext.Services.GetService<MainPage>();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);
            if (OperatingSystem.IsWindows() || OperatingSystem.IsMacCatalyst())
            {
                window.Created += Window_Created;
            }
            return window;
        }

        private async void Window_Created(object sender, EventArgs e)
        {
            const int defaultWidth = 450;
            const int defaultHeight = 800;

            var window = (Window)sender;
            window.Width = defaultWidth;
            window.Height = defaultHeight;
            window.X = -defaultWidth;
            window.Y = -defaultHeight;

            await window.Dispatcher.DispatchAsync(() => { });

            var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
            window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
            window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;

            window.Created -= Window_Created;
        }
    }
}