using Celebratix.Common.Configs;
using Celebratix.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stripe;

namespace Celebratix.Controllers;

[Route("[controller]")]
[ApiController]
public class StripeController : ControllerBase
{
    private readonly StripeConfig _stripeConfig;
    private readonly PaymentService _paymentService;
    private readonly ILogger<StripeController> _logger;

    public StripeController(IOptions<StripeConfig> stripeConfig, PaymentService paymentService,
        ILogger<StripeController> logger)
    {
        _paymentService = paymentService;
        _logger = logger;
        _stripeConfig = stripeConfig.Value;
    }

    [HttpGet("client-key")]
    public ActionResult<string> GetClientKey()
    {
        return Ok(_stripeConfig.PublicClientKey);
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> HandleAccountWebhook()
    {
        _logger.LogInformation("Started processing stripe (account) webhook request");
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _stripeConfig.AccountWebhookSecret);

            _paymentService.HandleStripeEvent(stripeEvent);

            _logger.LogInformation("Successfully finished processing stripe (account) webhook request");
            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }

    [HttpPost("webhook/connect")]
    public async Task<IActionResult> HandleConnectWebhook()
    {
        _logger.LogInformation("Started processing stripe (connect) webhook request");
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json,
                Request.Headers["Stripe-Signature"], _stripeConfig.ConnectWebhookSecret);

            _paymentService.HandleStripeEvent(stripeEvent);

            _logger.LogInformation("Successfully finished processing stripe (connect) webhook request");
            return Ok();
        }
        catch (StripeException)
        {
            return BadRequest();
        }
    }
}
