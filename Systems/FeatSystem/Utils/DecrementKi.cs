using Anvil.API;

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
    }
  }
}
