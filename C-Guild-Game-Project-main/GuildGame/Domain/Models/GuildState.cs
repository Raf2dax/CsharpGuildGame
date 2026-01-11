namespace GuildGame.Domain.Models;

public class GuildState
{
    public List<Hero> Heroes { get; } = new();
    public ResourceStock Resources { get; } = new();
    public int Debt { get; private set; }
    public int Day { get; private set; } = 1;
    public List<RareItem> RareItems { get; } = new();

    public bool HasLivingHeroes => Heroes.Any(h => h.IsAlive);

    public void NextDay() => Day++;

    public void AddDebt(int amount) => Debt += amount;

    public bool CheckDefeat(out string reason)
    {
        if (!HasLivingHeroes)
        {
            reason = "Tous les héros sont morts ou gravement blessés.";
            return true;
        }

        if (Resources.Food <= 0)
        {
            reason = "Plus de nourriture pour nourrir la guilde.";
            return true;
        }

        if (Debt >= 100 || Resources.Money < -20)
        {
            reason = "La guilde croule sous les dettes.";
            return true;
        }

        reason = string.Empty;
        return false;
    }
}
