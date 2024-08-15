using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs.User.Orders;
using Microsoft.EntityFrameworkCore;

namespace Celebratix.Common.Services;

public class MagicService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly JwtService _jwtService;
    private readonly TicketService _ticketService;

    public MagicService(CelebratixDbContext dbContext, JwtService jwtService, TicketService ticketService)
    {
        _dbContext = dbContext;
        _jwtService = jwtService;
        _ticketService = ticketService;
    }

    public string CreateMagicForOrder(Guid orderId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, orderId.ToString()),
        };
        var jwtToken = _jwtService.GenerateToken(claims, DateTime.Now.AddYears(1));
        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(jwtToken);
    }

    public async Task<OrderFromMagicDto> GetOrderFromMagicAsDto(string magic, bool excludeUnavailable)
    {
        var order = await GetOrderFromMagic(magic);
        var purchaserId = order.PurchaserId;
        if (purchaserId == null)
        {
            throw new UserDeletedException();
        }
        var tickets = order.Status != Enums.OrderStatus.Completed ? null : (await _ticketService.GetUsersTicketsForOrder(purchaserId, order.Id, excludeUnavailable))
            .ToList();
        if (excludeUnavailable && tickets != null)
        {
#pragma warning disable 0618 // disable use of obsolete variable warning
            tickets = tickets.Where(t => t.Key != null).ToList();
#pragma warning restore 0618
        }
        return new OrderFromMagicDto(order, tickets);
    }

    private async Task<Order> GetOrderFromMagic(string magic)
    {
        if (!_jwtService.IsValidToken(magic))
        {
            throw new JwtTokenInvalidException();
        }
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(magic);
        var orderId = jwt.Claims
            .Where(c => c.Type == ClaimTypes.NameIdentifier)
            .Select(c => Guid.Parse(c.Value))
            .First();
        return await _dbContext.Orders
            .Where(o => o.Id == orderId)
            .Include(o => o.Event)
                .ThenInclude(e => e!.Business)
            .Include(o => o.Event)
                .ThenInclude(e => e!.Image)
            .Include(o => o.Event)
                .ThenInclude(e => e!.TicketBackgroundImage)
            .Include(o => o.Event)
                .ThenInclude(e => e!.Category)
            .Include(o => o.Currency)
            .Include(o => o.TicketType)
            .FirstOrThrowAsync();
    }
}
