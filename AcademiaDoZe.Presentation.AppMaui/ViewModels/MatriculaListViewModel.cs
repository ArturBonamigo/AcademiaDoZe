using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    public partial class MatriculaListViewModel : BaseViewModel
    {
        private readonly IMatriculaService _matriculaService;

        public MatriculaListViewModel(IMatriculaService matriculaService)
        {
            _matriculaService = matriculaService;
            Title = "Matrículas";
            Matriculas = new ObservableCollection<MatriculaDTO>();
        }

        private ObservableCollection<MatriculaDTO> _matriculas;
        public ObservableCollection<MatriculaDTO> Matriculas
        {
            get => _matriculas;
            set => SetProperty(ref _matriculas, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private MatriculaDTO? _selected;
        public MatriculaDTO? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        [RelayCommand]
        private async Task LoadAsync()
        {
            if (IsBusy) return;
            try
            {
                IsBusy = true;
                Matriculas.Clear();
                var lista = await _matriculaService.ObterTodasAsync();
                if (lista != null)
                {
                    foreach (var m in lista) Matriculas.Add(m);
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao carregar matrículas: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
                IsRefreshing = false;
            }
        }

        [RelayCommand]
        private async Task RefreshAsync()
        {
            IsRefreshing = true;
            await LoadAsync();
        }

        [RelayCommand]
        private async Task AddAsync()
        {
            try
            {
                await Shell.Current.GoToAsync("matricula"); // ou rota que você registrou
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao navegar: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditAsync(MatriculaDTO matricula)
        {
            if (matricula == null) return;
            try
            {
                await Shell.Current.GoToAsync($"matricula?Id={matricula.Id}");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao abrir matrícula: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task DeleteAsync(MatriculaDTO matricula)
        {
            if (matricula == null) return;
            bool confirm = await Shell.Current.DisplayAlert("Confirmação", "Remover matrícula?", "Sim", "Não");
            if (!confirm) return;

            try
            {
                var ok = await _matriculaService.RemoverAsync(matricula.Id);
                if (ok) Matriculas.Remove(matricula);
                else await Shell.Current.DisplayAlert("Erro", "Não foi possível remover a matrícula.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao remover matrícula: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private void Search()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                _ = LoadAsync();
                return;
            }

            var filtro = SearchText.Trim().ToLower();
            var list = Matriculas.Where(m =>
                (m.AlunoMatricula?.Nome?.ToLower().Contains(filtro) ?? false) ||
                (m.AlunoMatricula?.Cpf?.ToLower().Contains(filtro) ?? false) ||
                (m.Objetivo?.ToLower().Contains(filtro) ?? false)).ToList();

            Matriculas.Clear();
            foreach (var m in list) Matriculas.Add(m);
        }
    }
}
