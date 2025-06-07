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
    [Table("tbPosicoes")]
    public class Posicao
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

        [Required, Column("preco_medio", TypeName = "decimal(10,2)")]
        public decimal PrecoMedio { get; set; }

        [Required, Column("PL", TypeName = "decimal(12,2)")]
        public decimal PL { get; set; }
    }
}
