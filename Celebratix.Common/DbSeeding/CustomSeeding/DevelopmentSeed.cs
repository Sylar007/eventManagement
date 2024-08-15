using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Celebratix.Common.DbSeeding.CustomSeeding;

public class DevelopmentSeed
{

    #region Business constants

    private const string BusinessName = "uBit";
    private const decimal BusinessDefaultVat = 0.09M;

    #endregion

    #region User constants

    // Password is meant to be changed on the live environment manually.
    // Therefore the lack of a serious password here or fetching of password from appsettings
    private const string BusinessUserEmail = "business@ubit.nu";
    private const string BusinessUserPassword = "Password123!";
    private const string BusinessUserGivenName = "Business";
    private const string BusinessUserFamilyName = "Businesson";

    private const string TicketPurchaseUserEmail = "guest@ubit.nu";
    private const string TicketPurchaseUserPassword = "Password123!";
    private const string TicketPurchaseUserGivenName = "Guest";
    private const string TicketPurchaseUserFamilyName = "Guestsson";

    #endregion

    #region Currency constants

    private const string CurrencyCode = "SEK";
    private const string CurrencyName = "crowns";
    private const string CurrencySymbol = "SEK";
    private const bool CurrencyEnabled = true;
    private const int CurrencyDecimalPlaces = 2;

    #endregion

    #region TicketType constants
    private const decimal TicketTypePrice = 10;
    private const decimal TicketTypeVat = 0.09m;
    #endregion

    #region Event constants
    private const int AmountOfSeededEvents = 15;
    #endregion

    #region Category constants

    private ICollection<string> Categories = new List<string>() { "festival", "business", "other" };

    #endregion

    #region DateVaribles
    private int Year = 2023;
    private int Month = 1;
    #endregion
    protected readonly CelebratixDbContext DbContext;
    protected readonly UserManager<ApplicationUser> UserManager;

    protected ApplicationUser BusinessUser = null!;
    protected ApplicationUser TicketPurchaseUser = null!;
    protected Business Business = null!;
    protected Channel Channel = null!;
    protected Currency BusinessCurrency = null!;

    protected List<Event> Events = new List<Event>();
    protected List<ChannelEvent> ChannelEvents = new List<ChannelEvent>();
    protected List<EventTicketType> TicketTypes = new List<EventTicketType>();
    protected List<Affiliate> Affiliates = new List<Affiliate>();

    public DevelopmentSeed(CelebratixDbContext dbContext, UserManager<ApplicationUser> userManager)
    {
        DbContext = dbContext;
        UserManager = userManager;
    }


    public void Seed(bool save = true)
    {
        // Logic to check if this seed shouldn't be run
        if (DbContext.Businesses.Any() || DbContext.Events.Any())
        {
            return;
        }

        AddCurrency();
        AddBusiness();
        AddUsers(Business);
        AddCategories();
        AddEvents(Business);
        AddTicketTypes();
        AddAffiliateCodes();
        AddOrders();

        if (save)
            DbContext.SaveChanges();
    }

    #region Helper functions

    private DateTime GenerateRandomDate()
    {
        var rand = new Random();
        var day = rand.Next(1, 28);
        var month = (Month++ % 12) + 1;             // Month increases by one for each call to get a better spread of events
        return new DateTime(Year, month, day).ToUniversalTime();      // Adds event this year, with a random date
    }

    private static string GenerateWords(int minWords = 3, int maxWords = 15)
    {
        var words = new[]{"lorem", "ipsum", "dolor", "sit", "amet", "consectetuer",
        "adipiscing", "elit", "sed", "diam", "nonummy", "nibh", "euismod",
        "tincidunt", "ut", "laoreet", "dolore", "magna", "aliquam", "erat"};

        var rand = new Random();
        var result = new StringBuilder();

        var numWords = rand.Next(minWords, maxWords + 1);

        for (var w = 0; w < numWords; w++)
        {
            if (w > 0) { result.Append(" "); }
            result.Append(words[rand.Next(words.Length)]);
        }

        return result.ToString();
    }
    private void FillTicketTypeWithOrders(EventTicketType ticketType)
    {
        var affiliateCodes = Affiliates;
        var alreadySold = ticketType.TicketsSold;
        var channelEvents = ChannelEvents;

        var relevantAffiliateCodes = affiliateCodes.Where(ac => channelEvents.Any(c => c.ChannelId == ac.ChannelId)).Distinct().ToList();
        var amountOfAffiliateCodes = relevantAffiliateCodes.Count;

        while (ticketType.AvailableTickets > 0)
        {
            var affiliateCode = relevantAffiliateCodes[new Random().Next(0, amountOfAffiliateCodes)];
            var ticketsLeft = ticketType.AvailableTickets ?? 0;
            var id = Guid.NewGuid();
            var tickets = AddTickets(ticketType, Math.Min((int)ticketsLeft, ticketType.MaxTicketsPerPurchase), id);
            var availableFrom = DateTimeOffset.MinValue;
            var availableUntil = DateTimeOffset.MaxValue;
            var endTime = availableUntil > DateTime.UtcNow ? DateTime.UtcNow : availableUntil;
            var availableDays = endTime - availableFrom;
            var createdAt = availableFrom.AddDays(new Random(ticketsLeft).Next(0, availableDays.Days + 1));

            var order = new Order
            {
                Id = id,
                Status = Enums.OrderStatus.Completed,
                CompletedAt = createdAt,
                AffiliateCode = affiliateCode,
                Purchaser = TicketPurchaseUser,
                TicketType = ticketType,
                Event = ticketType.Event!,
                TicketQuantity = tickets.Count,
                BaseAmount = tickets.Count * ticketType.Price, // TODO: Do these 3 amounts work for the marketplace
                ServiceAmount = tickets.Count * ticketType.ServiceFee,
                ApplicationAmount = tickets.Count * ticketType.ApplicationFee,
                Tickets = tickets,
                CurrencyId = CurrencyCode,
                CreatedAt = createdAt
            };

            DbContext.Orders.Add(order);

            ticketType.TicketsSold += tickets.Count;
        }
    }
    #endregion

    private void AddBusiness()
    {
        var country = DbContext.Countries.FirstOrDefault(c => c.Enabled);
        Business = new Business()
        {
            Name = BusinessName,
            CountryId = country!.Id,
        };

        DbContext.Businesses.Add(Business);

        Channel = new Channel()
        {
            Name = "Default",
            BusinessId = Business.Id,
            Business = Business,
            Slug = "default",
        };
        DbContext.Channels.Add(Channel);
    }

    private void AddUsers(Business business)
    {
        var rand = new Random();
        BusinessUser = new ApplicationUser
        {
            UserName = BusinessUserEmail,
            Email = BusinessUserEmail,
            EmailConfirmed = true,
            FirstName = BusinessUserGivenName,
            LastName = BusinessUserFamilyName,
            CreatedAt = DateTimeOffset.UtcNow,
            BusinessId = business.Id,
            DateOfBirth = DateTime.UtcNow.AddYears(rand.Next(-30, -20)).AddDays(rand.Next(-365, 1)),     //A business user is at least 21 years old
        };

        UserManager.CreateAsync(BusinessUser, BusinessUserPassword).Wait();
        UserManager.AddToRoleAsync(BusinessUser, Enums.Business).Wait();

        TicketPurchaseUser = new ApplicationUser
        {
            UserName = TicketPurchaseUserEmail,
            Email = TicketPurchaseUserEmail,
            EmailConfirmed = true,
            FirstName = TicketPurchaseUserGivenName,
            LastName = TicketPurchaseUserFamilyName,
            Gender = (Enums.Gender)rand.Next(1, 4),
            CreatedAt = DateTimeOffset.UtcNow,
            DateOfBirth = DateTime.UtcNow.AddYears(rand.Next(-30, -18)).AddDays(rand.Next(-365, 1)), //A ticket purchaser is at least 18 years old
        };

        UserManager.CreateAsync(TicketPurchaseUser, TicketPurchaseUserPassword).Wait();
    }

    private void AddCurrency()
    {
        BusinessCurrency = new Currency()
        {
            Code = CurrencyCode,
            Name = CurrencyName,
            Symbol = CurrencySymbol,
            Enabled = CurrencyEnabled,
            DecimalPlaces = CurrencyDecimalPlaces
        };
        DbContext.Currencies.Add(BusinessCurrency);
    }

    private void AddCategories()
    {
        foreach (var category in Categories)
        {
            var c = new Category()
            {
                Name = category
            };

            DbContext.Categories.Add(c);
        }
    }

    private void AddEvents(Business business)
    {
        for (var i = 0; i < AmountOfSeededEvents; i++)
        {
            var date = GenerateRandomDate();
            var e = new Event
            {
                Visible = true,
                BusinessId = business.Id,
                Business = business,
                Name = GenerateWords(2, 5),                 // Generate 2-5 word title with date
                Description = GenerateWords(6, 20),
                OpenDate = date,
                StartDate = date.AddHours(new Random().Next(0, 2)),     // Event starts 0-1 hours after doors open
                EndDate = date.AddHours(new Random().Next(5, 49)),       // Event ends 5-48 hours after doors
                Website = "https://ubit.com",
                //Location
                //Category
                CurrencyId = BusinessCurrency.Code,
                //Image
                CreatorId = BusinessUser.Id
                //Channels = new List<Channel>() { Channel }
            };

            DbContext.Events.Add(e);
            Events.Add(e);
            var ch = new ChannelEvent
            {
                ChannelId = Channel.Id,
                EventId = e.Id
            };
            ChannelEvents.Add(ch);
        }

        var now = DateTime.UtcNow;
        var eToday = new Event()                                        // Adds event for today
        {
            Visible = true,
            BusinessId = business.Id,
            Business = business,
            Name = GenerateWords(2, 5),
            Description = GenerateWords(6, 20),
            OpenDate = now,
            StartDate = now.AddHours(new Random().Next(0, 2)),
            EndDate = now.AddHours(new Random().Next(5, 49)),
            Website = "https://ubit.com/today",
            //Location
            //Category
            CurrencyId = BusinessCurrency.Code,
            //Image
            CreatorId = BusinessUser.Id
            //Channels = new List<Channel>() { Channel }
        };

        DbContext.Events.Add(eToday);
        Events.Add(eToday);
        var c = new ChannelEvent
        {
            ChannelId = Channel.Id,
            EventId = eToday.Id
        };
        ChannelEvents.Add(c);
    }

    private void AddTicketTypes()
    {
        var events = Events;

        var rand = new Random();
        foreach (var e in events)
        {
            var eventId = DbContext.Entry(e).Property(e => e.Id).CurrentValue;
            var ticketType = new EventTicketType
            {
                Name = "Early Bird " + e.Name.Split(" ")[0],
                Price = TicketTypePrice,
                CustomVat = TicketTypeVat,
                EventId = eventId,
                Event = e,
                AvailableFrom = e.StartDate.AddDays(rand.Next(-70, -40)),  // Available between 70-40 days before the event starts
                AvailableUntil = e.EndDate,                                // Unavaible after event finishes
                MaxTicketsPerPurchase = rand.Next(1, 5),                   // Max 1-4 tickets can be purchased per order
                MaxTicketsAvailable = rand.Next(1, 6) * 100,               // Max 100-500 tickets available for tickettype
                PubliclyAvailable = true,
            };

            e.TicketTypes = new List<EventTicketType>() { ticketType };
            TicketTypes.Add(ticketType);

            if (rand.Next(0, 2) >= 1)
            {
                var freeTicketType = new EventTicketType
                {
                    Name = "Family Free " + e.Name.Split(" ")[0],
                    Price = 0,
                    CustomVat = TicketTypeVat,
                    EventId = eventId,
                    Event = e,
                    AvailableFrom = e.StartDate.AddDays(rand.Next(-70, -40)),
                    AvailableUntil = e.EndDate,
                    MaxTicketsPerPurchase = rand.Next(1, 5),
                    MaxTicketsAvailable = rand.Next(1, 11) * 5,
                    PubliclyAvailable = false,
                    LinkCode = rand.Next(0, 2) >= 1 ? null : Guid.NewGuid().ToString().Split("-")[0]
                };

                e.TicketTypes.Add(freeTicketType);
                TicketTypes.Add(freeTicketType);
            }

            if (rand.Next(0, 2) >= 1)
            {
                var affiliateTicketType = new EventTicketType
                {
                    Name = "Affiliate " + e.Name.Split(" ")[0],
                    Price = 0,
                    CustomVat = TicketTypeVat,
                    EventId = eventId,
                    Event = e,
                    AvailableFrom = e.StartDate.AddDays(rand.Next(-70, -40)),
                    AvailableUntil = e.EndDate,
                    MaxTicketsPerPurchase = rand.Next(1, 5),
                    MaxTicketsAvailable = rand.Next(1, 11) * 5,
                    PubliclyAvailable = false,
                    OnlyAffiliates = true
                };

                e.TicketTypes.Add(affiliateTicketType);
                TicketTypes.Add(affiliateTicketType);
            }
        }
    }

    private void AddAffiliateCodes()
    {
        var ticketTypes = TicketTypes;

        foreach (var tt in ticketTypes)
        {
            var amountOfTrackers = new Random().Next(3, 5);
            for (int i = 0; i < amountOfTrackers; i++)
            {
                var affiliateCode = new Affiliate
                {
                    ChannelId = Channel.Id,
                    Name = (tt.Event != null ? tt.Event.Name.Split(" ")[0] : "(unnamed)") + " " + i,
                    Description = GenerateWords(2, 5),
                    Creator = BusinessUser,
                    Code = Guid.NewGuid().ToString().Split("-")[0]
                };

                DbContext.Affiliates.Add(affiliateCode);
                Affiliates.Add(affiliateCode);
            }
        }
    }

    private void AddOrders()
    {
        var ticketTypes = TicketTypes;
        var affiliateCodes = Affiliates;
        var channelEvents = ChannelEvents;
        var rand = new Random();

        foreach (var ticketType in ticketTypes)
        {

            if (ticketType.AvailableFrom > DateTime.UtcNow)
            {
                continue;
            }
            var maxAmount = (int)ticketType.MaxTicketsAvailable! / ticketType.MaxTicketsPerPurchase;         // Max amount of orders to avoid "overbuying" a tickettype
            var orderAmount = rand.Next(1, maxAmount);                                                      // Amount of orders to be seeded

            var relevantAffiliateCodes = affiliateCodes.Where(ac => channelEvents.Any(c => c.ChannelId == ac.ChannelId)).Distinct().ToList();
            var amountOfAffiliateCodes = relevantAffiliateCodes.Count;

            for (var i = 0; i < orderAmount; i++)
            {
                var affiliateCode = relevantAffiliateCodes[new Random().Next(0, amountOfAffiliateCodes)];
                var id = Guid.NewGuid();
                var tickets = AddTickets(ticketType, 0, id);
                var availableFrom = ticketType.AvailableFrom?.ToUniversalTime() ?? DateTime.UtcNow;
                var availableUntil = ticketType.AvailableUntil?.ToUniversalTime() ?? DateTime.UtcNow;
                var endTime = availableUntil > DateTime.UtcNow ? DateTime.UtcNow : availableUntil;
                var availableDays = endTime - availableFrom;
                var createdAt = availableFrom.DateTime.AddDays(new Random(i).Next(0, availableDays.Days + 1)).ToUniversalTime();

                var order = new Order()
                {
                    Id = id,
                    Status = Enums.OrderStatus.Completed,
                    CompletedAt = createdAt,
                    AffiliateCode = affiliateCode,
                    Purchaser = TicketPurchaseUser,
                    TicketType = ticketType,
                    Event = ticketType.Event!,
                    TicketQuantity = tickets.Count,
                    BaseAmount = tickets.Count * ticketType.Price,
                    ServiceAmount = tickets.Count * ticketType.ServiceFee,
                    ApplicationAmount = tickets.Count * ticketType.ApplicationFee,
                    Tickets = tickets,
                    Vat = ticketType.CustomVat,
                    CurrencyId = BusinessCurrency.Code,
                    CreatedAt = createdAt
                };
                DbContext.Orders.Add(order);

                ticketType.TicketsSold += tickets.Count;
            }
        }
        FillTicketTypeWithOrders(ticketTypes.First());
    }

    /// <summary>
    /// Used in AddOrders
    /// </summary>
    private ICollection<Ticket> AddTickets(EventTicketType ticketType, int amount, Guid latestOrderId)
    {
        var rand = new Random();

        var ticketQuantity = amount > 0 ? amount : rand.Next(1, ticketType.MaxTicketsPerPurchase + 1);          //randomise if amount == 0
        var eventTime = (ticketType.Event!.EndDate - ticketType.Event.OpenDate).Hours;

        var tickets = new List<Ticket>();

        for (int i = 0; i < ticketQuantity; i++)
        {
            var ticket = new Ticket()
            {
                TicketType = ticketType,
                Owner = TicketPurchaseUser,
                CheckedIn = DateTime.Now > ticketType.Event.OpenDate.AddHours(rand.Next(0, eventTime)),      //Random checkin from opening time of event until end of event
                LatestOrderId = latestOrderId
            };

            tickets.Add(ticket);

            DbContext.Tickets.Add(tickets[i]);

            if (ticket.CheckedIn)
            {
                ticketType.TicketsCheckedIn++;
            }
        }
        return tickets;
    }

}
