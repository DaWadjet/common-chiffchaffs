using CSONGE.Application.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSONGE.Application.Extensions
{
    public static class IEnumerableExtensions
    {
        public async static Task<IPagedList<T>> ToPagedListAsync<T>(this IEnumerable<T> source, int pageIndex, int pageSize) 
            => await PagedList<T>.ToPagedListAsync(source, pageIndex, pageSize);
    }
}
