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
    [Table("tbCotacoes")]
    public class Cotacao
    {
        [Key, Column("id")]
        public int Id { get; set; }


        [Required, ForeignKey("ativo_id"), Column("ativo_id")]
        public int AtivoId { get; set; }
        public Ativo Ativo { get; set; }


        [Required, Column("preco_unitario", TypeName = "decimal(10,2)")]
        public decimal PrecoUnitario { get; set; }

        [Required, Column("data_hora")]
        public DateTime DataHora { get; set; }
    }
}
