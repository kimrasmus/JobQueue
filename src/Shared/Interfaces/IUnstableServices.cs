using Shared.DTOs;

namespace Shared.Interfaces;

public interface IUnstableServices
{
    Task<EmailDto> SendEmailAsync(EmailDto email);
    Task<InstrumentDataDto> GetInstrumentAsync(string isinCode);
}
