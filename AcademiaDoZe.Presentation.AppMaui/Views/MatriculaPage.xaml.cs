using AcademiaDoZe.Presentation.AppMaui.ViewModels;

namespace AcademiaDoZe.Presentation.AppMaui.Views
{
    public partial class MatriculaPage : ContentPage
    {
        public MatriculaPage(MatriculaViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
