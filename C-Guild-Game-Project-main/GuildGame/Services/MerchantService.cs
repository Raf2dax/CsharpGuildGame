using GuildGame.Domain.Models;

namespace GuildGame.Services;

public class MerchantService
{
    private readonly GuildState _guild;
    private readonly Random _random;

    public MerchantService(GuildState guild, Random? random = null)
    {
        _guild = guild;
        _random = random ?? new Random();
    }

    public IEnumerable<MerchantOffer> GetDailyOffers()
    {
        yield return new MerchantOffer
        {
            Name = "Pack de nourriture",
            Cost = new ResourceChange { Money = -6 },
            Gain = new ResourceChange { Food = 8 }
        };

        yield return new MerchantOffer
        {
            Name = "Trousse de soins",
            Cost = new ResourceChange { Money = -5 },
            Gain = new ResourceChange { Medicine = 2 }
        };

        yield return new MerchantOffer
        {
            Name = "Armes solides",
            Cost = new ResourceChange { Money = -8 },
            Gain = new ResourceChange { Equipment = 3 }
        };
    }

    public bool ExecuteOffer(MerchantOffer offer)
    {
        if (!_guild.Resources.CanAfford(offer.Cost))
        {
            return false;
        }

        _guild.Resources.Apply(offer.Cost);
        _guild.Resources.Apply(offer.Gain);

        if (offer.RecruitFactory != null)
        {
            _guild.Heroes.Add(offer.RecruitFactory());
        }

        return true;
    }
}
