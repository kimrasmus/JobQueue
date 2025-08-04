using Shared.Interfaces;

namespace Shared.Jobs.ExampleJob;

public class ExampleJobInputDto : IJobInput
{
    public string? Email { get; set; }
    public string? IsinCode { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Email) && !string.IsNullOrWhiteSpace(IsinCode);
    }
}
