namespace GuildGame.Domain.Models;

public class ResourceStock
{
    public int Money { get; private set; } = 200;
    public int Food { get; private set; } = 60;
    public int Medicine { get; private set; } = 20;
    public int Equipment { get; private set; } = 15;

    public bool CanAfford(ResourceChange cost) =>
        Money + cost.Money >= 0 &&
        Food + cost.Food >= 0 &&
        Medicine + cost.Medicine >= 0 &&
        Equipment + cost.Equipment >= 0;

    public void Apply(ResourceChange delta)
    {
        Money += delta.Money;
        Food += delta.Food;
        Medicine += delta.Medicine;
        Equipment += delta.Equipment;
    }
}
