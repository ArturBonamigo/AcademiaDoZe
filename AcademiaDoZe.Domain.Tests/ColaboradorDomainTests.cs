//Artur Bonamigo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests
{
    public class ColaboradorDomainTests
    {
        private Logradouro GetValidLogradouro() => Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1], ".jpg");

        [Fact]
        public void CriarColaborador_ComDadosValidos_DeveCriarObjeto()
        {
            var colaborador = Colaborador.Criar(
                "Maria Souza",
                "98765432100",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                "11988887777",
                "maria@email.com",
                GetValidLogradouro(),
                "456",
                "Sala 2",
                "Senha@456",
                GetValidArquivo(),
                DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                EColaboradorTipo.Atendente,
                EColaboradorVinculo.Estagio
            );

            Assert.NotNull(colaborador);
        }

        [Fact]
        public void CriarColaborador_ComCpfVazio_DeveLancarExcecao()
        {
            var ex = Assert.Throws<DomainException>(() =>
                Colaborador.Criar(
                    "Maria Souza",
                    "",
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-25)),
                    "11988887777",
                    "maria@email.com",
                    GetValidLogradouro(),
                    "456",
                    "Sala 2",
                    "Senha@456",
                    GetValidArquivo(),
                    DateOnly.FromDateTime(DateTime.Today.AddYears(-1)),
                    EColaboradorTipo.Atendente,
                    EColaboradorVinculo.Estagio
                )
            );

            Assert.Equal("CPF_OBRIGATORIO", ex.Message);
        }
    }
}
