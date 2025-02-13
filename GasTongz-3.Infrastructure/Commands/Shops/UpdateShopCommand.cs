using _2_GasTongz.Application.Interfaces;
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
    public record UpdateShopCommand(
        int Id,               // Used to identify the shop to update
        string Name,
        string? Location,
        int? UpdatedBy) : IRequest<int>;

    public class UpdateShopCommandValidator : AbstractValidator<UpdateShopCommand>
    {
        public UpdateShopCommandValidator()
        {
            RuleFor(c => c.Id).GreaterThan(0).WithMessage("Invalid shop ID.");
            RuleFor(c => c.Name).NotEmpty().WithMessage("Shop name is required.");
            RuleFor(c => c.UpdatedBy).GreaterThan(0).When(c => c.UpdatedBy.HasValue)
                .WithMessage("UpdatedBy must be greater than zero.");
        }
    }


    public class UpdateShopCommandHandler : IRequestHandler<UpdateShopCommand, int>
    {
        private readonly IShopRepository _shopRepository;
        private readonly ILogger<UpdateShopCommandHandler> _logger;

        public UpdateShopCommandHandler(IShopRepository shopRepository, ILogger<UpdateShopCommandHandler> logger)
        {
            _shopRepository = shopRepository;
            _logger = logger;
        }

        public async Task<int> Handle(UpdateShopCommand command, CancellationToken cancellationToken)
        {
            try {
                var validator = new UpdateShopCommandValidator();
                var validationResult = await validator.ValidateAsync(command);
                if (!validationResult.IsValid)
                {
                    var valiErrors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogError("FV validation error: " + valiErrors);
                    return 0;
                }

                var existingShop = await _shopRepository.GetByIdAsync(command.Id);
                if (existingShop == null)
                {
                    _logger.LogError("Shop not found.");
                    return 0;
                }

                if (existingShop.IsDeleted)
                {
                    _logger.LogError($"Attempted to update soft-deleted shop with ID {command.Id}.");
                    return 0;
                }

                existingShop.UpdateShop(command.Name, command.Location, command.UpdatedBy);
                await _shopRepository.UpdateAsync(existingShop);

                return existingShop.Id;
            }
            catch (Exception ex){
                _logger.LogCritical("Update not behaving right");
                throw;
            }

            
        }
    }

}
