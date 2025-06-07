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
    [Table("tbAtivos")]
    public class Ativo
    {
        [Key, Column("id")]
        public int Id { get; set; }

        [Required, MaxLength(10), Column("codigo")]
        public string Codigo { get; set; }

        [Required, MaxLength(100), Column("nome")]
        public string Nome { get; set; }
    }
}
