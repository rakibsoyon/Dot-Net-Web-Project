using Microsoft.EntityFrameworkCore;

namespace Shared
{
    public class PagedList<T>
    {
        private PagedList(List<T> data, int page, int pageSize, int totalRecords,int totalPages)
        {
            Data = data;
            Page = page;
            PageSize = pageSize;
            TotalRecords = totalRecords;
            TotalPages = totalPages;
        }
        public List<T> Data { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalRecords { get; init; }
        public bool HasNextPage => Page * PageSize < TotalRecords;
        public bool HasPreviousPage => Page > 1;
        public int TotalPages { get; set; }


        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> query, int page, int pageSize)
        {
            var totalCount = await query.CountAsync();

            var data = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            return new(data, page, pageSize, totalCount, totalPages);
        }
    }

    public class PagedListDTO<T>
    {
        public List<T> Data { get; init; } = [];
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalRecords { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
        public int TotalPages { get; set; }
    }
}
