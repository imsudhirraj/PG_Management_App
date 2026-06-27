namespace PGManagement.Application.DTOs;

public class OwnerPaymentSettingsRequest
{
    public string AccountHolderName { get; set; } = "";

    public string UpiId { get; set; } = "";
}