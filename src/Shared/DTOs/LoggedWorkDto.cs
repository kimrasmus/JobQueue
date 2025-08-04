namespace Shared.DTOs;

public class LoggedWorkDto
{
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public string Work { get; set; } = String.Empty;
}
