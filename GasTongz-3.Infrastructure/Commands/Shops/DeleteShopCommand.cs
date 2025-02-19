﻿using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Repos;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3_GasTongz.Infrastructure.Commands.Shops
{
    public record DeleteShopCommand(int Id) : IRequest<Unit>;

    public class DeleteShopCommandValidator : AbstractValidator<DeleteShopCommand>
    {
        public DeleteShopCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Invalid shop ID.");
        }
    }

    public class DeleteShopCommandHandler : IRequestHandler<DeleteShopCommand, Unit>
    {
        private readonly IShopRepository _shopRepository;
        private readonly IInventoryRepository _inventoryRepository;
        private readonly ILogger<DeleteShopCommandHandler> _logger;

        public DeleteShopCommandHandler(IShopRepository shopRepository, ILogger<DeleteShopCommandHandler> logger, IInventoryRepository inventoryRepository)
        {
            _shopRepository = shopRepository;
            _inventoryRepository = inventoryRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteShopCommand command, CancellationToken cancellationToken)
        {
            try 
            {
                var validator = new DeleteShopCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    _logger.LogError($"Validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}");
                    return Unit.Value;
                }

                var existingShop = await _shopRepository.GetByIdAsync(command.Id);
                if (existingShop == null)
                {
                    _logger.LogError($"Shop with ID {command.Id} not found.");
                    return Unit.Value;
                }

                if (existingShop.IsDeleted)
                {
                    _logger.LogError($"Attempted to delete already deleted shop with ID {command.Id}.");
                    return Unit.Value;
                }

                await _shopRepository.DeleteAsync(command.Id);
                await _inventoryRepository.SoftDeleteByShopIdAsync(command.Id);
                _logger.LogInformation($"Shop with ID {command.Id} marked as deleted.");
                return Unit.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shop.");
                throw; 
            }
            
        }
    }
}
