using Corretora.Bussiness.Database;
using Corretora.Model.Enums;
using Corretora.Model.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corretora.Bussiness.Services
{
    public class OperacaoService
    {
        private readonly CorretoraContext _context;

        public OperacaoService(CorretoraContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Recupera o total investido por ativo de um usuário, sem considerar as vendas.
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <returns>Um dicionário cujo a chave é o código do ativo e o valor é o total investido nesse ativo.</returns>
        public async Task<Dictionary<int, decimal>> GetTotalInvestidoPorAtivo(Guid usuarioId)
        {
            return await _context.Operacoes
                .Where(o => o.UsuarioId == usuarioId && o.TipoOperacao == TipoOperacao.Compra)
                .GroupBy(o => o.Ativo.Id)
                .Select(g => new
                {
                    Ativo = g.Key,
                    TotalInvestido = g.Sum(o => o.Quantidade * o.PrecoUnitario + o.Corretagem)
                })
                .ToDictionaryAsync(x => x.Ativo, x => x.TotalInvestido);
        }

        /// <summary>
        /// Recupera o total de vendas por ativo de um usuário.
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <returns>Um dicionário cujo a chave é o código do ativo e o valor é o total vendido nesse ativo.</returns>
        public async Task<Dictionary<int, decimal>> GetTotalVendasPorAtivo(Guid usuarioId)
        {
            return await _context.Operacoes
                .Where(o => o.UsuarioId == usuarioId && o.TipoOperacao == TipoOperacao.Venda)
                .GroupBy(o => o.Ativo.Id)
                .AsNoTracking()
                .Select(g => new
                {
                    Ativo = g.Key,
                    TotalVendido = g.Sum(o => o.Quantidade * o.PrecoUnitario + o.Corretagem)
                })
                .ToDictionaryAsync(x => x.Ativo, x => x.TotalVendido);
        }

        /// <summary>
        /// Recupera o total investido em um ativo de um usuário, sem considerar as vendas.
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <param name="codigoAtivo">O código do ativo sendo consultado.</param>
        /// <returns>O valor total investido nesse ativo.</returns>
        public async Task<decimal> GetTotalInvestidoPorAtivo(Guid usuarioId, string codigoAtivo)
        {
            return await _context.Operacoes
                .Include(o => o.Ativo)
                .Where(o => o.UsuarioId == usuarioId && o.TipoOperacao == TipoOperacao.Compra && o.Ativo.Codigo == codigoAtivo)
                .AsNoTracking()
                .SumAsync(o => o.Quantidade * o.PrecoUnitario + o.Corretagem);
        }

        /// <summary>
        /// Recupera o total de vendas de um ativo de um usuário.
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <param name="codigoAtivo">O código do ativo sendo consultado.</param>
        /// <returns>O valor total investido nesse ativo.</returns>
        public async Task<decimal> GetTotalVendasPorAtivo(Guid usuarioId, string codigoAtivo)
        {
            return await _context.Operacoes
                .Include(o => o.Ativo)
                .Where(o => o.UsuarioId == usuarioId && o.TipoOperacao == TipoOperacao.Venda && o.Ativo.Codigo == codigoAtivo)
                .AsNoTracking()
                .SumAsync(o => o.Quantidade * o.PrecoUnitario + o.Corretagem);
        }

        /// <summary>
        /// Recupera o total de corretagem de um usuário.
        /// </summary>
        /// <param name="usuarioId">O id do usuário a ser consultado.</param>
        /// <returns>O valor total de corretagem.</returns>
        public async Task<decimal> GetTotalCorretagem(Guid usuarioId)
        {
            return await _context.Operacoes
                .Where(o => o.UsuarioId == usuarioId)
                .AsNoTracking()
                .SumAsync(o => o.Corretagem);
        }

        /// <summary>
        /// Realiza a compra de um ativo.
        /// </summary>
        /// <param name="usuarioId">O id do usuário que está comprando o ativo</param>
        /// <param name="ativoId">O id do ativo sendo comprado</param>
        /// <param name="quantidade">A quantidade de ativos sendo comprados</param>
        /// <returns>A quantidade de registros inseridos</returns>
        public async Task<int> RealizaCompra(Guid usuarioId, int ativoId, int quantidade)
        {
            var cotacaoService = new CotacaoService(_context);
            var cotacaoAtual = await cotacaoService.GetCotacaoAtualAtivo(ativoId);
            var taxaCorretagemUsuario = await _context.Usuarios.Where(u => u.Id == usuarioId)
                .Select(u => u.PercCorretagem)
                .FirstOrDefaultAsync();

            var operacao = new Operacao()
            {
                UsuarioId = usuarioId,
                AtivoId = ativoId,
                Quantidade = quantidade,
                PrecoUnitario = cotacaoAtual,
                TipoOperacao = TipoOperacao.Compra
            };

            var corretagem = cotacaoAtual * quantidade * (taxaCorretagemUsuario / 100);
            operacao.Corretagem = Math.Round(corretagem, 2);
            operacao.DataHora = DateTime.UtcNow;

            // Não consigo usar o EF Core para inserir diretamente o registro com o .Add por causa da trigger do banco de dados.
            return await _context.Database.ExecuteSqlInterpolatedAsync($@"
                INSERT INTO tbOperacoes (usuario_id, ativo_id, quantidade, preco_unitario, tipo_operacao, corretagem, data_hora)
                VALUES ({operacao.UsuarioId}, {operacao.AtivoId}, {operacao.Quantidade}, {operacao.PrecoUnitario}, '0', {operacao.Corretagem}, {operacao.DataHora})");
        }
    }
}