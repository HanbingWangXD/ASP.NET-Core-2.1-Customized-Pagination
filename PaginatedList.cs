using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MinerCoreApplication

{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int TotalItems { get; private set; }
        public int ItemOnLastPage { get; private set; }


        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, int ItemOnLastPage)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            if (TotalPages == 0)
            {
                TotalItems = 0;
            }
            else
            { TotalItems = pageSize * (TotalPages - 1) + ItemOnLastPage; }
            


            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var ItemOnLastPage = source.Count() % pageSize;
            return new PaginatedList<T>(items, count, pageIndex, pageSize, ItemOnLastPage);
        }
    }
}