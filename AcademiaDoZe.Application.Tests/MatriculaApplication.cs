//Artur Bonamigo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Enums;
using AcademiaDoZe.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Cryptography;

namespace AcademiaDoZe.Application.Tests
{
    public class MatriculaApplicationTests
    {
        const string connectionString = "Server=localhost;Database=db_academia_do_ze;User Id=sa;Password=$Tmlts&29;TrustServerCertificate=True;Encrypt=True";
        const EAppDatabaseType databaseType = EAppDatabaseType.SqlServer;

        [Fact(Timeout = 60000)]
        public async Task MatriculaService_Integracao_Adicionar_Obter_Atualizar_Remover()
        {
            // ARRANGE - Configuração do ambiente e dependências
            var services = DependencyInjection.ConfigureServices(connectionString, databaseType);
            var provider = DependencyInjection.BuildServiceProvider(services);

            var matriculaService = provider.GetRequiredService<IMatriculaService>();
            var alunoService = provider.GetRequiredService<IAlunoService>();
            var logradouroService = provider.GetRequiredService<ILogradouroService>();

            // Variáveis para cleanup no 'finally'
            AlunoDTO? alunoCriado = null;
            MatriculaDTO? matriculaCriada = null;

            try
            {
                // ARRANGE - Criar um aluno temporário para o teste
                var logradouro = await logradouroService.ObterPorIdAsync(1);
                Assert.NotNull(logradouro);

                var alunoDto = new AlunoDTO
                {
                    Nome = "Aluno Para Matrícula Teste",
                    Cpf = GerarCpfFake(),
                    DataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                    Telefone = "49999990000",
                    Email = $"aluno.matricula.{Guid.NewGuid()}@teste.com",
                    Endereco = logradouro,
                    Numero = "100",
                    Senha = "Senha@123"
                };
                alunoCriado = await alunoService.AdicionarAsync(alunoDto);
                Assert.NotNull(alunoCriado);

                // ARRANGE - Dados da nova matrícula
                var dto = new MatriculaDTO
                {
                    AlunoMatricula = alunoCriado,
                    Plano = EAppMatriculaPlano.Anual,
                    DataInicio = DateOnly.FromDateTime(DateTime.Today),
                    DataFim = DateOnly.FromDateTime(DateTime.Today.AddYears(1)),
                    Objetivo = "Hipertrofia",
                    RestricoesMedicas = EAppMatriculaRestricoes.None,
                    ObservacoesRestricoes = string.Empty
                };

                // ACT - Adicionar
                matriculaCriada = await matriculaService.AdicionarAsync(dto);

                // ASSERT - Criação
                Assert.NotNull(matriculaCriada);
                Assert.True(matriculaCriada.Id > 0);
                Assert.Equal(alunoCriado.Id, matriculaCriada.AlunoMatricula.Id);

                // ACT - Obter por ID
                var obtida = await matriculaService.ObterPorIdAsync(matriculaCriada.Id);

                // ASSERT - Obter
                Assert.NotNull(obtida);
                Assert.Equal(matriculaCriada.Id, obtida.Id);

                // ACT - Atualizar
                var paraAtualizar = obtida;
                paraAtualizar.Objetivo = "Definição Muscular";
                var atualizada = await matriculaService.AtualizarAsync(paraAtualizar);

                // ASSERT - Atualizar
                Assert.NotNull(atualizada);
                Assert.Equal("Definição Muscular", atualizada.Objetivo);

                // ACT - Remover
                var removido = await matriculaService.RemoverAsync(matriculaCriada.Id);
                Assert.True(removido);

                // ACT - Conferir remoção
                var aposRemocao = await matriculaService.ObterPorIdAsync(matriculaCriada.Id);
                Assert.Null(aposRemocao);
            }
            finally
            {
                // CLEAN-UP - Remover os dados criados, na ordem correta
                if (matriculaCriada is not null)
                {
                    try { await matriculaService.RemoverAsync(matriculaCriada.Id); } catch { }
                }
                if (alunoCriado is not null)
                {
                    try { await alunoService.RemoverAsync(alunoCriado.Id); } catch { }
                }
            }
        }

        // Helper para gerar um CPF numérico de 11 dígitos para evitar conflitos no banco
        private static string GerarCpfFake()
        {
            var buffer = new byte[8];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(buffer);
            long longRnd = BitConverter.ToInt64(buffer, 0);
            return Math.Abs(longRnd).ToString().PadLeft(11, '0').Substring(0, 11);
        }
    }
}