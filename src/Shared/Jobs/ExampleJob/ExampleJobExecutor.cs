using Shared.DTOs;
using Shared.Interfaces;
using Shared.Services;

namespace Shared.Jobs.ExampleJob;

public class ExampleJobExecutor : JobExecutorBase<ExampleJobInputDto, ExampleJobContextDto>
{
    private readonly IUnstableServices _services;

    public ExampleJobExecutor(IUnstableServices services)
    {
        _services = services;

        // **********************************************************************
        // 1) Fondskodens data læses fra NasdaqOMX
        AddStateHandler("Read Instrument Data", ReadInstrumentDataAsync, true);
        // 2) Der sendes en email med den seneste handelskurs til brugeren
        AddStateHandler("Send Email", SendEmailAsync);
        // **********************************************************************
    }

    protected override Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    protected override Task FinishAsync()
    {
        return Task.CompletedTask;
    }

    private async Task<JobResultDto> ReadInstrumentDataAsync()
    {
        try
        {
            _context!.LogWork("Data hentet fra NasdaqOMX");

            _context!.InstrumentData = await _services.GetInstrumentAsync(_input.IsinCode!);
            return ChangeState("Send Email");
        }
        catch (Exception ex)
        {
            return Retry(ex, 5);
        }
    }

    private async Task<JobResultDto> SendEmailAsync()
    {
        try
        {
            _context!.LogWork($"Email sendt til: {_input.Email}");

            EmailDto email = new EmailDto();
            email.SendTo = _input.Email!;
            email.Subject = $"Her er kursen for: {_context?.InstrumentData?.CompanyName}";
            email.Body = $"Kursen er: {_context?.InstrumentData?.LastSalePrice}";

            await _services.SendEmailAsync(email);
            return Finish();
        }
        catch (Exception ex)
        {
            return Retry(ex, 5);
        }
    }
}
