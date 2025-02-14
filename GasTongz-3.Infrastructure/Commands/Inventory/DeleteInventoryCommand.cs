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
    public record DeleteInventoryCommand(int InventoryId) : IRequest<Unit>;

    public class DeleteInventoryCommandValidator : AbstractValidator<DeleteInventoryCommand>
    {
        public DeleteInventoryCommandValidator()
        {
            RuleFor(c => c.InventoryId).GreaterThan(0).WithMessage("Invalid inventory ID.");
        }
    }
    public class DeleteInventoryCommandHandler : IRequestHandler<DeleteInventoryCommand, Unit>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<DeleteInventoryCommandHandler> _logger;

        public DeleteInventoryCommandHandler(
            IInventoryRepository inventoryRepository,
            ILogger<DeleteInventoryCommandHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(
            DeleteInventoryCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                // Validate command  
                var validator = new DeleteInventoryCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError($"Validation failed: {errorMessages}");
                    return Unit.Value;
                }

                // Check if inventory exists  
                var existingInventory = await _inventoryRepository.GetInventoryByIdAsync(command.InventoryId);
                if (existingInventory == null)
                {
                    _logger.LogError($"Inventory with ID {command.InventoryId} not found.");
                    return Unit.Value;
                }

                // Check if inventory is already soft-deleted  
                if (existingInventory.IsDeleted)
                {
                    _logger.LogError($"Attempted to delete already deleted inventory with ID {command.InventoryId}.");
                    return Unit.Value;
                }

                // Soft-delete inventory  
                await _inventoryRepository.SoftDeleteInventoryAsync(command.InventoryId);
                _logger.LogInformation($"Inventory with ID {command.InventoryId} marked as deleted.");
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting inventory.");
                throw; // Let global exception handler catch this  
            }
        }
    }
}
