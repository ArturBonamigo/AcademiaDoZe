using AcademiaDoZe.Presentation.AppMaui.Views;

namespace AcademiaDoZe.Presentation.AppMaui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
        }

        // Registrar rotas para páginas de detalhe, edição ou cadastro
        private static void RegisterRoutes()
        {
            Routing.RegisterRoute(nameof(LogradouroPage), typeof(LogradouroPage));
            Routing.RegisterRoute(nameof(LogradouroListPage), typeof(LogradouroListPage));
            Routing.RegisterRoute(nameof(DashboardListPage), typeof(DashboardListPage));
            Routing.RegisterRoute("colaborador", typeof(ColaboradorPage));
            Routing.RegisterRoute("aluno", typeof(AlunoPage));
        }
    }
}
