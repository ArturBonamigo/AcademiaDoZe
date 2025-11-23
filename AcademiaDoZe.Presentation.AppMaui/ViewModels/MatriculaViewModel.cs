using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Maui.Storage;
using System.Text.Json; // 💡 Importado para desserialização JSON
using System.Linq; // Necessário para o Linq na classe

namespace AcademiaDoZe.Presentation.AppMaui.ViewModels
{
    [QueryProperty(nameof(MatriculaId), "Id")]
    // 🆕 NOVO: QueryProperty para receber o aluno selecionado (JSON serializado)
    [QueryProperty(nameof(AlunoSelecionadoJson), "AlunoSelecionado")]
    public partial class MatriculaViewModel : BaseViewModel
    {
        private readonly IMatriculaService _service;

        public MatriculaViewModel(IMatriculaService service)
        {
            _service = service;
            Title = "Matrícula";
            Planos = Enum.GetValues(typeof(EAppMatriculaPlano)).Cast<EAppMatriculaPlano>().ToList();
            RestricoesOptions = new ObservableCollection<RestricaoOption>();

            foreach (EAppMatriculaRestricoes r in Enum.GetValues(typeof(EAppMatriculaRestricoes)))
            {
                RestricoesOptions.Add(new RestricaoOption
                {
                    Value = r,
                    Name = GetDisplayName(r),
                    IsChecked = false
                });
            }

            DataInicioDate = DateTime.Today;
            DataFimDate = DateTime.Today.AddMonths(1);

            // 🆕 Inicializa a propriedade Aluno com o AlunoMatricula do DTO
            Aluno = Matricula.AlunoMatricula;
        }

        public class RestricaoOption : ObservableObject
        {
            public EAppMatriculaRestricoes Value { get; set; }
            public string Name { get; set; } = string.Empty;
            private bool _isChecked;
            public bool IsChecked
            {
                get => _isChecked;
                set => SetProperty(ref _isChecked, value);
            }
        }

        private static string GetDisplayName(Enum e)
        {
            var mem = e.GetType().GetMember(e.ToString());
            var attr = mem.FirstOrDefault()?.GetCustomAttributes(false)
                        .OfType<System.ComponentModel.DataAnnotations.DisplayAttribute>()
                        .FirstOrDefault();
            return attr?.GetName() ?? e.ToString();
        }

        // Bindings
        private int _matriculaId;
        public int MatriculaId
        {
            get => _matriculaId;
            set
            {
                SetProperty(ref _matriculaId, value);
                if (_matriculaId > 0) _ = LoadAsync();
            }
        }

        private MatriculaDTO _matricula = new MatriculaDTO
        {
            AlunoMatricula = new AlunoDTO { Id = 0, Nome = string.Empty, Cpf = string.Empty, DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-18)), Telefone = string.Empty, Endereco = new LogradouroDTO { Cep = string.Empty, Nome = string.Empty, Bairro = string.Empty, Cidade = string.Empty, Estado = string.Empty, Pais = string.Empty }, Numero = string.Empty },
            Plano = EAppMatriculaPlano.Mensal,
            DataInicio = DateOnly.FromDateTime(DateTime.Today),
            DataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
            Objetivo = string.Empty,
            RestricoesMedicas = EAppMatriculaRestricoes.None
        };

        public MatriculaDTO Matricula
        {
            get => _matricula;
            set => SetProperty(ref _matricula, value);
        }

        private AlunoDTO? _aluno;
        public AlunoDTO? Aluno // 💡 Usado no XAML para exibição do aluno
        {
            get => _aluno;
            set => SetProperty(ref _aluno, value);
        }

        // 🆕 NOVO: Propriedade para receber o aluno serializado na navegação
        private string _alunoSelecionadoJson = string.Empty;
        public string AlunoSelecionadoJson
        {
            get => _alunoSelecionadoJson;
            set
            {
                if (string.IsNullOrWhiteSpace(value)) return;

                SetProperty(ref _alunoSelecionadoJson, value);

                try
                {
                    // Desserializa o JSON de volta para AlunoDTO
                    var aluno = JsonSerializer.Deserialize<AlunoDTO>(Uri.UnescapeDataString(value));

                    if (aluno != null)
                    {
                        Aluno = aluno; // Atualiza a propriedade de binding
                        Matricula.AlunoMatricula = aluno; // Atualiza o DTO da matrícula
                        _alunoSelecionadoJson = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    Shell.Current.DisplayAlert("Erro de Seleção", $"Falha ao carregar aluno selecionado: {ex.Message}", "OK");
                }
            }
        }

        public List<EAppMatriculaPlano> Planos { get; }
        public ObservableCollection<RestricaoOption> RestricoesOptions { get; }

        // Date wrappers for DatePicker
        private DateTime _dataInicioDate;
        public DateTime DataInicioDate
        {
            get => _dataInicioDate;
            set
            {
                SetProperty(ref _dataInicioDate, value);
                Matricula.DataInicio = DateOnly.FromDateTime(value);
            }
        }

        private DateTime _dataFimDate;
        public DateTime DataFimDate
        {
            get => _dataFimDate;
            set
            {
                SetProperty(ref _dataFimDate, value);
                Matricula.DataFim = DateOnly.FromDateTime(value);
            }
        }

        private string _objetivo = string.Empty;
        public string Objetivo
        {
            get => _objetivo;
            set => SetProperty(ref _objetivo, value);
        }

        private string? _observacoes;
        public string? Observacoes
        {
            get => _observacoes;
            set => SetProperty(ref _observacoes, value);
        }

        private ArquivoDTO? _laudo;
        public ArquivoDTO? Laudo
        {
            get => _laudo;
            set => SetProperty(ref _laudo, value);
        }

        [RelayCommand]
        public async Task LoadAsync()
        {
            if (MatriculaId <= 0) return;
            try
            {
                IsBusy = true;
                var dto = await _service.ObterPorIdAsync(MatriculaId);
                if (dto == null) return;
                Matricula = dto;
                Aluno = dto.AlunoMatricula; // Atualiza a propriedade Aluno na carga
                DataInicioDate = dto.DataInicio.ToDateTime(TimeOnly.MinValue);
                DataFimDate = dto.DataFim.ToDateTime(TimeOnly.MinValue);
                Objetivo = dto.Objetivo;
                Observacoes = dto.ObservacoesRestricoes;
                Laudo = dto.LaudoMedico;

                // aplicar restrições
                foreach (var opt in RestricoesOptions)
                {
                    opt.IsChecked = (dto.RestricoesMedicas & opt.Value) == opt.Value;
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Falha ao carregar matrícula: {ex.Message}", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task SelectAlunoAsync()
        {
            // CORREÇÃO: Navega para a ROTA 'alunoslist' (agora registrada)
            // e passa o parâmetro 'selecting=true'
            await Shell.Current.GoToAsync("alunoslist?selecting=true");
        }

        [RelayCommand]
        public async Task PickLaudoAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Selecione o laudo médico (imagem ou PDF)",
                    FileTypes = FilePickerFileType.Images
                });

                if (result == null) return;

                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                Laudo = new ArquivoDTO { Conteudo = ms.ToArray(), ContentType = result.ContentType };
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao selecionar arquivo: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task SaveAsync()
        {
            try
            {
                // Verifica se o Aluno selecionado tem um ID válido
                if (Aluno == null || Aluno.Id <= 0)
                {
                    await Shell.Current.DisplayAlert("Atenção", "Selecione um aluno válido para criar a matrícula.", "OK");
                    return;
                }

                // valida idade 12-16 exige laudo
                int idade = 0;
                try { idade = CalculateAge(Aluno.DataNascimento); } catch { }

                bool exigeLaudo = idade >= 12 && idade <= 16;
                if (exigeLaudo && (Laudo == null || Laudo.Conteudo == null || Laudo.Conteudo.Length == 0))
                {
                    await Shell.Current.DisplayAlert("Atenção", "Alunos entre 12 e 16 anos devem apresentar laudo médico.", "OK");
                    return;
                }

                // montar DTO
                Matricula.AlunoMatricula = Aluno;
                Matricula.Plano = Matricula.Plano; // já setado via Picker
                Matricula.DataInicio = DateOnly.FromDateTime(DataInicioDate);
                Matricula.DataFim = DateOnly.FromDateTime(DataFimDate);
                Matricula.Objetivo = Objetivo ?? string.Empty;
                Matricula.ObservacoesRestricoes = Observacoes;
                Matricula.LaudoMedico = Laudo;

                // montar restrições a partir das opções
                var restr = RestricoesOptions.Where(r => r.IsChecked).Aggregate(EAppMatriculaRestricoes.None, (acc, x) => acc | x.Value);
                Matricula.RestricoesMedicas = restr;

                if (MatriculaId > 0)
                {
                    await _service.AtualizarAsync(Matricula);
                }
                else
                {
                    await _service.AdicionarAsync(Matricula);
                }

                await Shell.Current.GoToAsync("..");
            }
            catch (InvalidOperationException inv)
            {
                await Shell.Current.DisplayAlert("Atenção", inv.Message, "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Falha ao salvar matrícula: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        public async Task DeleteAsync()
        {
            if (MatriculaId <= 0) return;
            bool confirm = await Shell.Current.DisplayAlert("Confirmar", "Remover matrícula?", "Sim", "Não");
            if (!confirm) return;

            try
            {
                var ok = await _service.RemoverAsync(MatriculaId);
                if (ok) await Shell.Current.GoToAsync("..");
                else await Shell.Current.DisplayAlert("Erro", "Não foi possível remover a matrícula.", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Erro", $"Erro ao remover matrícula: {ex.Message}", "OK");
            }
        }

        private int CalculateAge(DateOnly nascimento)
        {
            var today = DateTime.Today;
            int age = today.Year - nascimento.Year;
            if (new DateTime(today.Year, nascimento.Month, nascimento.Day) > today) age--;
            return age;
        }
    }
}