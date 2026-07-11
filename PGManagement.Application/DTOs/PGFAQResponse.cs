namespace PGManagement.Application.DTOs;

public class PGFAQResponse
{
    public int Id { get; set; }

    public string Question { get; set; } = string.Empty;

    public string Answer { get; set; } = string.Empty;
}