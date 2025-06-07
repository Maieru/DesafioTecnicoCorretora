namespace Corretora.Web.Models
{
    public class ResumoComprasViewModel
    {
        public string NomeUsuario { get; set; }
        public List<DadosPorAtivoViewModel> DadosAtivos { get; set; } = new List<DadosPorAtivoViewModel>();
        public decimal ResultadoGlobal { get; set; }
        public decimal TotalCorretagem { get; set; }
    }

    public class DadosPorAtivoViewModel
    {
        public string CodigoAtivo { get; set; }
        public string NomeAtivo { get; set; }
        public decimal TotalInvestido { get; set; }
        public decimal TotalVendido { get; set; }
        public int QuantidadeTotal { get; set; }
        public decimal PrecoMedio { get; set; }
        public decimal Resultado { get; set; }
    }
}
