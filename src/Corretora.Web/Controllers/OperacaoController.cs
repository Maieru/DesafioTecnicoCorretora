using Corretora.Bussiness.Database;
using Corretora.Bussiness.Services;
using Corretora.Model.Enums;
using Corretora.Model.Models;
using Corretora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Corretora.Web.Controllers
{
    public class OperacaoController : Controller
    {
        private readonly CorretoraContext _context;

        public OperacaoController(CorretoraContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Comprar(Guid usuarioId)
        {
            var ativos = await RecuperaOpcoesAtivos();

            var viewModel = new OperacaoViewModel
            {
                UsuarioId = usuarioId,
                Ativos = ativos
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Comprar(OperacaoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Ativos = await RecuperaOpcoesAtivos();
                return View(model);
            }

            var operacao = model.ToOperacao();

            var operacaoService = new OperacaoService(_context);
            await operacaoService.RealizaCompra(operacao.UsuarioId, operacao.AtivoId, operacao.Quantidade);

            return RedirectToAction("Index", "Usuario");
        }

        public async Task<IActionResult> ResumoPosicao(Guid usuarioId)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);

            if (usuario == null)
                return NotFound();

            var ativosEnvolvidos = new List<int>();
            var operacaoService = new OperacaoService(_context);

            var operacoesUsuario = await operacaoService.GetOperacoesUsuario(usuarioId);
            ativosEnvolvidos.AddRange(operacoesUsuario.Select(o => o.AtivoId));

            var totalCorretagem = await operacaoService.GetTotalCorretagem(usuarioId);

            var posicaoService = new PosicaoService(_context);
            var posicoesUsuario = await posicaoService.GetPosicaoPorPapel(usuarioId);
            ativosEnvolvidos.AddRange(posicoesUsuario.Select(p => p.AtivoId));

            var resultadoGlobal = await posicaoService.GetResultadoGlobal(usuarioId);

            var ativos = await _context.Ativos
                .Where(a => ativosEnvolvidos.Contains(a.Id))
                .ToListAsync();

            var viewModel = new ResumoComprasViewModel();
            viewModel.NomeUsuario = usuario.Nome;
            viewModel.TotalCorretagem = totalCorretagem;
            viewModel.ResultadoGlobal = resultadoGlobal;

            foreach (var ativo in ativos)
            {
                var posicaoAtivo = posicoesUsuario.FirstOrDefault(p => p.AtivoId == ativo.Id);
                var operacoesAtivo = operacoesUsuario.Where(o => o.AtivoId == ativo.Id).ToList();

                var operacoesCompraAtivo = operacoesAtivo.Where(o => o.TipoOperacao == TipoOperacao.Compra).ToList();
                var operacoesVendaAtivo = operacoesAtivo.Where(o => o.TipoOperacao == TipoOperacao.Venda).ToList();

                var dadosDoAtivo = new DadosPorAtivoViewModel
                {
                    NomeAtivo = ativo.Nome,
                    CodigoAtivo = ativo.Codigo,
                    TotalInvestido = operacoesCompraAtivo?.Sum(o => o.Quantidade * o.PrecoUnitario + o.Corretagem) ?? 0,
                    TotalVendido = operacoesVendaAtivo?.Sum(o => o.Quantidade * o.PrecoUnitario + o.Corretagem) ?? 0,
                };

                if (posicaoAtivo != null)
                {
                    dadosDoAtivo.QuantidadeTotal = posicaoAtivo.Quantidade;

                    // Daria para utilizar o dado de preço médio da posição, mas quero usar o método de calculo do preço médio
                    dadosDoAtivo.PrecoMedio = operacaoService.CalculaPrecoMedioDasCompras(operacoesCompraAtivo) ?? 0;
                    dadosDoAtivo.Resultado = posicaoAtivo.PL;
                }

                viewModel.DadosAtivos.Add(dadosDoAtivo);
            }


            return View(viewModel);
        }

        private async Task<List<SelectListItem>> RecuperaOpcoesAtivos()
        {
            return await _context.Ativos.Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = a.Codigo
            }).ToListAsync();
        }
    }
}
