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
    public class CotacaoService
    {
        private readonly CorretoraContext _context;

        public CotacaoService(CorretoraContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna a cotação atual de um ativo específico.
        /// </summary>
        /// <param name="ativoId">Ativo sendo consultado</param>
        /// <returns>Valor da cotação atual</returns>
        /// <exception cref="InvalidOperationException">Será lançada somente se o ativo sendo pesquisado não tiver uma cotação especificada.</exception>
        public async Task<decimal> GetCotacaoAtualAtivo(int ativoId)
        {
            var cotacao = await _context.Cotacoes
                .Where(c => c.AtivoId == ativoId)
                .OrderByDescending(c => c.DataHora)
                .FirstOrDefaultAsync();

            if (cotacao == null)
                throw new InvalidOperationException("Cotação não encontrada para o ativo especificado.");

            return cotacao.PrecoUnitario;
        }

        /// <summary>
        /// Salva uma nova cotação para um ativo específico.
        /// </summary>
        /// <param name="novaCotacao">Novo valor da cotação</param>
        /// <param name="ativoId">Id do ativo</param>
        /// <returns>A quantidade de registros inseridos</returns>
        public async Task<int> SalvaCotacao(decimal novaCotacao, int ativoId)
        {
            // Não consigo usar o EF Core para inserir diretamente o registro com o .Add por causa da trigger do banco de dados.
            return await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO tbCotacoes (ativo_id, preco_unitario, data_hora)
                VALUES ({ativoId}, {novaCotacao}, {DateTime.UtcNow})");
        }
    }
}
