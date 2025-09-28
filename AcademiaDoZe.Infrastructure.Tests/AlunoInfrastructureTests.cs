//Artur Bonamigo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests
{
    public class AlunoInfrastructureTests : TestBase
    {
        [Fact]
        public async Task Aluno_Adicionar_ObterPorCpf()
        {
            var repoLogradouro = new LogradouroRepository(ConnectionString, DatabaseType);
            var logradouro = await repoLogradouro.ObterPorId(1); // pegar um logradouro existente no banco

            var foto = Arquivo.Criar(new byte[] { 1, 2, 3 }, "foto.jpg");

            var aluno = Aluno.Criar(
                nome: "Aluno Teste",
                cpf: "98765432101",
                dataNascimento: new DateOnly(2000, 01, 01),
                telefone: "49999999999",
                email: "teste@teste.com",
                endereco: logradouro!,
                numero: "123",
                complemento: "Casa",
                senha: "senha123@A",
                foto: foto
            );

            var repo = new AlunoRepository(ConnectionString, DatabaseType);
            var inserido = await repo.Adicionar(aluno);

            Assert.NotNull(inserido);
            Assert.True(inserido.Id > 0);

            var repo2 = new AlunoRepository(ConnectionString, DatabaseType);
            var buscado = await repo2.ObterPorCpf("98765432101");
            Assert.NotNull(buscado);
            Assert.Equal("Aluno Teste", buscado!.Nome);
        }

        [Fact]
        public async Task Aluno_LogradouroPorId_CpfJaExiste_Adicionar()
        {
            // com base em logradouroID, acessar logradourorepository e obter o logradouro
            var logradouroId = 4;
            var repoLogradouroObterPorId = new LogradouroRepository(ConnectionString, DatabaseType);
            Logradouro? logradouro = await repoLogradouroObterPorId.ObterPorId(logradouroId);

            // cria um arquivo de exemplo
            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, "foto.jpg");

            var _cpf = "12345678900";
            // verifica se cpf já existe
            var repoAlunoCpf = new AlunoRepository(ConnectionString, DatabaseType);
            var cpfExistente = await repoAlunoCpf.CpfJaExiste(_cpf);
            Assert.False(cpfExistente, "CPF já existe no banco de dados.");

            var aluno = Aluno.Criar(
                nome: "Aluno Teste",
                cpf: "98765432103",
                dataNascimento: new DateOnly(2000, 01, 01),
                telefone: "49999999999",
                email: "teste@teste.com",
                endereco: logradouro!,
                numero: "123",
                complemento: "Casa",
                senha: "senha123@A",
                foto: arquivo
            );

            // Adicionar
            var repoAlunoAdicionar = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoInserido = await repoAlunoAdicionar.Adicionar(aluno);
            Assert.NotNull(alunoInserido);
            Assert.True(alunoInserido.Id > 0);
        }

        [Fact]
        public async Task Aluno_ObterPorCpf_Atualizar()
        {
            var _cpf = "98765432101";
            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, "foto.jpg");
            var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
            Assert.NotNull(alunoExistente);

            // criar novo colaborador com os mesmos dados, editando o que quiser
            var alunoAtualizado = Aluno.Criar(

                "zé dos testes 123",
                alunoExistente.Cpf,
                alunoExistente.DataNascimento,
                alunoExistente.Telefone,
                alunoExistente.Email,
                alunoExistente.Endereco,
                alunoExistente.Numero,
                alunoExistente.Complemento,
                alunoExistente.Senha,
                arquivo
            );
            // Usar reflexão para definir o ID

            var idProperty = typeof(Entity).GetProperty("Id");

            idProperty?.SetValue(alunoAtualizado, alunoExistente.Id);
            // Teste de Atualização

            var repoAlunoAtualizar = new AlunoRepository(ConnectionString, DatabaseType);
            var resultadoAtualizacao = await repoAlunoAtualizar.Atualizar(alunoAtualizado);
            Assert.NotNull(resultadoAtualizacao);

            Assert.Equal("zé dos testes 123", resultadoAtualizacao.Nome);
        }

        [Fact]
        public async Task Aluno_ObterPorCpf_TrocarSenha()
        {
            var _cpf = "98765432101";
            Arquivo arquivo = Arquivo.Criar(new byte[] { 1, 2, 3 }, "foto.jpg");

            var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
            Assert.NotNull(alunoExistente);

            var novaSenha = "novaSenha123";
            var repoAlunoTrocarSenha = new AlunoRepository(ConnectionString, DatabaseType);

            var resultadoTrocaSenha = await repoAlunoTrocarSenha.TrocarSenha(alunoExistente.Id, novaSenha);
            Assert.True(resultadoTrocaSenha);

            var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoAtualizado = await repoAlunoObterPorId.ObterPorId(alunoExistente.Id);

            Assert.NotNull(alunoAtualizado);
            Assert.Equal(novaSenha, alunoAtualizado.Senha);
        }

        [Fact]
        public async Task Aluno_ObterPorCpf_Remover_ObterPorId()
        {
            var _cpf = "98765432102";
            var repoAlunoObterPorCpf = new AlunoRepository(ConnectionString, DatabaseType);
            var alunoExistente = await repoAlunoObterPorCpf.ObterPorCpf(_cpf);
            Assert.NotNull(alunoExistente);

            // Remover
            var repoAlunoRemover = new AlunoRepository(ConnectionString, DatabaseType);
            var resultadoRemover = await repoAlunoRemover.Remover(alunoExistente.Id);
            Assert.True(resultadoRemover);

            var repoAlunoObterPorId = new AlunoRepository(ConnectionString, DatabaseType);
            var resultadoRemovido = await repoAlunoObterPorId.ObterPorId(alunoExistente.Id);
            Assert.Null(resultadoRemovido);
        }

        [Fact]
        public async Task Aluno_ObterTodos()
        {
            var repoAlunoRepository = new ColaboradorRepository(ConnectionString, DatabaseType);
            var resultado = await repoAlunoRepository.ObterTodos();
            Assert.NotNull(resultado);
        }
    }
}
