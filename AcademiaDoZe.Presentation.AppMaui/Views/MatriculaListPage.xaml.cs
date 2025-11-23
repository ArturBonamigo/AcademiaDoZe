using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class MatriculaListPage : ContentPage
    {
        public MatriculaListPage(MatriculaListViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
