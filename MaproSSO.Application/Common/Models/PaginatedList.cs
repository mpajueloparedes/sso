﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MaproSSO.Application.Common.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items = items;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public int FirstItemIndex => (PageIndex - 1) * PageSize + 1;
        public int LastItemIndex => Math.Min(PageIndex * PageSize, TotalCount);

        public static async Task<PaginatedList<T>> CreateAsync(
            IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }

        public static PaginatedList<T> Create(
            IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var enumerable = source as List<T> ?? source.ToList();
            var count = enumerable.Count;
            var items = enumerable.Skip((pageIndex - 1) * pageSize)
                                  .Take(pageSize)
                                  .ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
