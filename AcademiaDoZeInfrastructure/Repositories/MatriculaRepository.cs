//Artur Bonamigo

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories
{
    public class MatriculaRepository : BaseRepository<Matricula>, IMatriculaRepository
    {
        public MatriculaRepository(string connectionString, DatabaseType databaseType)
            : base(connectionString, databaseType) { }

        // Adiciona uma nova matrícula no banco
        public override async Task<Matricula> Adicionar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = _databaseType == DatabaseType.SqlServer
                    ? $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricao_medica, obs_restricao, laudo_medico) " +
                      "OUTPUT INSERTED.id_matricula " +
                      "VALUES (@AlunoId, @Plano, @DataInicio, @DataFim, @Objetivo, @RestricoesMedicas, @ObservacoesRestricoes, @LaudoMedico);"
                    : $"INSERT INTO {TableName} (aluno_id, plano, data_inicio, data_fim, objetivo, restricao_medica, obs_restricao, laudo_medico) " +
                      "VALUES (@AlunoId, @Plano, @DataInicio, @DataFim, @Objetivo, @RestricoesMedicas, @ObservacoesRestricoes, @LaudoMedico); " +
                      "SELECT LAST_INSERT_ID();";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@RestricoesMedicas", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@ObservacoesRestricoes", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                var id = await command.ExecuteScalarAsync();
                if (id != null && id != DBNull.Value)
                {
                    typeof(Entity).GetProperty("Id")?.SetValue(entity, Convert.ToInt32(id));
                }

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao adicionar matrícula: {ex.Message}", ex);
            }
        }

        // Atualiza os dados de uma matrícula existente
        public override async Task<Matricula> Atualizar(Matricula entity)
        {
            try
            {
                await using var connection = await GetOpenConnectionAsync();

                string query = $"UPDATE {TableName} SET aluno_id = @AlunoId, plano = @Plano, data_inicio = @DataInicio, data_fim = @DataFim, " +
                               "objetivo = @Objetivo, restricao_medica = @RestricoesMedicas, obs_restricao = @ObservacoesRestricoes, laudo_medico = @LaudoMedico " +
                               "WHERE id_matricula = @Id";

                await using var command = DbProvider.CreateCommand(query, connection);
                command.Parameters.Add(DbProvider.CreateParameter("@Id", entity.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", entity.AlunoMatricula.Id, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Plano", (int)entity.Plano, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataInicio", entity.DataInicio, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@DataFim", entity.DataFim, DbType.Date, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@Objetivo", entity.Objetivo, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@RestricoesMedicas", (int)entity.RestricoesMedicas, DbType.Int32, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@ObservacoesRestricoes", (object)entity.ObservacoesRestricoes ?? DBNull.Value, DbType.String, _databaseType));
                command.Parameters.Add(DbProvider.CreateParameter("@LaudoMedico", (object)entity.LaudoMedico?.Conteudo ?? DBNull.Value, DbType.Binary, _databaseType));

                if (await command.ExecuteNonQueryAsync() == 0)
                    throw new InvalidOperationException($"Nenhuma matrícula encontrada com ID {entity.Id}.");

                return entity;
            }
            catch (DbException ex)
            {
                throw new InvalidOperationException($"Erro ao atualizar matrícula ID {entity.Id}: {ex.Message}", ex);
            }
        }

        // Obtém todas as matrículas de um aluno específico
        public async Task<IEnumerable<Matricula>> ObterPorAluno(int alunoId)
        {
            await using var connection = await GetOpenConnectionAsync();
            string query = $"SELECT * FROM {TableName} WHERE aluno_id = @AlunoId";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@AlunoId", alunoId, DbType.Int32, _databaseType));

            var lista = new List<Matricula>();
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(await MapAsync(reader));
            }
            return lista;
        }

        // Obtém matrículas ativas
        public async Task<IEnumerable<Matricula>> ObterAtivas()
        {
            await using var connection = await GetOpenConnectionAsync();

            string query = _databaseType == DatabaseType.SqlServer
                ? $"SELECT * FROM {TableName} WHERE data_fim >= GETDATE()"
                : $"SELECT * FROM {TableName} WHERE data_fim >= CURDATE()";

            var lista = new List<Matricula>();
            await using var command = DbProvider.CreateCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(await MapAsync(reader));
            }
            return lista;
        }

        // Obtém todas as matrículas
        public override async Task<IEnumerable<Matricula>> ObterTodos()
        {
            await using var connection = await GetOpenConnectionAsync();
            string query = $"SELECT * FROM {TableName}";

            var lista = new List<Matricula>();
            await using var command = DbProvider.CreateCommand(query, connection);
            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(await MapAsync(reader));
            }
            return lista;
        }

        // Obtém matrícula por ID
        public override async Task<Matricula?> ObterPorId(int id)
        {
            await using var connection = await GetOpenConnectionAsync();
            string query = $"SELECT * FROM {TableName} WHERE id_matricula = @Id";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));

            await using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return await MapAsync(reader);
            }
            return null;
        }

        // Remove uma matrícula
        public override async Task<bool> Remover(int id)
        {
            await using var connection = await GetOpenConnectionAsync();
            string query = $"DELETE FROM {TableName} WHERE id_matricula = @Id";

            await using var command = DbProvider.CreateCommand(query, connection);
            command.Parameters.Add(DbProvider.CreateParameter("@Id", id, DbType.Int32, _databaseType));

            return await command.ExecuteNonQueryAsync() > 0;
        }

        // Mapeia os dados do banco para um objeto Matricula
        protected override async Task<Matricula> MapAsync(DbDataReader reader)
        {
            var alunoRepository = new AlunoRepository(_connectionString, _databaseType);
            var aluno = await alunoRepository.ObterPorId(Convert.ToInt32(reader["aluno_id"]));

            var matricula = Matricula.Criar(
                alunoMatricula: aluno!,
                plano: (Domain.Enums.EMatriculaPlano)Convert.ToInt32(reader["plano"]),
                dataInicio: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_inicio"])),
                dataFim: DateOnly.FromDateTime(Convert.ToDateTime(reader["data_fim"])),
                objetivo: reader["objetivo"].ToString()!,
                restricoesMedicas: (Domain.Enums.EMatriculaRestricoes)Convert.ToInt32(reader["restricao_medica"]),
                laudoMedico: reader["laudo_medico"] is DBNull ? null : Domain.ValueObjects.Arquivo.Criar((byte[])reader["laudo_medico"], "pdf"),
                observacoesRestricoes: reader["obs_restricao"]?.ToString() ?? ""
            );

            typeof(Entity).GetProperty("Id")?.SetValue(matricula, Convert.ToInt32(reader["id_matricula"]));
            return matricula;
        }

        public Task<IEnumerable<Matricula>> ObterVencendoEmDias(int dias)
        {
            throw new NotImplementedException();
        }
    }
}