using Corretora.Bussiness.Database;
using Corretora.Model.Models;
using Corretora.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Corretora.Web.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly CorretoraContext _context;
        private readonly ILogger<UsuarioController> _logger;

        public UsuarioController(CorretoraContext context, ILogger<UsuarioController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            try
            {
                int pageSize = 10;
                var usuarios = _context.Usuarios.OrderBy(u => u.Nome).Select(u => UsuarioViewModel.FromUsuario(u));
                return View(await ListaPaginada<UsuarioViewModel>.CreateAsync(usuarios.AsNoTracking(), pageNumber, pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        public IActionResult Create() => View("Form", new UsuarioViewModel());

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

                if (usuario == null)
                    return NotFound();

                return View("Form", UsuarioViewModel.FromUsuario(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel usuario)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Form", usuario);

                usuario.Id = Guid.NewGuid();
                _context.Usuarios.Add(usuario.ToUsuario());
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(UsuarioViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return View("Form", model);

                _context.Usuarios.Update(model.ToUsuario());
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
