using Corretora.Bussiness.Database;
using Corretora.Bussiness.Services;
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
