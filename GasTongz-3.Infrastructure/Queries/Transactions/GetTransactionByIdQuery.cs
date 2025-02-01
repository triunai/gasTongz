using _1_GasTongz.Domain.Entities;
using _2_GasTongz.Application.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Queries.Transactions
{
    // 1. Query: Define the request object
    public record GetTransactionByIdQuery(int TransactionId) : IRequest<Transaction?>;

    // 2. FluentValidator: Validate the query data
    public class GetTransactionByIdQueryValidator : AbstractValidator<GetTransactionByIdQuery>
    {
        public GetTransactionByIdQueryValidator()
        {
            RuleFor(q => q.TransactionId)
                .GreaterThan(0)
                .WithMessage("TransactionId must be greater than zero.");
        }
    }

    // 3. Handler: Process the query by calling the repository
    public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, Transaction?>
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ILogger<GetTransactionByIdQueryHandler> _logger;

        public GetTransactionByIdQueryHandler(
            ITransactionRepository transactionRepository,
            ILogger<GetTransactionByIdQueryHandler> logger)
        {
            _transactionRepository = transactionRepository;
            _logger = logger;
        }

        public async Task<Transaction?> Handle(GetTransactionByIdQuery query, CancellationToken cancellationToken)
        {
            try
            {
                // Use the repository to get the transaction by its ID.
                var transaction = await _transactionRepository.GetByIdAsync(query.TransactionId);
                return transaction;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving transaction with ID {TransactionId}", query.TransactionId);
                throw; // Let the global exception handler catch and process this error.
            }
        }
    }
}
