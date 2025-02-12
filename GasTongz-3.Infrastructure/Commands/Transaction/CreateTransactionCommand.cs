using _1_GasTongz.Domain.Entities;
using _1_GasTongz.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using Newtonsoft.Json;


namespace Commands.Transaction
{
    // Application/Commands/CreateTransactionCommand.cs
    public record CreateTransactionCommand(
        int ShopId,
        PaymentMethod PaymentMethod,
        List<LineItemDto> LineItems,
        int? UserId
    ) : IRequest<int>;
    public record LineItemDto(
        int ProductId,
        int Quantity,
        decimal UnitPrice);
    public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
    {
        public CreateTransactionCommandValidator()
        {
            // Validate ShopId
            RuleFor(x => x.ShopId)
                .GreaterThan(0)
                .WithMessage("ShopId must be greater than zero.");

            // Validate PaymentMethod
            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("Invalid payment method.");

            // Validate LineItems
            RuleFor(x => x.LineItems)
                .NotEmpty()
                .WithMessage("At least one line item is required.");

            // Validate each LineItemDto
            RuleForEach(x => x.LineItems).SetValidator(new LineItemDtoValidator());

            // Validate UserId (optional, but if provided, should be greater than zero)
            When(x => x.UserId.HasValue, () =>
            {
                RuleFor(x => x.UserId.Value)
                    .GreaterThan(0)
                    .WithMessage("UserId must be greater than zero.");
            });
        }
    }

    // FluentValidation for LineItemDto
    public class LineItemDtoValidator : AbstractValidator<LineItemDto>
    {
        public LineItemDtoValidator()
        {
            // Validate ProductId
            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero.");

            // Validate Quantity
            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than zero.");

            // Validate UnitPrice
            RuleFor(x => x.UnitPrice)
                .GreaterThanOrEqualTo(0)
                .WithMessage("UnitPrice cannot be negative.");
        }
    }



    // fluent validation goes here
    public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, int>
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly ILogger<CreateTransactionCommandHandler> _logger;

        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepo,
            ILogger<CreateTransactionCommandHandler> logger)
        {
            _transactionRepo = transactionRepo;
            _logger = logger;
        }

        public async Task<int> Handle(CreateTransactionCommand command, CancellationToken cancellationToken)
        {
            // todo: Write explicit fluent validation validators here, or ask ai
            // Serialize LineItems to JSON
            var lineItemsJson = JsonConvert.SerializeObject(command.LineItems);

            // Call the stored procedure via repository
            return await _transactionRepo.CreateTransactionWithInventoryUpdate(
                command.ShopId,
                command.PaymentMethod,
                lineItemsJson,
                command.UserId
            );
        }
    }

}

