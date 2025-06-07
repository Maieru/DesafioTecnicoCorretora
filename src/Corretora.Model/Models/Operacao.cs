using Corretora.Model.Enums;
using Corretora.Model.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corretora.Model.Models
{
    [Table("tbOperacoes")]
    public class Operacao
    {
        [Key, Column("id")]
        public int Id { get; set; }


        [Required, ForeignKey("usuario_id"), Column("usuario_id")]
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; }


        [Required, ForeignKey("ativo_id"), Column("ativo_id")]
        public int AtivoId { get; set; }
        public Ativo Ativo { get; set; }


        [Required, Column("quantidade")]
        public int Quantidade { get; set; }

        [Required, Column("preco_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [Required, Column("tipo_operacao")]
        public TipoOperacao TipoOperacao { get; set; }

        [Required, Column("corretagem", TypeName = "decimal(10,2)")]
        public decimal Corretagem { get; set; }

        [Required, Column("data_hora")]
        public DateTime DataHora { get; set; }
    }
}
