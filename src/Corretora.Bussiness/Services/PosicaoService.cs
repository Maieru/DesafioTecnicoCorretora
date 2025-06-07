using Corretora.Bussiness.Database;
using Corretora.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corretora.Bussiness.Services
{
    public class PosicaoService 
    {
        private readonly CorretoraContext _context;

        public PosicaoService(CorretoraContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recupera cada uma das posições de um usuário. 
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <returns>Uma lista de posições do usuário, com os seus respectivos papeis.</returns>
        public async Task<List<Posicao>> GetPosicaoPorPapel(Guid usuarioId)
        {
            return await _context.Posicoes
                .Include(p => p.Ativo)
                .Where(p => p.UsuarioId == usuarioId)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// Recupera o resultado total de um usuário. 
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <returns>O resultado do usuário.</returns>
        public async Task<decimal> GetResultadoGlobal(Guid usuarioId)
        {
            return await _context.Posicoes
                .Where(p => p.UsuarioId == usuarioId)
                .AsNoTracking()
                .SumAsync(p => p.PL);
        }
    }
}
