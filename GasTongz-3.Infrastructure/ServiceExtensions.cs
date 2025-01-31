using _3_GasTongz.Infrastructure.Commands;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using _2_GasTongz.Application.Interfaces;
using _3_GasTongz.Infrastructure.Repos;


namespace _3_GasTongz.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateTransactionCommandHandler).Assembly));

            // Register FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateTransactionCommandValidator>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();

            return services;
        }

    }
}
