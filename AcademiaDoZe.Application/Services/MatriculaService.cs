//Artur Bonamigo

using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Application.Interfaces;
using AcademiaDoZe.Application.Mappings;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Application.Services
{
    public class MatriculaService : IMatriculaService
    {
        private readonly Func<IMatriculaRepository> _repoFactory;
        public MatriculaService(Func<IMatriculaRepository> repoFactory)
        {
            _repoFactory = repoFactory ?? throw new ArgumentNullException(nameof(repoFactory));
        }

        public async Task<MatriculaDTO?> ObterPorIdAsync(int id)
        {
            var matricula = await _repoFactory().ObterPorId(id);
            return matricula?.ToDto();
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterTodasAsync()
        {
            var matriculas = await _repoFactory().ObterTodos();
            return matriculas.Select(c => c.ToDto());
        }

        public async Task<MatriculaDTO> AdicionarAsync(MatriculaDTO matriculaDto)
        {
            // Verifica se o aluno já possui matrícula ativa
            var matriculasExistentes = await _repoFactory().ObterPorAluno(matriculaDto.AlunoMatricula.Id);
            if (matriculasExistentes.Any(m => m.DataFim >= DateOnly.FromDateTime(DateTime.Today)))
            {
                throw new InvalidOperationException(
                    $"O aluno {matriculaDto.AlunoMatricula.Id} já possui matrícula ativa."
                );
            }

            var entidade = matriculaDto.ToEntity();
            await _repoFactory().Adicionar(entidade);

            return entidade.ToDto();
        }

        public async Task<MatriculaDTO> AtualizarAsync(MatriculaDTO matriculaDto)
        {
            var matriculaExistente = await _repoFactory().ObterPorId(matriculaDto.Id);
            if (matriculaExistente == null)
            {
                throw new KeyNotFoundException($"Matrícula com ID {matriculaDto.Id} não encontrada.");
            }

            var entidade = matriculaDto.ToEntity();
            await _repoFactory().Atualizar(entidade);

            return entidade.ToDto();
        }

        public async Task<bool> RemoverAsync(int id)
        {
            var aluno = await _repoFactory().ObterPorId(id);
            if (aluno == null)
            {
                return false;
            }

            await _repoFactory().Remover(id);
            return true;
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterPorAlunoIdAsync(int alunoId)
        {
            var matriculas = await _repoFactory().ObterPorAluno(alunoId);
            return matriculas.Select(m => m.ToDto());
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterAtivasAsync(int alunoId)
        {
            var matriculas = await _repoFactory().ObterAtivas();

            if (alunoId > 0)
            {
                matriculas = matriculas.Where(m => m.AlunoMatricula.Id == alunoId);
            }

            return matriculas.Select(m => m.ToDto());
        }

        public async Task<IEnumerable<MatriculaDTO>> ObterVencendoEmDiasAsync(int dias)
        {
            var matriculas = await _repoFactory().ObterVencendoEmDias(dias);
            return matriculas.Select(m => m.ToDto());
        }
    }
}
