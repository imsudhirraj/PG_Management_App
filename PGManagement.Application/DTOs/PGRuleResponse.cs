namespace PGManagement.Application.DTOs;

public class PGRuleResponse
{
    public int Id { get; set; }

    public string RuleName { get; set; } = string.Empty;

    public string? RuleDescription { get; set; }
}