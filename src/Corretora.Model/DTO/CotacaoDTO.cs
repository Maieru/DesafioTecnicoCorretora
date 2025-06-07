using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corretora.Model.DTO
{
    public class CotacaoDTO
    {
        public int AtivoId { get; set; }
        public decimal PrecoUnitario { get; set; }
        public Guid Identificador { get; set; }
    }
}
