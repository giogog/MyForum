using Microsoft.EntityFrameworkCore;

namespace Domain.Models;

public class PagedList<T> : List<T>
{
    public PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
    {
        SelectedPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        ItemCount = count;
        PageSize = pageSize;
        AddRange(items);
    }

    public int SelectedPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int ItemCount { get; set; }

    public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize)
    {
        var count = await source.CountAsync();
        var items  = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        if (!items.Any())
        {
            throw new NotFoundException("Items Not Found");
        }
        return new PagedList<T>(items, count, pageNumber, pageSize);
    }

}