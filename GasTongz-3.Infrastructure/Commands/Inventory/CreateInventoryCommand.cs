using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.DTOs;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.Commands.Inventory
{
    public record CreateInventoryCommand(
        int ShopId,
        int ProductId,
        int Quantity,
        int? CreatedBy) : IRequest<int>;


    public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
    {
        public CreateInventoryCommandValidator()
        {
            RuleFor(c => c.ShopId).GreaterThan(0).WithMessage("Invalid shop ID.");

            RuleFor(c => c.ProductId).GreaterThan(0).WithMessage("Invalid product ID.");

            RuleFor(c => c.Quantity).GreaterThanOrEqualTo(0).WithMessage("Quantity must be non-negative.");

            RuleFor(c => c.CreatedBy).GreaterThan(0).When(c => c.CreatedBy.HasValue)
                .WithMessage("CreatedBy must be greater than zero.");
        }
    }

    public class CreateInventoryCommandHandler : IRequestHandler<CreateInventoryCommand, int>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<CreateInventoryCommandHandler> _logger;
        private readonly IShopRepository _shopRepository;

        public CreateInventoryCommandHandler(
            IInventoryRepository inventoryRepository,
            ILogger<CreateInventoryCommandHandler> logger,
            IShopRepository shopRepository)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
            _shopRepository = shopRepository;
        }

        public async Task<int> Handle(
            CreateInventoryCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate command  
                var validator = new CreateInventoryCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError($"Validation failed: {errorMessages}");
                    return 0; // Indicate failure  
                }

                // Check if shop exists  
                var existingShop = await _shopRepository.GetByIdAsync(command.ShopId);
                if (existingShop == null)
                {
                    _logger.LogError($"Shop with ID {command.ShopId} not found.");
                    return 0;
                }

                var existingInventory = await _inventoryRepository.GetInventoryIncludingDeletedAsync(command.ShopId, command.ProductId);
                if (existingInventory != null && !existingInventory.IsDeleted)
                {
                    _logger.LogError($"Active inventory already exists for ShopId: {command.ShopId}, ProductId: {command.ProductId}.");
                    return 0; // Block creation
                }

                // Create inventory  
                var inventory = new _1_GasTongz.Domain.Entities.Inventory(
                    shopId: command.ShopId,
                    productId: command.ProductId,
                    quantity: command.Quantity,
                    status: 'F',
                    createdBy: command.CreatedBy);

                return await _inventoryRepository.CreateAsync(inventory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating inventory.");
                return 0;
            }
        }
    }
}
