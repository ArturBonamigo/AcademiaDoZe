//Artur Bonamigo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using Moq;

namespace AcademiaDoZe.Application.Tests
{
    public class MoqMatriculaServiceTests
    {
        private readonly Mock<IMatriculaService> _matriculaServiceMock;
        private readonly IMatriculaService _matriculaService;

        public MoqMatriculaServiceTests()
        {
            _matriculaServiceMock = new Mock<IMatriculaService>();
            _matriculaService = _matriculaServiceMock.Object;
        }

        private MatriculaDTO CriarMatriculaPadrao(int id = 1)
        {
            return new MatriculaDTO
            {
                Id = id,
                AlunoMatricula = new AlunoDTO
                {
                    Id = 1,
                    Nome = "Aluno Teste",
                    Cpf = "12345678901",
                    DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                    Telefone = "11999999999",
                    Email = "aluno@teste.com",
                    Endereco = new LogradouroDTO
                    {
                        Id = 1,
                        Cep = "12345678",
                        Nome = "Rua Teste",
                        Bairro = "Centro",
                        Cidade = "São Paulo",
                        Estado = "SP",
                        Pais = "Brasil"
                    },
                    Numero = "100",
                    Complemento = "Apto 101",
                    Senha = "Senha@123"
                },
                Plano = EAppMatriculaPlano.Mensal,
                DataInicio = DateOnly.FromDateTime(DateTime.Today),
                DataFim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1)),
                Objetivo = "Condicionamento",
                RestricoesMedicas = EAppMatriculaRestricoes.None,
                ObservacoesRestricoes = null,
                LaudoMedico = null
            };
        }


        [Fact]
        public async Task ObterPorIdAsync_DeveRetornarMatricula_QuandoExistir()
        {
            var matricula = CriarMatriculaPadrao(1);
            _matriculaServiceMock.Setup(s => s.ObterPorIdAsync(1)).ReturnsAsync(matricula);

            var result = await _matriculaService.ObterPorIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _matriculaServiceMock.Verify(s => s.ObterPorIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task AdicionarAsync_DeveCriarMatricula_QuandoValida()
        {
            var novaMatricula = CriarMatriculaPadrao(0);
            var criada = CriarMatriculaPadrao(1);
            _matriculaServiceMock.Setup(s => s.AdicionarAsync(It.IsAny<MatriculaDTO>())).ReturnsAsync(criada);

            var result = await _matriculaService.AdicionarAsync(novaMatricula);

            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            _matriculaServiceMock.Verify(s => s.AdicionarAsync(It.IsAny<MatriculaDTO>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarAsync_DeveAtualizarMatricula()
        {
            var matricula = CriarMatriculaPadrao(1);
            matricula.Objetivo = "Hipertrofia";
            _matriculaServiceMock.Setup(s => s.AtualizarAsync(It.IsAny<MatriculaDTO>())).ReturnsAsync(matricula);

            var result = await _matriculaService.AtualizarAsync(matricula);

            Assert.NotNull(result);
            Assert.Equal("Hipertrofia", result.Objetivo);
            _matriculaServiceMock.Verify(s => s.AtualizarAsync(It.IsAny<MatriculaDTO>()), Times.Once);
        }

        [Fact]
        public async Task RemoverAsync_DeveRetornarTrue_QuandoExistir()
        {
            _matriculaServiceMock.Setup(s => s.RemoverAsync(1)).ReturnsAsync(true);

            var result = await _matriculaService.RemoverAsync(1);

            Assert.True(result);
            _matriculaServiceMock.Verify(s => s.RemoverAsync(1), Times.Once);
        }

        [Fact]
        public async Task ObterTodasAsync_DeveRetornarListaDeMatriculas()
        {
            var lista = new List<MatriculaDTO> { CriarMatriculaPadrao(1), CriarMatriculaPadrao(2) };
            _matriculaServiceMock.Setup(s => s.ObterTodasAsync()).ReturnsAsync(lista);

            var result = await _matriculaService.ObterTodasAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            _matriculaServiceMock.Verify(s => s.ObterTodasAsync(), Times.Once);
        }
    }
}
