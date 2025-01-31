using _1_GasTongz.Domain.Entities;
using _1_GasTongz.Domain.Enums;
using _3_GasTongz.Infrastructure.Commands;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;


namespace _3_GasTongz.Infrastructure.Commands
{
    // Application/Commands/CreateTransactionCommand.cs
    public record CreateTransactionCommand(
        int ShopId,
        PaymentMethod PaymentMethod,
        List<LineItemDto> LineItems,
        int? UserId
    ): IRequest<int>;

    public record LineItemDto(int ProductId, int Quantity, decimal UnitPrice);


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
    public class CreateTransactionCommandHandler
    : IRequestHandler<CreateTransactionCommand, int> // returns new Transaction.Id
    {
        private readonly ITransactionRepository _transactionRepo;
        private readonly IInventoryRepository _inventoryRepo;
        private readonly ILogger<CreateTransactionCommandHandler> _logger;


        public CreateTransactionCommandHandler(
            ITransactionRepository transactionRepo,
            IInventoryRepository inventoryRepo,
            ILogger<CreateTransactionCommandHandler> logger)
        {
            _transactionRepo = transactionRepo;
            _inventoryRepo = inventoryRepo;
            _logger = logger;
        }

        public async Task<int> Handle(CreateTransactionCommand command, CancellationToken ct)
        {
            // 1. Create domain entity
            var transaction = new Transaction(
                shopId: command.ShopId,
                paymentMethod: command.PaymentMethod,
                paymentStatus: PaymentStatus.Pending,
                totalAmount: 0m,
                createdBy: command.UserId
            );

            // 2. Add details
            foreach (var item in command.LineItems)
            {
                transaction.AddDetail(item.ProductId, item.Quantity, item.UnitPrice, command.UserId);

                // Optionally deduct inventory
                var inventory = await _inventoryRepo.GetInventoryAsync(command.ShopId, item.ProductId);
                try
                {

                    if (inventory == null || inventory.Quantity < item.Quantity)
                    {
                        // dont throw exceptions
                        //throw new Exception("Insufficient stock.");

                    }
                    inventory.UpdateQuantity(inventory.Quantity - item.Quantity, command.UserId);
                    await _inventoryRepo.UpdateAsync(inventory);
                }

                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Something failed while updating inventory for ShopId: {ShopId}, ProductId: {ProductId}",
                        command.ShopId, item.ProductId);
                }

            }

            // 3. Mark payment success or keep pending?
            // transaction.MarkPaymentSuccess(command.UserId);

            // 4. Save transaction
            var newId = await _transactionRepo.CreateAsync(transaction);
            return newId;
        }
    }

}

