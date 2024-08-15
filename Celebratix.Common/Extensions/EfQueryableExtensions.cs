using System.Linq.Expressions;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Extensions;

public static class EfQueryableExtensions
{
    // If T can't be the dto directly (e.g. because the constructor is too advanced),
    // this should be refactored to allow returning non dto results and letting the dto take the non dto result
    public static async Task<PagedResultDto<T>> ToPagedResult<T>(this IQueryable<T> query, int page, int pageSize)
        where T : class
    {
        pageSize = Math.Clamp(pageSize, 1, 1024);
        page = Math.Max(page, 1);
        var result = new PagedResultDto<T>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = await query.CountAsync()
        };

        result.PageCount = result.RowCount / pageSize;
        if (result.RowCount % pageSize != 0)
            result.PageCount++;

        var skip = (page - 1) * pageSize;
        result.List = await query.Skip(skip).Take(pageSize).ToListAsync();

        return result;
    }

    public static async Task<bool> AnyOrThrowAsync<T>(this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate = null) where T : class
    {
        return await query.AnyOrThrowAsync<T, object>(predicate, null);
    }

    public static async Task<bool> AnyOrThrowAsync<T, TId>(this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate, TId? logId) where T : class
    {
        bool exists;

        if (predicate != null)
            exists = await query.AnyAsync(predicate);
        else
            exists = await query.AnyAsync();

        if (exists) return exists;
        if (logId != null)
            throw new ObjectNotFoundException($"No {typeof(T).Name} with id {logId} was found!");

        throw new ObjectNotFoundException($"No {typeof(T).Name} for the supplied query was found!");

    }

    public static async Task<T> FirstOrThrowAsync<T>(this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate = null)
    {
        return await query.FirstOrThrowAsync<T, object>(predicate, null);
    }

    public static async Task<T> FirstOrThrowAsync<T, TId>(this IQueryable<T> query,
        Expression<Func<T, bool>>? predicate, TId? logId)
    {
        T? dbModel;

        if (predicate != null)
            dbModel = await query.FirstOrDefaultAsync(predicate);
        else
            dbModel = await query.FirstOrDefaultAsync();

        if (dbModel == null)
        {
            if (logId != null)
                throw new ObjectNotFoundException($"No {typeof(T).Name} with id {logId} was found!");

            throw new ObjectNotFoundException($"No {typeof(T).Name} for the supplied query was found!");
        }

        return dbModel;
    }

    public static async Task<IEnumerable<TResult>> SelectAsync<TSource, TResult>(
        this IEnumerable<TSource> source, Func<TSource, Task<TResult>> method,
        int concurrency = 1)
    {
        var semaphore = new SemaphoreSlim(concurrency);
        try
        {
            return await Task.WhenAll(source.Select(async s =>
            {
                try
                {
                    await semaphore.WaitAsync();
                    return await method(s);
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }
        finally
        {
            semaphore.Dispose();
        }
    }
}
