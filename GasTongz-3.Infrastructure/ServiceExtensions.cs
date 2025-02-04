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
using Commands.Transaction;
using _2_GasTongz.Application;


namespace _3_GasTongz.Infrastructure
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register MediatR using assembly marker
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly));

            // Register FluentValidation
            services.AddValidatorsFromAssemblyContaining<CreateTransactionCommandValidator>();

            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IShopRepository, ShopRepository>();

            return services;
        }

    }
}
