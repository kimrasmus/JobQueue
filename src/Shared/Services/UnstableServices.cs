using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Services;

public class UnstableServices : IUnstableServices
{
    private static readonly HttpClient _httpClient = new();
    private static readonly Random _random = new();

    private const int MinDelayMs = 200;
    private const int MaxDelayMs = 2000;
    private const int ErrorChancePercent = 8;

    public async Task<EmailDto> SendEmailAsync(EmailDto email)
    {
        await SimulateUnstableAsync();

        return email;
    }

    public async Task<InstrumentDataDto> GetInstrumentAsync(string isinCode)
    {
        await SimulateUnstableAsync();

        string json =
            "{\"data\":{\"qdHeader\":{\"symbol\":\"JYSK\",\"companyName\":\"Jyske Bank\",\"exchange\":\"Nasdaq Copenhagen\",\"segment\":\"Large Cap\",\"marketStatus\":\"Opening Auction\",\"isin\":\"DK0010307958\",\"currency\":\"DKK\",\"lastSalePrice\":649.00,\"lastTradeTimestamp\":\"2025-07-23 07:01:49 CET\",\"primaryData\":{\"lastSalePrice\":\"DKK 649,00\",\"lastTradeTimestamp\":\"2025-07-23 07:01:49 CET\",\"netChange\":\"\",\"percentageChange\":\"0,00%\",\"deltaIndicator\":\"unch\"},\"keyStats\":{\"bid\":{\"label\":\"Bud:\",\"value\":\"\"},\"ask\":{\"label\":\"Udbud:\",\"value\":\"\"},\"volume\":{\"label\":\"Mængde:\",\"value\":\"\"},\"fiftyTwoWeekHighLow\":{\"label\":\"52 Week Range:\",\"value\":\"450,00-662,50\"},\"dayRange\":{\"label\":\"Day Range:\",\"value\":\"\"}},\"notifications\":null}},\"messages\":null,\"status\":{\"timestamp\":\"2025-07-24T09:15:05+0200\",\"rCode\":200,\"bCodeMessage\":null,\"developerMessage\":\"\"}}";

        using JsonDocument document = JsonDocument.Parse(json);
        JsonElement root = document.RootElement;

        if (
            root.TryGetProperty("data", out var dataElement)
            && dataElement.TryGetProperty("qdHeader", out var qdHeaderElement)
        )
        {
            string raw = qdHeaderElement.GetRawText();
            var dto = JsonSerializer.Deserialize<InstrumentDataDto>(
                raw,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (dto is not null)
                return dto;
        }

        throw new InvalidOperationException("qdHeader kunne ikke findes i responsen.");
    }

    private async Task SimulateUnstableAsync()
    {
        int delay = _random.Next(MinDelayMs, MaxDelayMs);
        await Task.Delay(delay);

        if (_random.Next(0, 100) < ErrorChancePercent)
        {
            throw new InvalidOperationException("Simulated Error");
        }
    }
}
