using Corretora.Bussiness.Database;
using Corretora.Bussiness.Services;
using Corretora.Model.Models;
using Corretora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Corretora.Web.Controllers
{
    public class AtivoController : Controller
    {
        private readonly CorretoraContext _context;
        private readonly ILogger<AtivoController> _logger;

        public AtivoController(CorretoraContext context, ILogger<AtivoController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;
                var usuarios = _context.Ativos.OrderBy(u => u.Nome).Select(a => AtivoViewModel.FromAtivo(a));
                return View(await ListaPaginada<AtivoViewModel>.CreateAsync(usuarios.AsNoTracking(), pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        public IActionResult Create() => View("Form", new AtivoViewModel());

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var ativo = await _context.Ativos.FirstOrDefaultAsync(a => a.Id == id);

                if (ativo == null)
                    return NotFound();

                var viewModel = AtivoViewModel.FromAtivo(ativo);
                var cotacaoService = new CotacaoService(_context);
                viewModel.CotacaoAtual = await cotacaoService.GetCotacaoAtualAtivo(ativo.Id);

                return View("Form", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtivoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Form", model);

                var ativo = model.ToAtivo();
                _context.Ativos.Add(ativo);
                await _context.SaveChangesAsync();

                var cotacaoService = new CotacaoService(_context);
                await cotacaoService.SalvaCotacao(model.CotacaoAtual, ativo.Id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AtivoViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Form", model);

                _context.Update(model.ToAtivo());
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }
    }
}
