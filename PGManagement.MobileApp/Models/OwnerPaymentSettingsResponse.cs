namespace PGManagement.MobileApp.Models;

public class OwnerPaymentSettingsResponse
{
    public int Id { get; set; }

    public string OwnerId { get; set; } = "";

    public string AccountHolderName { get; set; } = "";

    public string UpiId { get; set; } = "";

    public bool IsActive { get; set; }
}