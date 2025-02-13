using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Commands.Shops;
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
    public record CreateShopCommand(
        string Name,
        string Location,
        int? CreatedBy) : IRequest<int>;
}

public class CreateShopCommandValidator : AbstractValidator<CreateShopCommand>
{
    public CreateShopCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Shop name is required.");

        RuleFor(c => c.Location).NotEmpty().WithMessage("Location is required.");

        RuleFor(c => c.CreatedBy).GreaterThan(0).When(c => c.CreatedBy.HasValue)
            .WithMessage("CreatedBy must be greater than zero.");
    }
}

public class CreateShopCommandHandler : IRequestHandler<CreateShopCommand, int>
{
    private readonly IShopRepository _shopRepository;
    private readonly ILogger<CreateShopCommandHandler> _logger;

    public CreateShopCommandHandler(IShopRepository shopRepository, ILogger<CreateShopCommandHandler> logger)
    {
        _shopRepository = shopRepository;
        _logger = logger;
    }

    public async Task<int> Handle(CreateShopCommand command, CancellationToken cancellationToken)
    {
        try 
        {
            var validator = new CreateShopCommandValidator();
            var validationResult = await validator.ValidateAsync(command);
            if (!validationResult.IsValid)
            {
                _logger.LogWarning("fluent validation failed");
                return 0;
            }

            var existingShop = await _shopRepository.GetByNameAsync(command.Name);
            if (existingShop != null)
            {
                _logger.LogWarning("A shop with this name already exists.");
                return 0;
            }
            // Use the public constructor to create the Shop instance
            var shop = new Shop(
                command.Name,
                command.Location,
                command.CreatedBy);

            return await _shopRepository.CreateAsync(shop);
        }
        

        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating shop.");
            throw;
        }
    }

}

