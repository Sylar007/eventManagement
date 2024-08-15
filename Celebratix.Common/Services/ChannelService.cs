using AutoMapper;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Exceptions;
using Celebratix.Common.Extensions;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DbModels;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Services.Interfaces;
using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Celebratix.Common.Services;

public class ChannelService
{
    private readonly CelebratixDbContext _dbContext;
    private readonly ILogger<ChannelService> _logger;
    private readonly IMapper _mapper;
    private readonly IImageStorageService _imageStorageService;
    private readonly IValidator<ChannelRequest> _channelRequestValidator;

    public ChannelService(CelebratixDbContext dbContext, ILogger<ChannelService> logger,
        IImageStorageService imageStorageService,
        IMapper mapper,
        IValidator<ChannelRequest> channelRequestValidator)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
        _imageStorageService = imageStorageService;
        _channelRequestValidator = channelRequestValidator;
    }

    public async Task<Channel> GetChannel(Guid id)
    {
        return await _dbContext.Channels
            .Include(ts => ts.Business)
            .FirstOrThrowAsync(ts => ts.Id == id);
    }

    public async Task<ChannelBusinessDto> GetChannelDto(Guid id)
    {
        return new ChannelBusinessDto(await GetChannel(id));
    }

    public async Task<Result<ChannelDetailedDto>> GetChannelRequest(Guid id)
    {
        var channel = await _dbContext
            .Channels
            .Include(channel => channel.ChannelEvents)
            .ThenInclude(events => events.SelectedTicketTypes)
            .ThenInclude(ticketTypes => ticketTypes.EventTicketType)
            .Where(channel => channel.Id == id)
            .FirstOrDefaultAsync();

        return channel == null
            ? Result.Fail(new CelebratixError(
                ErrorCode.CelebratixChannelInvalidOrNotFound,
                $"Channel with id '{id}' is not linked to any events."))
            : (ChannelDetailedDto)channel;
    }

    public async Task<Channel> GetChannelBySlug(string slug)
    {
        return await _dbContext.Channels
            .Include(ts => ts.Business)
            .FirstOrThrowAsync(ts => ts.Slug == slug);
    }

    /// <summary>
    /// If no channels is provided for creating a event for a business the default channel is used
    /// </summary>
    public async Task<Channel> GetOrCreateDefaultChannel(Guid businessId)
    {
        var exists = await _dbContext.Channels.AnyAsync(ts => ts.BusinessId == businessId);
        if (!exists)
        {
            var dto = new ChannelCreateBusinessRequest { Name = "Default channel" };
            return await CreateChannel(businessId, dto);
        }

        return await _dbContext.Channels
            .Include(ts => ts.Business)
            .Where(ts => ts.BusinessId == businessId)
            .OrderBy(ts => ts.CreatedAt) // The first created channel is the default
            .FirstOrThrowAsync();
    }

    public async Task<PagedResultDto<ChannelBusinessDto>> GetChannels(Guid businessId, int page, int pageSize)
    {
        return await _dbContext.Channels
            .Include(ts => ts.Business)
            .Include(ts => ts.AffiliateCodes)
            .Where(ts => ts.BusinessId == businessId)
            .Select(ts => new ChannelBusinessDto(ts))
            .ToPagedResult(page, pageSize);
    }

    public async Task<Result<PagedResultDto<ChannelBusinessDto>>> GetChannelsRequest(Guid businessId, int page, int pageSize)
    {
        return await _dbContext.Channels
            .Include(ts => ts.Business)
            .Include(ts => ts.AffiliateCodes)
            .Where(ts => ts.BusinessId == businessId)
            .Select(ts => new ChannelBusinessDto(ts))
            .ToPagedResult(page, pageSize);
    }

    public async Task<Result<List<ChannelBusinessDto>>> GetChannelsForEvent(int eventId, Guid businessId)
    {
        return await _dbContext.ChannelEvents
            .Include(e => e.Event)
            .Where(x => x.EventId == eventId)
            .Select(ce => new ChannelBusinessDto(ce.Channel))
            .ToListAsync();
    }

    public async Task<Channel> CreateChannel(Guid businessId, ChannelCreateBusinessRequest dto)
    {
        string slug = dto.Slug ?? RandomStringsHelper.RandomSlug(5);
        bool slugExists = await _dbContext.Channels.AnyAsync(ts => ts.Slug == slug);
        if (slugExists)
            throw new ObjectAlreadyExistsException($"Channel with slug {slug} already exists");

        ImageDbModel? dbImage = null;
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();

        try
        {
            dbImage = await _imageStorageService.UploadFile(dto.Image);

            var channel = new Channel();
            _dbContext.Entry(channel).CurrentValues.SetValues(dto);
            channel.Slug = slug;
            channel.BusinessId = businessId;
            channel.CustomBackground = dbImage;

            _dbContext.Channels.Add(channel);

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Created channel for business: {businessId} with id {channel.Id}");
            return channel;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();

            if (dbImage != null)
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            throw;
        }
        finally
        {
            await transaction.CommitAsync();
        }
    }

    public async Task<Result<ChannelBusinessDto>> CreateChannelV2(Guid businessId, ChannelRequest request)
    {
        ImageDbModel? dbImage = null;

        try
        {
            var validationResult = await _channelRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }
            var slug = request.Slug ?? RandomStringsHelper.RandomSlug(5);
            var slugExists = await _dbContext.Channels.AnyAsync(ts => ts.Slug == slug);
            if (slugExists)
            {
                return Result.Fail(new CelebratixError(ErrorCode.CelebratixChannelSlugAlreadyExists, $"Channel with slug {slug} already exists"));
            }

            var channel = _mapper.Map<Channel>(request);
            dbImage = await _imageStorageService.UploadFile(request.Image);

            if (dbImage != null)
            {
                channel.CustomBackground = dbImage;
                _dbContext.Add(dbImage);
            }
            channel.Slug = slug;
            channel.BusinessId = businessId;
            _dbContext.Add(channel);

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Created channel for business: {businessId} with id {channel.Id}");
            return _mapper.Map<ChannelBusinessDto>(channel);
        }
        catch (Exception)
        {
            if (dbImage != null)
            {
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            }
            throw;
        }
    }

    public async Task<Result<ChannelBusinessDto>> CreateChannelEvent(int eventId, Guid businessId, ChannelEventRequest request)
    {
        ImageDbModel? dbImage = null;

        try
        {
            var validationResult = await _channelRequestValidator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return Result.Fail(validationResult.Errors.ToErrorMessage());
            }
            var slug = request.Slug ?? RandomStringsHelper.RandomSlug(5);
            var slugExists = await _dbContext.Channels.AnyAsync(ts => ts.Slug == slug);
            if (slugExists)
            {
                return Result.Fail(new CelebratixError(ErrorCode.CelebratixChannelSlugAlreadyExists, $"Channel with slug {slug} already exists"));
            }

            var channel = _mapper.Map<Channel>(request);
            dbImage = await _imageStorageService.UploadFile(request.Image);
            channel.Slug = slug;
            channel.CustomBackground = dbImage;
            channel.BusinessId = businessId;
            _dbContext.Add(channel);

            var channelEvent = new ChannelEvent();
            channelEvent.ChannelId = channel.Id;
            channelEvent.EventId = eventId;
            _dbContext.Add(channelEvent);

            foreach (var ticketTypeId in request.SelectedTicketTypes)
            {
                var ticketTypeExists = _dbContext.EventTicketTypes
                    .Any(tt => tt.Id == ticketTypeId && tt.EventId == eventId);

                if (!ticketTypeExists)
                {
                    return Result.Fail(new CelebratixError(ErrorCode.GenericValidator,
                        $"TicketTypeId {ticketTypeExists} does not exist or does not have relationship with EventId {eventId}"));
                }

                var channelTickets = new ChannelEventTicketType
                {
                    ChannelEventId = channelEvent.Id,
                    EventTicketTypeId = ticketTypeId,
                };
                channelTickets.ChannelEventId = channelEvent.Id;
                channelTickets.EventTicketTypeId = ticketTypeId;
                _dbContext.Add(channelTickets);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Created channel for business: {businessId} with id {channel.Id}");
            return _mapper.Map<ChannelBusinessDto>(channel);
        }
        catch (Exception)
        {
            if (dbImage != null)
            {
                await _imageStorageService.DeleteFile(dbImage.StorageIdentifier);
            }
            throw;
        }
    }

    public async Task<ChannelBusinessDto> CreateChannelDto(Guid businessId, ChannelCreateBusinessRequest dto)
        => new ChannelBusinessDto(await CreateChannel(businessId, dto));

    public async Task<Result<ChannelBusinessDto>> UpdateChannelV2(Guid id, ChannelRequest request)
    {
        var validationResult = await _channelRequestValidator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Result.Fail(validationResult.Errors.ToErrorMessage());
        }

        var channel = _dbContext.Channels.FirstOrDefault(e => e.Id == id);

        if (channel != null)
        {
            channel
                .Update(() => channel.Name = request.Name, !string.IsNullOrEmpty(request.Name))
                .Update(() => channel.Slug = request.Slug!, request.Slug != null)
                .Update(() => channel.Color = request.Color!, request.Color != null)
                .Update(() => channel.TemplateType = request.TemplateType!, request.TemplateType != Enums.ChannelTemplateTypes.Widget); ;

            if (request.Image != null)
            {
                await UpdateImage(channel, request.Image);
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogInformation($"Updated channel with id {id}");

            return new ChannelBusinessDto(channel);
        }

        return Result.Fail(
            new CelebratixError(
                ErrorCode.CelebratixGeneric,
                $"Channel with id {id} could not be found."));
    }

    public async Task<ChannelBusinessDto> UpdateChannel(Guid channelId, ChannelCreateBusinessRequest dto)
    {
        var channel = await GetChannel(channelId);
        _dbContext.Entry(channel).CurrentValues.SetValues(dto);
        await _dbContext.SaveChangesAsync();
        _logger.LogInformation($"Updated channel with id {channel.Id}");
        return new ChannelBusinessDto(channel);
    }

    private async Task UpdateImage(Channel channel, IFormFile file)
    {
        var currentImage = channel.CustomBackgroundId;

        var uploadFile = await _imageStorageService.UploadFile(file);

        if (uploadFile != null && currentImage != null)
        {
            channel.CustomBackgroundId = uploadFile.StorageIdentifier;
            await _imageStorageService.DeleteFile(currentImage.Value);
        }
    }
}