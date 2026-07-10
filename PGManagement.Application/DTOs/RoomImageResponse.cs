namespace PGManagement.Application.DTOs;

public class RoomImageResponse
{
    public int Id { get; set; }

    public int RoomId { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public bool IsCover { get; set; }
}