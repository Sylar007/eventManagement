using AutoFixture;
using Celebratix.Common.Models;
using Celebratix.Tests.SpecimenBuilders;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Celebratix.Tests.Fixtures;

public sealed class DatabaseContextFixture
{
    private readonly IServiceProvider _provider;
    private readonly Fixture _fixture;

    public DatabaseContextFixture(IServiceProvider provider)
    {
        _provider = provider;
        _fixture = new Fixture();
        _fixture.AddEntitySpecimenBuilders();
    }

    public DatabaseContextFixture EnsureWith<TEntity>(Func<TEntity, int> id, params TEntity[] events)
    where TEntity : class
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CelebratixDbContext>();

        foreach (var @event in events)
        {
            if (context.Find<TEntity>(id.Invoke(@event)) == null)
            {
                context.Set<TEntity>().Add(@event);
            }
        }

        context.SaveChanges();

        return this;
    }

    public DatabaseContextFixture With<TEntity>(params TEntity[] entities)
        where TEntity : class
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CelebratixDbContext>();

        context.Set<TEntity>().AddRange(entities);

        context.SaveChanges();

        return this;
    }

    public void Has<TEntity>(Func<TEntity, bool> predicate, int count)
        where TEntity : class
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CelebratixDbContext>();

        context.Set<TEntity>()
            .AsEnumerable()
            .Count(predicate.Invoke)
            .Should().Be(count);
    }

    public void HasOne<TEntity>(Func<TEntity, bool> predicate)
        where TEntity : class
    {
        Has(predicate, 1);
    }

    public void HasOne<TEntity, TChild>(
        Expression<Func<TEntity, IReadOnlyList<TChild>>> expression,
        Func<TChild, bool> predicate)
        where TEntity : class
        where TChild : class
    {
        using var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<CelebratixDbContext>();

        context.Set<TEntity>()
            .Include(expression)
            .AsEnumerable()
            .SelectMany(expression.Compile())
            .Count(predicate.Invoke)
            .Should().Be(1);
    }
}