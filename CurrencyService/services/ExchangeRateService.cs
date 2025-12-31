using System.Net.Http;
using System.Text.Json;

namespace CurrencyService.Services
{
    public interface IExchangeRateService
    {
        Task<decimal?> GetRateAsync(string from, string to, CancellationToken ct);
    }

    public class ExchangeRateService : IExchangeRateService
    {
        private readonly HttpClient _http;

        public ExchangeRateService(HttpClient httpClient)
        {
            _http = httpClient;
        }

        public async Task<decimal?> GetRateAsync(string from, string to, CancellationToken ct)
        {
            var url = $"https://api.exchangerate-api.com/v4/latest/{Uri.EscapeDataString(from)}";
            using var res = await _http.GetAsync(url, ct);
            if (!res.IsSuccessStatusCode) return null;

            using var stream = await res.Content.ReadAsStreamAsync(ct);
            using var doc = await JsonDocument.ParseAsync(stream, cancellationToken: ct);

            if (!doc.RootElement.TryGetProperty("rates", out var rates)) return null;
            if (!rates.TryGetProperty(to, out var rateElem)) return null;

            return rateElem.TryGetDecimal(out var rate) ? rate : null;
        }
    }
}