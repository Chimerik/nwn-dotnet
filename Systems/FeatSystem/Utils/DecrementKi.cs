using Anvil.API;
using Google.Apis.Discovery;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static void DecrementKi(NwCreature creature)
    {
      DecrementFeatUses(creature, CustomSkill.MonkPatience);   
      DecrementFeatUses(creature, CustomSkill.MonkDelugeDeCoups);
      DecrementFeatUses(creature, CustomSkill.MonkStunStrike);
      DecrementFeatUses(creature, CustomSkill.MonkDesertion);
      DecrementFeatUses(creature, CustomSkill.MonkExplosionKi);
      DecrementFeatUses(creature, CustomSkill.MonkPaumeVibratoire);
      DecrementFeatUses(creature, CustomSkill.MonkDarkVision);
      DecrementFeatUses(creature, CustomSkill.MonkTenebres);
      DecrementFeatUses(creature, CustomSkill.MonkPassageSansTrace);
      DecrementFeatUses(creature, CustomSkill.MonkSilence);
      DecrementFeatUses(creature, CustomSkill.MonkFrappeDombre);
    }
  }
}
