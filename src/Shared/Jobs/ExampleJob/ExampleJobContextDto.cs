using Shared.DTOs;
using Shared.Interfaces;

namespace Shared.Jobs.ExampleJob;

public class ExampleJobContextDto : JobContextBase
{
    public InstrumentDataDto? InstrumentData { get; set; }
}
