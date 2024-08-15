using Asp.Versioning;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.User.Events;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Celebratix.Controllers.Business.V2
{
    [ApiVersion(2.0)]
    [Area("business")]
    [Route("v{version:apiVersion}/[area]/channels/{channelId:guid}/events/{eventId:int}/ticket-types")]
    [ApiController]
    [AuthorizeRoles(Enums.Role.Business)]
    public class ChannelEventTicketTypesController : CelebratixControllerBase
    {
        private readonly UserService _userService;
        private readonly ChannelEventTicketTypeService _channelEventTicketTypeService;

        public ChannelEventTicketTypesController(
            ChannelEventTicketTypeService channelEventTicketTypeService,
            UserService userService)
        {
            _channelEventTicketTypeService = channelEventTicketTypeService;
            _userService = userService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(OperationResult<List<EventTicketTypeBasicDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult<List<EventTicketTypeBasicDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(Guid channelId, int eventId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var accessValidationResult = await _userService.VerifyUserHasChannelAccessV2(userId, channelId);

            if (accessValidationResult.IsFailed)
            {
                return GetOkResult<List<TicketTypeDashboardDto>>(accessValidationResult);
            }
            var result = await _channelEventTicketTypeService.GetTicketTypesAsync(channelId, eventId);

            return GetOkResult(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(OperationResult<List<EventTicketTypeBasicDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult<List<EventTicketTypeBasicDto>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddOrUpdate(int eventId, Guid channelId, List<Guid> selectedTicketTypes)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var accessValidationResult = await _userService.VerifyUserHasChannelAccessV2(userId, channelId);

            if (accessValidationResult.IsFailed)
            {
                return GetOkResult<List<TicketTypeDashboardDto>>(accessValidationResult);
            }

            var result = await _channelEventTicketTypeService.AddOrUpdateAsync(channelId, eventId, selectedTicketTypes);

            return GetOkResult(result);
        }
    }
}
