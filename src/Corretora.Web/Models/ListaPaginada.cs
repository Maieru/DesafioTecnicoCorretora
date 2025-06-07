using Microsoft.EntityFrameworkCore;

namespace Corretora.Web.Models
{
    public class ListaPaginada<T> : List<T>
    {
        public int NumeroPagina { get; private set; }
        public int NumeroPaginas { get; private set; }

        public ListaPaginada(List<T> items, int count, int pageIndex, int pageSize)
        {
            NumeroPagina = pageIndex;
            NumeroPaginas = (int)Math.Ceiling(count / (double)pageSize);
            this.AddRange(items);
        }

        public bool HasPreviousPage => NumeroPagina > 1;
        public bool HasNextPage => NumeroPagina < NumeroPaginas;

        public static async Task<ListaPaginada<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new ListaPaginada<T>(items, count, pageIndex, pageSize);
        }
    }
}
