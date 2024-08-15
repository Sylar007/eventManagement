using System.Security.Claims;
using Asp.Versioning;
using Celebratix.Common.Helpers;
using Celebratix.Common.Models;
using Celebratix.Common.Models.DTOs;
using Celebratix.Common.Models.DTOs.Business;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.Business.Transaction;
using Celebratix.Common.Services;
using Celebratix.Models;
using Microsoft.AspNetCore.Mvc;
using SendGrid.Helpers.Errors.Model;

namespace Celebratix.Controllers.Business.V2;

[ApiVersion(2.0)]
[Area("business")]
[Route("v{version:apiVersion}/[area]/[controller]")]
[ApiController]
[AuthorizeRoles(Enums.Role.Business)]
public class TransactionController : CelebratixControllerBase
{
    private readonly TransactionService _transactionService;
    private readonly UserService _userService;

    public TransactionController(UserService userService, TransactionService TransactionService)
    {
        _userService = userService;
        _transactionService = TransactionService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<TransactionBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<TransactionBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTransactions(int page = 1, int pageSize = 10)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _transactionService.GetTransactions(businessId.Value, page, pageSize);

        return GetOkResult(result);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<TransactionBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<TransactionBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTransactionById(Guid id)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _transactionService.GetTransactionDto(id);

        return GetOkResult(result);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<string>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionCreateBusinessRequest createRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _transactionService.CreateTransaction(businessId.Value, createRequest);

        return GetOkResult(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OperationResult<TransactionBusinessDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<TransactionBusinessDto>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTransaction(Guid id, TransactionCreateBusinessRequest updateRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _transactionService.UpdateTransaction(id, updateRequest);

        return GetOkResult(result);
    }

    [HttpPost("search")]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<TransactionBusinessDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResult<PagedResultDto<TransactionBusinessDto>>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetTransactionsBySearch(TransactionSearchRequest? searchRequest)
    {
        var businessId = await _userService.GetUserBusinessIdByClaimsPrincipal(User);
        if (businessId.IsFailed)
        {
            return GetOkResult(businessId);
        }
        var result = await _transactionService.GetTransactionsBySearch(searchRequest, businessId.Value);

        return GetOkResult(result);
    }
}
