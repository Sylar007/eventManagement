using Celebratix.Common.DbSeeding.CustomSeeding;
using Celebratix.Common.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Celebratix.Common.Models;

public class CelebratixDbContext : IdentityDbContext<ApplicationUser>
{
    /// <summary>
    /// Contains both enabled and disabled countries
    /// </summary>
    public DbSet<Country> Countries { get; set; } = null!;

    /// <summary>
    /// All enabled countries. This should (by default) be used to verify if a supplied country is valid
    /// </summary>
    public IQueryable<Country> EnabledCountries => Countries.Where(c => c.Enabled);

    /// <summary>
    /// Contains both enabled and disabled currencies
    /// </summary>
    public DbSet<Currency> Currencies { get; set; } = null!;

    /// <summary>
    /// All enabled currencies. This should (by default) be used to list currencies
    /// </summary>
    public IQueryable<Currency> EnabledCurrencies => Currencies.Where(c => c.Enabled);

    public DbSet<ImageDbModel> Images { get; set; } = null!;

    public DbSet<Business> Businesses { get; set; } = null!;

    public DbSet<Category> Categories { get; set; } = null!;

    public DbSet<Event> Events { get; set; } = null!;

    /// <summary>
    /// All visible events. This should (by default) be used to both fetch and list events
    /// </summary>
    public IQueryable<Event> VisibleEvents => Events.Where(c => c.Visible);

    public DbSet<EventTicketType> EventTicketTypes { get; set; } = null!;

    public DbSet<MarketplaceListing> MarketplaceListings { get; set; } = null!;

    public DbSet<TicketTransferOffer> TicketTransferOffers { get; set; } = null!;

    public DbSet<Ticket> Tickets { get; set; } = null!;

    public DbSet<Channel> Channels { get; set; } = null!;

    public DbSet<ChannelEvent> ChannelEvents { get; set; } = null!;

    public DbSet<ChannelEventTicketType> ChannelEventTicketTypes { get; set; } = null!;

    public DbSet<Tracking> Trackings { get; set; } = null!;

    public DbSet<Transaction> Transactions { get; set; } = null!;

    public DbSet<Order> Orders { get; set; } = null!;

    public DbSet<Affiliate> Affiliates { get; set; } = null!;

    public DbSet<BusinessPayout> BusinessPayouts { get; set; } = null!;

    public DbSet<PayoutAccount> PayoutAccounts { get; set; } = null!;

    public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

    public CelebratixDbContext(DbContextOptions<CelebratixDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Used to seed database with custom seeding logic.
    /// Running this every start up is not a good idea
    /// Can be used for e.g. unit tests and manual testing
    ///
    /// This method should be run from e.g. Startup.Configure
    /// </summary>
    public static void SeedCustomData(IServiceProvider serviceProvider, bool development)
    {
        using var serviceScope = serviceProvider.CreateScope();
        var context = serviceScope.ServiceProvider.GetService<CelebratixDbContext>();
        var userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>();

        if (development)
        {
            new BaseSeed(context!, userManager!).Seed();
            new DevelopmentSeed(context!, userManager!).Seed();
        }
        else
        {
            new BaseSeed(context!, userManager!).Seed();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // We want to be able to use MARS and are not using SavePoints anyway
        optionsBuilder.ConfigureWarnings(w => w.Ignore(SqlServerEventId.SavepointsDisabledBecauseOfMARS));
        //optionsBuilder.EnableSensitiveDataLogging();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCustomPrimaryKeys(modelBuilder);
        ConfigureCustomRelations(modelBuilder);
        ConfigureCustomConstraints(modelBuilder);
        ConfigureCustomSerialization(modelBuilder);
        ConfigureCustomIndices(modelBuilder);
        ConfigureCustomDiscriminators(modelBuilder);
        ConfigureCustomDeletionBehaviour(modelBuilder);
        ConfigureGlobalFilters(modelBuilder);
        SeedMigrationData(modelBuilder);
    }

    private static void ConfigureCustomPrimaryKeys(ModelBuilder builder)
    {
    }

    private static void ConfigureCustomRelations(ModelBuilder builder)
    {
        builder.Entity<ApplicationUser>()
            .HasMany(u => u.Roles)
            .WithMany(nameof(Users))
            .UsingEntity<IdentityUserRole<string>>(
                userRole => userRole.HasOne<IdentityRole>()
                    .WithMany()
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired(),
                userRole => userRole.HasOne<ApplicationUser>()
                    .WithMany()
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired());

        builder.Entity<Event>()
            .HasMany(e => e.Channels)
            .WithMany(t => t.Events)
            .UsingEntity<ChannelEvent>(
                channelEvent =>
                {
                    channelEvent.HasOne(t => t.Event)
                        .WithMany(t => t.ChannelEvents)
                        .HasForeignKey(t => t.EventId);
                    channelEvent.HasKey(t => new { t.Id });
                });

    }

    private static void ConfigureCustomConstraints(ModelBuilder builder)
    {
    }

    private static void ConfigureCustomDiscriminators(ModelBuilder builder)
    {
    }

    private static void ConfigureCustomSerialization(ModelBuilder builder)
    {
    }

    private static void ConfigureCustomIndices(ModelBuilder builder)
    {
    }

    private static void ConfigureCustomDeletionBehaviour(ModelBuilder builder)
    {
        // Needed to break up delete cycle: Currency -> Event -> ... -> MarketplaceListing && Currency -> MarketplaceListing
        builder.Entity<Currency>()
            .HasMany<MarketplaceListing>()
            .WithOne(t => t.Currency)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<EventTicketType>()
            .HasMany<Order>()
            .WithOne(t => t.TicketType)
            .OnDelete(DeleteBehavior.NoAction);
    }

    private static void ConfigureGlobalFilters(ModelBuilder builder)
    {
    }

    private static void SeedMigrationData(ModelBuilder builder)
    {
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        SetCreatedAndUpdatedAt();
        return await base.SaveChangesAsync(true, cancellationToken);
    }

    public override int SaveChanges()
    {
        SetCreatedAndUpdatedAt();
        return base.SaveChanges();
    }

    private void SetCreatedAndUpdatedAt()
    {
        var currentTime = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State != EntityState.Modified && entry.State != EntityState.Added) continue;
            if (entry.Entity is not DbModelBase dbModelEntry) continue;
            dbModelEntry.UpdatedAt = currentTime;
            // We need to compare against minvalue to see if the datetime has been manually assigned
            // (as that is desirable to do in some scenarios)
            if (entry.State == EntityState.Added
                && dbModelEntry.CreatedAt == DateTimeOffset.MinValue)
            {
                dbModelEntry.CreatedAt = currentTime;
            }
        }
    }
}
