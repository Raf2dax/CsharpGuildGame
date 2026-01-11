namespace GuildGame.Domain.Models;

[Flags]
public enum Trait
{
    Aucun = 0,
    Brave = 1 << 0,
    Prudent = 1 << 1,
    Chanceux = 1 << 2,
    Malchanceux = 1 << 3,
    Agile = 1 << 4,
    Solide = 1 << 5
}

public static class TraitExtensions
{
    public static string ToDisplayString(this Trait traits)
    {
        if (traits == Trait.Aucun) return "Aucun";
        var parts = new List<string>();
        if (traits.HasFlag(Trait.Brave)) parts.Add("Brave");
        if (traits.HasFlag(Trait.Prudent)) parts.Add("Prudent");
        if (traits.HasFlag(Trait.Chanceux)) parts.Add("Chanceux");
        if (traits.HasFlag(Trait.Malchanceux)) parts.Add("Malchanceux");
        if (traits.HasFlag(Trait.Agile)) parts.Add("Agile");
        if (traits.HasFlag(Trait.Solide)) parts.Add("Solide");
        return string.Join(", ", parts);
    }
}
