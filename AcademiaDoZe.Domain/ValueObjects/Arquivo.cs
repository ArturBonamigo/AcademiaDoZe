using AcademiaDoZe.Domain.Exceptions;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects
{
    public record Arquivo
    {
        public byte[] Conteudo { get; }

        private Arquivo(byte[] conteudo)
        {
            Conteudo = conteudo;
        }

        public static Arquivo Criar(byte[] conteudo, string tipoArquivo)
        {
            if (conteudo == null || conteudo.Length == 0)
                throw new DomainException("ARQUIVO_VAZIO");

            const int tamanhoMaximoBytes = 5 * 1024 * 1024; // 5MB
            if (conteudo.Length > tamanhoMaximoBytes)
                throw new DomainException("ARQUIVO_TIPO_TAMANHO");

            // aqui você poderia validar o tipoArquivo se necessário (jpg, png, pdf, etc.)

            return new Arquivo(conteudo);
        }

        public static Arquivo Criar(byte[] bytes)
        {
            // chama a outra sobrecarga e passa um tipo padrão
            return Criar(bytes, "desconhecido");
        }
    }
}
