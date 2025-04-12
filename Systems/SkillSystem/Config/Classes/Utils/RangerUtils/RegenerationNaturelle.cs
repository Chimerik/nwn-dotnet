using Anvil.API;

namespace NWN.Systems
{
  public static partial class RangerUtils
  {
    public static void RegenerationNaturelle(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.RegenerationNaturelle))
        creature.SetFeatRemainingUses((Feat)CustomSkill.RegenerationNaturelle, CreatureUtils.GetAbilityModifierMin1(creature, Ability.Wisdom));
    }
  }
}
