using System.Collections.Generic;

namespace CSONGE.Application.Pagination
{
    public interface IPagedList<T>
    {
        int PageIndex { get; }
        int PageSize { get; }
        int PageCount { get; }
        long ItemCount { get; }
        IList<T> Items { get; }
    }
}
