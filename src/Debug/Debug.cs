using System.Text.Json;
using Shared.DTOs;
using Shared.Entities;
using Shared.Interfaces;
using Shared.Services;

public class Debug
{
    private readonly IJobQueueService _svc;

    public Debug(IJobQueueService svc)
    {
        _svc = svc;
    }

    public async Task RunAsync()
    {
        int loops = 1;
        int orders = 10000;

        for (int x = 0; x < loops; x++)
        {
            List<JobQueueItemDto> dtoList = new List<JobQueueItemDto>();

            for (int i = 0; i < orders; i++)
            {
                string email = GenerateRandomEmail();
                int fondskode = GenerateRandom(10000000, 99999999);
                string json = $"{{\"Email\":\"{email}\", \"IsinCode\":\"DK00{fondskode}\"}}";

                JobQueueItemDto dto = new JobQueueItemDto
                {
                    QueueId = GenerateRandom(1, 3),
                    JobType = "ExampleJob",
                    InputPayload = json,
                    ScheduledAt = DateTime.UtcNow,
                };

                dtoList.Add(dto);
            }

            await _svc.CreateAsync(dtoList);
        }
    }

    public static int GenerateRandom(int min, int max)
    {
        Random random = new Random();
        return random.Next(min, max + 1);
    }

    public string GenerateRandomEmail()
    {
        Random random = new Random();
        string[] domains = { "example.com", "testmail.com", "demo.org", "mail.dk" };
        string chars = "abcdefghijklmnopqrstuvwxyz0123456789";

        string localPart = new string(
            Enumerable.Range(0, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray()
        );

        string domain = domains[random.Next(domains.Length)];

        return $"{localPart}@{domain}";
    }
}
