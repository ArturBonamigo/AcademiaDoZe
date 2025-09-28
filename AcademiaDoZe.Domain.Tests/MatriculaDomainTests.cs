//Artur Bonamigo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Domain.Tests
{
    public class MatriculaDomainTests
    {
        private Logradouro GetValidLogradouro() => Logradouro.Criar("12345678", "Rua A", "Centro", "Cidade", "SP", "Brasil");
        private Arquivo GetValidArquivo() => Arquivo.Criar(new byte[1], ".jpg");

        private Aluno GetValidAluno() =>
            Aluno.Criar(
                "João da Silva",
                "12345678901",
                DateOnly.FromDateTime(DateTime.Today.AddYears(-20)),
                "11999999999",
                "joao@email.com",
                GetValidLogradouro(),
                "123",
                "Apto 1",
                "Senha@123",
                GetValidArquivo()
            );

        [Fact]
        public void CriarMatricula_ComDadosValidos_DeveCriarObjeto()
        {
            var aluno = GetValidAluno();
            var laudo = GetValidArquivo();
            var inicio = DateOnly.FromDateTime(DateTime.Today);
            var fim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));

            var matricula = Matricula.Criar(aluno, EMatriculaPlano.Mensal, inicio, fim, "Perder peso", EMatriculaRestricoes.None, laudo);

            Assert.NotNull(matricula);
        }

        [Fact]
        public void CriarMatricula_SemObjetivo_DeveLancarExcecao()
        {
            var aluno = GetValidAluno();
            var laudo = GetValidArquivo();
            var inicio = DateOnly.FromDateTime(DateTime.Today);
            var fim = DateOnly.FromDateTime(DateTime.Today.AddMonths(1));

            var ex = Assert.Throws<DomainException>(() =>
                Matricula.Criar(aluno, EMatriculaPlano.Mensal, inicio, fim, "", EMatriculaRestricoes.None, laudo)
            );

            Assert.Equal("OBJETIVO_OBRIGATORIO", ex.Message);
        }
    }
}
