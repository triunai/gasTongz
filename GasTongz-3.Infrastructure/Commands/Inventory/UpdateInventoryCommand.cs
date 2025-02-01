using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commands.Inventory
{
    // Command request: update an inventory record given shop, product, new values, and user info
    public record UpdateInventoryCommand(
        int ShopId,
        int ProductId,
        int NewQuantity,
        char NewStatus,
        int? UpdatedBy
    ) : IRequest<Unit>;

    // Validator for the command
    public class UpdateInventoryCommandValidator : AbstractValidator<UpdateInventoryCommand>
    {
        public UpdateInventoryCommandValidator()
        {
            RuleFor(c => c.ShopId)
                .GreaterThan(0)
                .WithMessage("ShopId must be greater than zero.");
            RuleFor(c => c.ProductId)
                .GreaterThan(0)
                .WithMessage("ProductId must be greater than zero.");
            RuleFor(c => c.NewQuantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("NewQuantity must be zero or greater.");
            // Optionally, validate NewStatus if there is a defined set of allowed characters.
        }
    }

    // Handler for the command
    public class UpdateInventoryCommandHandler : IRequestHandler<UpdateInventoryCommand, Unit>
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<UpdateInventoryCommandHandler> _logger;

        public UpdateInventoryCommandHandler(
            IInventoryRepository inventoryRepository,
            ILogger<UpdateInventoryCommandHandler> logger)
        {
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateInventoryCommand command, CancellationToken cancellationToken)
        {
            // Retrieve the existing inventory record using shop and product IDs.
            var inventory = await _inventoryRepository.GetInventoryAsync(command.ShopId, command.ProductId);
            if (inventory == null)
            {
                _logger.LogWarning("Inventory record not found for ShopId: {ShopId}, ProductId: {ProductId}", command.ShopId, command.ProductId);
                // Throw an exception (or return a failure result) so that the global error handler returns an appropriate response.
                throw new Exception("Inventory record not found.");
            }

            try
            {
                // Update the inventory record using domain methods.
                inventory.UpdateQuantity(command.NewQuantity, command.UpdatedBy);
                inventory.ChangeStatus(command.NewStatus, command.UpdatedBy);

                // Persist the changes.
                await _inventoryRepository.UpdateAsync(inventory);

                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating inventory for ShopId: {ShopId}, ProductId: {ProductId}", command.ShopId, command.ProductId);
                throw; // Let the global exception handler take care of this error.
            }
        }
    }
}
