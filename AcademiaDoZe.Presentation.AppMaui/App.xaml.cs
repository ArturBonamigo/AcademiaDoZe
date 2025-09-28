namespace AcademiaDoZe.Presentation.AppMaui
{
    // Application conflita com o nome da nossa camada de aplicação
    // Incluir o namespace completo, Microsoft.Maui.Controls.Application, para evitar conflito
    // Direcionando para a classe Application do MAUI
    public partial class App : Microsoft.Maui.Controls.Application
    {
        public App()
        {
            InitializeComponent();

            // Captura exceções não tratadas no domínio
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = e.ExceptionObject as Exception;
                if (ex != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[ERRO GLOBAL] {ex}");
                    Console.WriteLine($"[ERRO GLOBAL] {ex}");
                }
            };

            // Captura exceções não observadas em Tasks assíncronas
            TaskScheduler.UnobservedTaskException += (sender, e) =>
            {
                System.Diagnostics.Debug.WriteLine($"[ERRO TASK] {e.Exception}");
                Console.WriteLine($"[ERRO TASK] {e.Exception}");
            };
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new AppShell());
        }
    }
}
