using Corretora.Model.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Isso acontece porque a tabela de operações possui uma trigger. 
            // O Entity Framework Core não consegue lidar com isso diretamente, então precisamos informar que o Id não deve ser atualizado após a inserção.
            // Caso se torne um problema, mudarei a lógica de inserção para utilizar uma storaged procedure.
            modelBuilder.Entity<Operacao>()
                .Property(o => o.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
