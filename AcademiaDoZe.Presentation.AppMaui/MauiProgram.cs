using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using AcademiaDoZe.Presentation.AppMaui.Views;
using Microsoft.Extensions.Logging;
using AcademiaDoZe.Presentation.AppMaui.Configuration;
using AcademiaDoZe.Infrastructure.Repositories;
using AcademiaDoZe.Application.Services;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Infrastructure.Data;
using Microsoft.Maui.Storage;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIcons");
                });

            // Tratamento global de exceções
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                var ex = (Exception)e.ExceptionObject;
                Console.WriteLine($"[ERRO GLOBAL] {ex.GetType().Name}: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            };

            // ------------------------------------------------------------------
            // ⚙️ CONFIGURAÇÃO DE CONEXÃO DINÂMICA VIA PREFERENCES
            // ------------------------------------------------------------------

            string dbServer = Preferences.Get("Servidor", "localhost");
            string dbDatabase = Preferences.Get("Banco", "db_academia_do_ze");
            string dbUser = Preferences.Get("Usuario", "sa");
            string dbPassword = Preferences.Get("Senha", "$Tmlts&29");
            string dbComplemento = Preferences.Get("Complemento", "TrustServerCertificate=True;Encrypt=True;");

            // Monta connection string completa (SQL Server)
            string connectionString =
                $"Server={dbServer};Database={dbDatabase};User Id={dbUser};Password={dbPassword};{dbComplemento}";

            DatabaseType databaseType = DatabaseType.SqlServer; // altere para MySql se necessário

            // Configurações adicionais globais (se houver)
            ConfigurationHelper.ConfigureServices(builder.Services);

            // ------------------------------------------------------------------
            // 📦 REGISTRO DE VIEWMODELS
            // ------------------------------------------------------------------
            builder.Services.AddTransient<DashboardListViewModel>();
            builder.Services.AddTransient<LogradouroListViewModel>();
            builder.Services.AddTransient<LogradouroViewModel>();
            builder.Services.AddTransient<ColaboradorListViewModel>();
            builder.Services.AddTransient<ColaboradorViewModel>();
            builder.Services.AddTransient<AlunoListViewModel>();
            builder.Services.AddTransient<AlunoViewModel>();

            // ------------------------------------------------------------------
            // 🧩 REGISTRO DE VIEWS
            // ------------------------------------------------------------------
            builder.Services.AddTransient<DashboardListPage>();
            builder.Services.AddTransient<LogradouroListPage>();
            builder.Services.AddTransient<LogradouroPage>();
            builder.Services.AddTransient<ConfigPage>();
            builder.Services.AddTransient<ColaboradorListPage>();
            builder.Services.AddTransient<ColaboradorPage>();
            builder.Services.AddTransient<AlunoListPage>();
            builder.Services.AddTransient<AlunoListPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
