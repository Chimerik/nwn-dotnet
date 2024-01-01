using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class FeatUtils
  {
    public static void DecrementTirArcanique(NwCreature creature)
    {
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirAffaiblissant);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirAgrippant);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirBannissement);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirChercheur);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirExplosif);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirOmbres);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirPerforant);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirAffaiblissant);
      DecrementFeatUses(creature, CustomSkill.ArcaneArcherTirEnvoutant);      
    }
  }
}
