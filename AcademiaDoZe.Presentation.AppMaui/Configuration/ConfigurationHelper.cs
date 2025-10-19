using AcademiaDoZe.Application.DependencyInjection;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Presentation.AppMaui.Message;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Maui.Storage;
using System;

namespace AcademiaDoZe.Presentation.AppMaui.Configuration
{
    /// <summary>
    /// Responsável por configurar o acesso ao banco de dados de forma dinâmica com base nas Preferences.
    /// </summary>
    public static class ConfigurationHelper
    {
        public static void ConfigureServices(IServiceCollection services)
        {
            // Lê as preferências do banco de dados (ou aplica valores padrão)
            var (connectionString, databaseType) = ReadDbPreferences();

            // Log para debug (opcional)
            Console.WriteLine($"[CONFIG HELPER] ConnectionString inicial: {connectionString}");
            Console.WriteLine($"[CONFIG HELPER] DatabaseType: {databaseType}");

            // Cria e registra a configuração de repositório
            var repoConfig = new RepositoryConfig
            {
                ConnectionString = connectionString,
                DatabaseType = databaseType
            };

            services.AddSingleton(repoConfig);
            services.AddApplicationServices();

            // 🔄 Escuta mudanças nas Preferences (ex: quando usuário muda dados no ConfigPage)
            WeakReferenceMessenger.Default.Register<RepositoryConfig, BancoPreferencesUpdatedMessage>(
                recipient: repoConfig,
                handler: static (recipient, message) =>
                {
                    var (newConn, newType) = ReadDbPreferences();
                    recipient.ConnectionString = newConn;
                    recipient.DatabaseType = newType;

                    Console.WriteLine($"[CONFIG HELPER] Preferences atualizadas:");
                    Console.WriteLine($" → Nova ConnectionString: {newConn}");
                    Console.WriteLine($" → Novo Tipo de Banco: {newType}");
                });
        }

        /// <summary>
        /// Lê as preferências do banco e monta a ConnectionString. 
        /// Caso algo falhe, aplica valores padrão seguros.
        /// </summary>
        private static (string ConnectionString, EAppDatabaseType DatabaseType) ReadDbPreferences()
        {
            string dbServer = Preferences.Get("Servidor", "localhost");
            string dbDatabase = Preferences.Get("Banco", "db_academia_do_ze");
            string dbUser = Preferences.Get("Usuario", "sa");
            string dbPassword = Preferences.Get("Senha", "$Tmlts&29");
            string dbComplemento = Preferences.Get("Complemento", "TrustServerCertificate=True;Encrypt=True;");

            string connectionString = $"Server={dbServer};Database={dbDatabase};User Id={dbUser};Password={dbPassword};{dbComplemento}";

            // 🧩 Verificação extra: fallback automático se vier vazia ou nula
            if (string.IsNullOrWhiteSpace(connectionString) ||
                string.IsNullOrWhiteSpace(dbServer) ||
                string.IsNullOrWhiteSpace(dbDatabase))
            {
                connectionString = "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=$Tmlts&29;TrustServerCertificate=True;Encrypt=True;";
                Console.WriteLine("[CONFIG HELPER] ⚠️ ConnectionString estava vazia — aplicando padrão local.");
            }

            // Determina o tipo de banco
            var dbTypeStr = Preferences.Get("DatabaseType", EAppDatabaseType.SqlServer.ToString());
            var dbType = dbTypeStr switch
            {
                "MySql" => EAppDatabaseType.MySql,
                _ => EAppDatabaseType.SqlServer
            };

            return (connectionString, dbType);
        }
    }
}
