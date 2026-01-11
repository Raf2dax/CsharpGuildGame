namespace GuildGame.Domain.Models;

public class ResourceChange
{
    public int Money { get; set; }
    public int Food { get; set; }
    public int Medicine { get; set; }
    public int Equipment { get; set; }

    public static ResourceChange operator +(ResourceChange a, ResourceChange b) => new()
    {
        Money = a.Money + b.Money,
        Food = a.Food + b.Food,
        Medicine = a.Medicine + b.Medicine,
        Equipment = a.Equipment + b.Equipment
    };

    public static ResourceChange operator -(ResourceChange a) => new()
    {
        Money = -a.Money,
        Food = -a.Food,
        Medicine = -a.Medicine,
        Equipment = -a.Equipment
    };
}
