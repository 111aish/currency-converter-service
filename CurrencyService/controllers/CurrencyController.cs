using System.Text;
using System.Text.Json;
using CurrencyService.Schema;
using CurrencyService.Services;
using NATS.Client;

namespace CurrencyService.Controllers
{
    public class CurrencyController
    {
        private readonly IConnection _nats;
        private readonly IExchangeRateService _rates;

        public CurrencyController(IConnection nats, IExchangeRateService rates)
        {
            _nats = nats;
            _rates = rates;
        }

        public void Register()
        {
            _nats.SubscribeAsync("currency.convert", async (sender, args) =>
            {
                try
                {
                    var json = Encoding.UTF8.GetString(args.Message.Data);
                    var req = JsonSerializer.Deserialize<CurrencyConvertRequest>(json);

                    if (req == null || req.Amount < 0 || req.From.Length != 3 || req.To.Length != 3)
                    {
                        Respond(args.Message.Reply, new ErrorResponse { Error = "Invalid input" });
                        return;
                    }

                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    var rate = await _rates.GetRateAsync(req.From.ToUpper(), req.To.ToUpper(), cts.Token);
                    if (rate is null)
                    {
                        Respond(args.Message.Reply, new ErrorResponse { Error = "Rate not available" });
                        return;
                    }

                    var converted = Math.Round(req.Amount * rate.Value, 2);
                    Respond(args.Message.Reply, new CurrencyConvertResponse
                    {
                        Amount = converted,
                        From = req.From.ToUpper(),
                        To = req.To.ToUpper(),
                        Rate = Math.Round(rate.Value, 6)
                    });
                }
                catch
                {
                    Respond(args.Message.Reply, new ErrorResponse { Error = "Unexpected error" });
                }
            });
        }

        private void Respond(string? replySubject, object payload)
        {
            if (string.IsNullOrWhiteSpace(replySubject)) return;
            var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
            _nats.Publish(replySubject, bytes);
        }
    }
}