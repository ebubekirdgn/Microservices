using SagaStateMachine.Service.Extensions;
using SagaStateMachine.Service.StateDbContexts;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddCustomMassTransit<OrderStateDbContext>(builder.Configuration);

var host = builder.Build();
host.Run();