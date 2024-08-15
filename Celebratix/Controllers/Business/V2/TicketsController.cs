using Asp.Versioning;
using Celebratix.Common.ErrorHandling;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.Business.Tickets;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Celebratix.Common.Models.Enums;

namespace Celebratix.Controllers.Business.V2
{
    [ApiVersion(2.0)]
    [Area("business")]
    [Route("v{version:apiVersion}/[area]/[controller]")]
    [ApiController]
    [AuthorizeRoles(Enums.Role.Business, Enums.Role.TicketChecker)]

    public class TicketsController : CelebratixControllerBase
    {
        private readonly UserService _userService;
        private readonly TicketService _ticketService;

        public TicketsController(UserService userService,
            TicketService ticketService)
        {
            _userService = userService;
            _ticketService = ticketService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(OperationResult<TicketTypeDashboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(OperationResult<TicketTypeDashboardDto>), StatusCodes.Status400BadRequest)]
        [AuthorizeRoles(Enums.Role.Business)]
        public async Task<IActionResult> CreateEventTicket([FromForm] EventTicketCreateRequest request)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var accessValidation = await _userService.VerifyUserHasEventAccessV2(currentUserId, request.EventId);
            if (accessValidation.IsFailed)
            {
                return GetOkResult<TicketTypeDashboardDto>(accessValidation);
            }

            var result = await _ticketService.CreateEventTicket(request);

            return GetCreatedResult(result);
        }

        [HttpPost("event/{eventId:int}/scan")]
        public async Task<ActionResult<ResultDto<TicketScanningResponseDto, ErrEnum<ScanFailReason>>>>
        ScanTicket(int eventId, TicketScanningInputDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            await _userService.VerifyUserHasEventAccess(currentUserId, eventId);
            return ToResponse(await _ticketService.ScanTicketV2(eventId, dto));
        }
    }
}
