using Corretora.Model.Models;
using System.ComponentModel.DataAnnotations;

namespace Corretora.Web.Models
{
    public class UsuarioViewModel
    {
        [Required(ErrorMessage = "O campo Id é obrigatório.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome deve ter no máximo 150 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "O e-mail informado não é válido.")]
        [StringLength(200, ErrorMessage = "O e-mail deve ter no máximo 200 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "O percentual de corretagem é obrigatório.")]
        [Range(0.01, 100.00, ErrorMessage = "O percentual de corretagem deve estar entre 0,01% e 100%.")]
        public decimal PercCorretagem { get; set; }

        public Usuario ToUsuario()
        {
            return new Usuario
            {
                Id = this.Id,
                Nome = this.Nome,
                Email = this.Email,
                PercCorretagem = this.PercCorretagem
            };
        }

        public static UsuarioViewModel FromUsuario(Usuario usuario)
        {
            if (usuario == null)
                return null;

            return new UsuarioViewModel
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                PercCorretagem = usuario.PercCorretagem
            };
        }
    }
}