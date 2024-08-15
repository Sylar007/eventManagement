using Celebratix.Common.Models.DTOs.Business;
using Celebratix.Common.Models.DTOs.Business.Channel;
using Celebratix.Common.Models.DTOs.Business.Events;
using Celebratix.Common.Models.DTOs.Business.Tickets;
using Celebratix.Common.Validators.Channel;
using Celebratix.Common.Validators.Events;
using Celebratix.Common.Validators.Tickets;
using Celebratix.Common.Validators.Tracking;
using Celebratix.Common.Validators.Transaction;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Celebratix.Common.Extensions
{
    public static class ValidatorExtensions
    {
        public static IServiceCollection AddValidators(
            this IServiceCollection services) =>
            services
                .AddValidator<EventCreateRequest, EventCreateRequestValidator>()
                .AddValidator<EventUpdateRequest, EventUpdateRequestValidator>()
                .AddValidator<EventTicketCreateRequest, EventTicketCreateRequestValidator>()
                .AddValidator<ChannelRequest, ChannelRequestValidator>()
                .AddValidator<ChannelEventRequest, ChannelEventRequestValidator>()
                .AddValidator<BusinessTrackerCreateRequest, BusinessTrackerCreateRequestValidator>()
                .AddValidator<TransactionCreateBusinessRequest, TransactionCreateBusinessRequestValidator>()
                .AddValidator<ChannelEventUpdateResaleRequest, ChannelEventUpdateResaleRequestValidator>()
                .AddValidator<TicketUpdateRefundInputDto, TicketUpdateRefundInputDtoValidator>();

        private static IServiceCollection AddValidator<TClass, TValidator>(
            this IServiceCollection services)
            where TValidator : class, IValidator<TClass> =>
            services.AddTransient<IValidator<TClass>, TValidator>();
    }
}