using CurrencyService.Controllers;
using CurrencyService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NATS.Client;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        services.AddHttpClient<IExchangeRateService, ExchangeRateService>();

        var natsUrl = Environment.GetEnvironmentVariable("NATS_URL") ?? "nats://localhost:4222";
        var opts = ConnectionFactory.GetDefaultOptions();
        opts.Url = natsUrl;
        var cf = new ConnectionFactory();
        var nats = cf.CreateConnection(opts);
        services.AddSingleton<IConnection>(nats);

        services.AddSingleton<CurrencyController>();
    })
    .Build();

var controller = host.Services.GetRequiredService<CurrencyController>();
controller.Register();

Console.WriteLine("CurrencyService listening on 'currency.convert'");

await host.RunAsync();