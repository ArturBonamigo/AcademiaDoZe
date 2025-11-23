using AcademiaDoZe.Presentation.AppMaui.ViewModels;
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

            // Rotas de Colaborador e Aluno já existentes
            Routing.RegisterRoute("colaborador", typeof(ColaboradorPage));
            Routing.RegisterRoute("aluno", typeof(AlunoPage));

            // 🆕 CORREÇÃO: REGISTRO DA ROTA DA LISTA DE ALUNOS
            // Assumindo que a página da lista se chama AlunoListPage
            Routing.RegisterRoute("alunoslist", typeof(AlunoListPage));

            // Rota de Matrícula
            Routing.RegisterRoute("matricula", typeof(MatriculaPage));
        }
    }
}