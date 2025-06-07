using Corretora.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Corretora.Web.Models
{
    public class AtivoViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O código do ativo é obrigatório.")]
        [StringLength(10, ErrorMessage = "O código deve ter no máximo 10 caracteres.")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O nome do ativo é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        public Ativo ToAtivo()
        {
            return new Ativo
            {
                Id = this.Id,
                Codigo = this.Codigo,
                Nome = this.Nome
            };
        }

        public static AtivoViewModel FromAtivo(Ativo ativo)
        {
            if (ativo == null)
                return null;

            return new AtivoViewModel
            {
                Id = ativo.Id,
                Codigo = ativo.Codigo,
                Nome = ativo.Nome
            };
        }
    }
}
