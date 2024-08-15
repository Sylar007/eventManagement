namespace Celebratix.Tests;

using System.Data.Common;
using Celebratix.Common.Database;
using Celebratix.Common.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

public class UnitOfWorkTests
{
    private readonly DbConnection _connection;
    private readonly DbContextOptions<CelebratixDbContext> _contextOptions;

    [Index(nameof(Name), IsUnique = true)]
    public class TestEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public UnitOfWorkTests()
    {
        _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();

        var builder = new ModelBuilder();
        var c = builder.Entity<TestEntity>();
        c.Property<int>("Id")
            .HasColumnType("int");
        c.Property<string>("Name")
            .HasColumnType("varchar");
        c.HasKey("Id");
        var model = builder.FinalizeModel();

        _contextOptions = new DbContextOptionsBuilder<CelebratixDbContext>()
            .UseSqlite(_connection)
            .UseModel(model) // Use custom model, because using the real model is too much of a hassle
            .Options;

        using var context = new CelebratixDbContext(_contextOptions);
        if (context.Database.EnsureCreated())
        {
            // Database just got created
        }
        context.AddRange(
                new TestEntity { Id = 1, Name = "1" },
                new TestEntity { Id = 2, Name = "2" },
                new TestEntity { Id = 3, Name = "3" });
        context.SaveChanges();
    }

    CelebratixDbContext CreateContext() => new CelebratixDbContext(_contextOptions);

    [Fact]
    public async void CommitAtComplete()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
        {
            var foo = TestEntities.First();
            EntitySet.Remove(foo);

            transactionScope.Complete(); // The top level complete should commit the transaction

            var bar = TestEntities.First();
            Assert.NotEqual(foo, bar);
        }
    }

    [Fact]
    public async void NoException()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
        {
            var foo = TestEntities.First();
            EntitySet.Remove(foo);

            using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
            {
                var bar = TestEntities.First();
                EntitySet.Remove(bar);

                transactionScope2.Complete();
            }

            transactionScope.Complete();
        }
        Assert.Equal(entityCount - 2, EntitySet.Count());
    }

    [Fact]
    public async void NoCatchInnerException()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        try
        {
            using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
            {
                var foo = TestEntities.First();
                EntitySet.Remove(foo);

                using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    throw new Exception();

#pragma warning disable 0162
                    transactionScope2.Complete();
#pragma warning restore 0162
                }

                transactionScope.Complete();
            }
        }
        catch (Exception)
        {
        }
        Assert.Equal(entityCount, EntitySet.Count());
    }

    [Fact]
    public async void CatchInnerException()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
        {
            var foo = TestEntities.First();
            EntitySet.Remove(foo);

            try
            {
                using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    throw new Exception();

#pragma warning disable 0162
                    transactionScope2.Complete();
#pragma warning restore 0162
                }
            }
            catch (Exception)
            {
            }

            transactionScope.Complete();
        }
        Assert.Equal(entityCount - 1, EntitySet.Count());
    }

    [Fact]
    public async void OuterException()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        try
        {
            using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
            {
                var foo = TestEntities.First();
                EntitySet.Remove(foo);

                using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    transactionScope2.Complete();
                }

                throw new Exception();

#pragma warning disable 0162
                transactionScope.Complete();
#pragma warning restore 0162
            }
        }
        catch (Exception)
        {
        }
        Assert.Equal(entityCount, EntitySet.Count());
    }

    [Fact]
    public async void NoExceptionAsync()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        await using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
        {
            var foo = TestEntities.First();
            EntitySet.Remove(foo);

            await using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
            {
                var bar = TestEntities.First();
                EntitySet.Remove(bar);

                transactionScope2.Complete();
            }

            transactionScope.Complete();
        }
        Assert.Equal(entityCount - 2, EntitySet.Count());
    }

    [Fact]
    public async void NoCatchInnerExceptionAsync()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        try
        {
            await using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
            {
                var foo = TestEntities.First();
                EntitySet.Remove(foo);

                await using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    throw new Exception();

#pragma warning disable 0162
                    transactionScope2.Complete();
#pragma warning restore 0162
                }

                transactionScope.Complete();
            }
        }
        catch (Exception)
        {
        }
        Assert.Equal(entityCount, EntitySet.Count());
    }

    [Fact]
    public async void CatchInnerExceptionAsync()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        await using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
        {
            var foo = TestEntities.First();
            EntitySet.Remove(foo);

            try
            {
                await using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    throw new Exception();

#pragma warning disable 0162
                    transactionScope2.Complete();
#pragma warning restore 0162
                }
            }
            catch (Exception)
            {
            }

            transactionScope.Complete();
        }
        Assert.Equal(entityCount - 1, EntitySet.Count());
    }

    [Fact]
    public async void OuterExceptionAsync()
    {
        using var context = CreateContext();
        var _unitOfWork = new UnitOfWork(context);
        var EntitySet = _unitOfWork.DbContext.Set<TestEntity>();

        var TestEntities = EntitySet.OrderBy(c => c.Id);
        var entityCount = TestEntities.Count();

        try
        {
            await using (var transactionScope = await _unitOfWork.CreateTransactionAsync())
            {
                var foo = TestEntities.First();
                EntitySet.Remove(foo);

                await using (var transactionScope2 = await _unitOfWork.CreateTransactionAsync())
                {
                    var bar = TestEntities.First();
                    EntitySet.Remove(bar);

                    transactionScope2.Complete();
                }

                throw new Exception();

#pragma warning disable 0162
                transactionScope.Complete();
#pragma warning restore 0162
            }
        }
        catch (Exception)
        {
        }
        Assert.Equal(entityCount, EntitySet.Count());
    }
}
