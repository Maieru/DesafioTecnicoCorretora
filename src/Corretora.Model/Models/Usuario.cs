using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corretora.Model.Models
{
    [Table("tbUsuarios")]
    public class Usuario
    {
        [Key, Column("id")]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required, MaxLength(150), Column("nome")]
        public string Nome { get; set; }

        [Required, MaxLength(200), Column("email")]
        public string Email { get; set; }

        [Required, Column("perc_corretagem", TypeName = "decimal(5,2)")]
        public decimal PercCorretagem { get; set; }
    }
}
