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

        public UsuarioController(CorretoraContext context) => _context = context;

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            int pageSize = 10;

            var teste = _context.Ativos.FirstOrDefault();
            var teste2 = _context.Cotacoes.FirstOrDefault();
            var teste3 = _context.Operacoes.FirstOrDefault();
            var teste4 = _context.Posicoes.FirstOrDefault();

            var usuarios = _context.Usuarios.OrderBy(u => u.Nome).Select(u => UsuarioViewModel.FromUsuario(u));
            return View(await ListaPaginada<UsuarioViewModel>.CreateAsync(usuarios.AsNoTracking(), pageNumber, pageSize));
        }

        public IActionResult Create() => View("Form", new UsuarioViewModel());

        public async Task<IActionResult> Edit(Guid id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
                return NotFound();

            return View("Form", UsuarioViewModel.FromUsuario(usuario));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UsuarioViewModel usuario)
        {
            if (!ModelState.IsValid)
                return View("Form", usuario);

            usuario.Id = Guid.NewGuid();
            _context.Usuarios.Add(usuario.ToUsuario());
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UsuarioViewModel usuario)
        {
            if (!ModelState.IsValid)
                return View("Form", usuario);

            _context.Usuarios.Update(usuario.ToUsuario());
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}
