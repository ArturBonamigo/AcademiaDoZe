using System;
using System.Collections.Generic;
using AcademiaDoZe.Presentation.AppMaui.ViewModels;
using Microsoft.Maui.Controls;

namespace AcademiaDoZe.Presentation.AppMaui.Views;
public partial class LogradouroPage : ContentPage, IQueryAttributable
{
    private LogradouroViewModel? ViewModel => BindingContext as LogradouroViewModel;

    public LogradouroPage(LogradouroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Recebe os parâmetros da rota e repassa para o ViewModel
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (ViewModel == null || query == null)
            return;

        if (query.TryGetValue("Id", out var idValue) && idValue != null)
        {
            if (int.TryParse(idValue.ToString(), out int id))
            {
                ViewModel.LogradouroId = id;
            }
        }
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (ViewModel != null && !ViewModel.IsBusy && ViewModel.Logradouro.Id == 0)
        {
            // Inicializa apenas se ainda não tiver carregado
            await ViewModel.InitializeAsync();
        }
    }
}
