namespace PGManagement.Application.DTOs;

public class PGImageResponse
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsCover { get; set; }

    public int DisplayOrder { get; set; }

    public string? Caption { get; set; }
}