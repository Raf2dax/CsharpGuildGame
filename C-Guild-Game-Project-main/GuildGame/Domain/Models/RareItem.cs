namespace GuildGame.Domain.Models;

public class RareItem
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public RareBuffType BuffType { get; init; }
    public int Magnitude { get; init; } = 5;
    public RareItemCategory Category { get; init; }
    public int PowerBonus { get; init; } = 1;

    public string DisplayBuff => $"{BuffType} (+{Magnitude})";
}
