using Corretora.Model.Models;
using Microsoft.EntityFrameworkCore;

namespace Corretora.Bussiness.Database
{
    public class CorretoraContext : DbContext
    {
        public CorretoraContext(DbContextOptions<CorretoraContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Ativo> Ativos { get; set; }
        public DbSet<Cotacao> Cotacoes { get; set; }
        public DbSet<Operacao> Operacoes { get; set; }
        public DbSet<Posicao> Posicoes { get; set; }
    }
}
