using Corretora.Bussiness.Database;
using Corretora.Model.Models;
using Corretora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Corretora.Web.Controllers
{
    public class AtivoController : Controller
    {
        private readonly CorretoraContext _context;

        public AtivoController(CorretoraContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 10;
            var usuarios = _context.Ativos.OrderBy(u => u.Nome).Select(a => AtivoViewModel.FromAtivo(a));
            return View(await ListaPaginada<AtivoViewModel>.CreateAsync(usuarios.AsNoTracking(), pageNumber, pageSize));
        }

        public IActionResult Create() => View("Form", new AtivoViewModel());

        public async Task<IActionResult> Edit(int id)
        {
            var ativo = await _context.Ativos.FirstOrDefaultAsync(a => a.Id == id);

            if (ativo == null)
                return NotFound();

            var viewModel = AtivoViewModel.FromAtivo(ativo);
            return View("Form", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AtivoViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Form", model);

            var ativo = model.ToAtivo();
            _context.Ativos.Add(ativo);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AtivoViewModel model)
        {
            if (!ModelState.IsValid) 
                return View("Form", model);

            _context.Update(model.ToAtivo());
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
