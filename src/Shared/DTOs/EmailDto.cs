namespace Shared.DTOs;

public class EmailDto
{
    public string SendTo { get; set; } = String.Empty;
    public string Subject { get; set; } = String.Empty;
    public string Body { get; set; } = String.Empty;
}
