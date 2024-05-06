using Anvil.API;

namespace NWN.Systems
{
  public static partial class BardUtils
  {
    public static void RestoreInspirationBardique(NwCreature creature)
    {
      if (creature.KnowsFeat((Feat)CustomSkill.BardInspiration))
      {
        int chaMod = creature.GetAbilityModifier(Ability.Charisma) > 0 ? creature.GetAbilityModifier(Ability.Charisma) : 1;
        creature.SetFeatRemainingUses((Feat)CustomSkill.BardInspiration, (byte)chaMod);
      }
    }
  }
}
