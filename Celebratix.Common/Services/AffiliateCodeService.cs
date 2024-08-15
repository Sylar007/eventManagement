using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class AffiliateCodeService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<AffiliateCodeService> _logger;
    private readonly ChannelService _channelService;
    private readonly UserService _userService;

    private const int AffiliateCodeLength = 10;

    public AffiliateCodeService(CelebratixDbContext dbContext, ILogger<AffiliateCodeService> logger, ChannelService channelService, UserService userService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _channelService = channelService;
        _userService = userService;
    }

    public async Task<PagedResultDto<AffliateBusinessDto>> GetAffiliateCodesForBusiness(Guid businessId, int page, int pageSize)
    {
        return await GetAffiliateCodeBase()
            .Where(ac => ac.Channel!.BusinessId == businessId)
            .Select(ac => new AffliateBusinessDto(ac))
            .ToPagedResult(page, pageSize);
    }

    public async Task<PagedResultDto<AffliateBusinessDto>> GetAffiliateCodesForChannel(Guid channelId, int page, int pageSize)
    {
        return await GetAffiliateCodeBase()
            .Include(ac => ac.Orders)
            .Where(ac => ac.ChannelId == channelId)
            .Select(ac => new AffliateBusinessDto(ac))
            .ToPagedResult(page, pageSize);
    }

    public async Task<AffliateDetailedBusinessDto> GetAffiliateCode(Guid id)
    {
        var affiliateCode = await _dbContext.Affiliates
            .Include(ac => ac.Orders)
            .FirstOrThrowAsync(ac => ac.Id == id);
        return new AffliateDetailedBusinessDto(affiliateCode);
    }

    public async Task<Affiliate> GetAffiliateCodeByCode(string affiliateCodeCode)
    {
        return await _dbContext.Affiliates.FirstOrThrowAsync(ac => ac.Code == affiliateCodeCode);
    }

    public async Task<AffliateDetailedBusinessDto> CreateAffiliateCode(string creatorId, AffliateCreateBusinessDto dto)
    {
        var code = dto.CustomCode ?? RandomStringsHelper.RandomString(AffiliateCodeLength);
        var codeExists = await _dbContext.Affiliates.AnyAsync(ac => ac.Code == code); // Affiliate codes should be globally unique
        if (codeExists)
            throw new ObjectAlreadyExistsException($"The given affiliate code {code} already exists");

        var affiliateCode = new Affiliate();
        _dbContext.Entry(affiliateCode).CurrentValues.SetValues(dto);
        affiliateCode.ChannelId = dto.ChannelId;
        affiliateCode.Code = code;
        affiliateCode.CreatorId = creatorId;
        _dbContext.Add(affiliateCode);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Created event affiliate code for channel: {dto.ChannelId} with id {affiliateCode.Id}");
        return new AffliateDetailedBusinessDto(affiliateCode);
    }

    public async Task<AffliateDetailedBusinessDto> UpdateAffiliateCode(Guid id, AffliateUpdateBusinessDto dto)
    {
        var affiliateCode = await _dbContext.Affiliates
            .Include(a => a.Orders)
            .FirstOrThrowAsync(a => a.Id == id);
        _dbContext.Entry(affiliateCode).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Updated event affiliate code with id {affiliateCode.Id}");
        return new AffliateDetailedBusinessDto(affiliateCode);
    }

    private IQueryable<Affiliate> GetAffiliateCodeBase()
    {
        return _dbContext.Affiliates
                .Include(ac => ac.Orders)
                .Include(ac => ac.Channel);
    }
}
