using MassTransit;
using Microsoft.EntityFrameworkCore;
using SagaStateMachine.Service.StateInstances;
using SagaStateMachine.Service.StateMachines;

namespace SagaStateMachine.Service.Extensions
{
    public static class MassTransitExtensions
    {
        public static IServiceCollection AddCustomMassTransit<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration) where TDbContext : DbContext
        {
            services.AddMassTransit(configurator =>
            {
                configurator.AddSagaStateMachine<OrderStateMachine, OrderStateInstance>()
                .EntityFrameworkRepository(options =>
                {
                    options.AddDbContext<DbContext, TDbContext>((provider, builder) =>
                    {
                        builder.UseSqlServer(configuration.GetConnectionString("MSSQLServer"));
                    });
                });

                configurator.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ"]);
                });
            });

            return services;
        }
    }
}