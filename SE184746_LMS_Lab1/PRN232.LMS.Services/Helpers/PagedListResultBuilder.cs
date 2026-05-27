using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Services.Models.Common;
using PRN232.LMS.Services.Models.Requests;

namespace PRN232.LMS.Services.Helpers;

public static class PagedListResultBuilder
{
    public static async Task<PagedResult<object>> CreateAsync<TEntity>(
        PagedListRequest request,
        IQueryable<TEntity> query,
        Func<IQueryable<TEntity>, IQueryable<TEntity>> applyFilter,
        Func<IQueryable<TEntity>, IReadOnlyList<string>, IQueryable<TEntity>> applySort,
        Func<TEntity, IReadOnlyList<string>, object> mapItem,
        Func<object, IReadOnlyList<string>, object> shapeItem,
        CancellationToken cancellationToken = default)
    {
        var expand = request.GetExpandTokens();
        var fields = request.GetFieldTokens();
        var (page, size) = request.GetPaging();

        query = applyFilter(query);
        query = applySort(query, request.GetSortTokens());

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalItems / (double)size);

        var entities = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        var items = entities
            .Select(entity => mapItem(entity, expand))
            .Select(item => fields.Count > 0 ? shapeItem(item, fields) : item)
            .Cast<object>()
            .ToList();

        return new PagedResult<object>
        {
            Items = items,
            Pagination = new PaginationMetadata
            {
                Page = page,
                PageSize = size,
                TotalItems = totalItems,
                TotalPages = totalPages
            }
        };
    }
}
