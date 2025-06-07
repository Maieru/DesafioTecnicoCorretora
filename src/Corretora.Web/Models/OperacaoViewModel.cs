using Corretora.Model.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Corretora.Web.Models
{

    public class OperacaoViewModel
    {
        [Required]
        public Guid UsuarioId { get; set; }

        [Required(ErrorMessage = "Selecione um ativo.")]
        public int AtivoId { get; set; }

        [Required(ErrorMessage = "Informe a quantidade.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        public List<SelectListItem>? Ativos { get; set; }

        public Operacao ToOperacao()
        {
            return new Operacao
            {
                UsuarioId = UsuarioId,
                AtivoId = AtivoId,
                Quantidade = Quantidade,
            };
        }

        public static OperacaoViewModel FromOperacao(Operacao operacao)
        {
            return new OperacaoViewModel
            {
                UsuarioId = operacao.UsuarioId,
                AtivoId = operacao.AtivoId,
                Quantidade = operacao.Quantidade,
            };
        }
    }
}
