namespace GuildGame.Domain.Models;

public class MerchantOffer
{
    public string Name { get; init; } = string.Empty;
    public ResourceChange Cost { get; init; } = new();
    public ResourceChange Gain { get; init; } = new();
    public Func<Hero>? RecruitFactory { get; init; }
}
