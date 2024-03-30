using Anvil.API;

namespace NWN.Systems
{
  public static partial class BarbarianUtils
  {
    public static int HandleBarbarianPuissanceIndomptable(NwCreature creature, int saveRoll)
    {
      byte? level = creature.GetClassInfo(ClassType.Barbarian)?.Level;

      if (!level.HasValue || level < 18)
        return saveRoll;

      if (saveRoll < creature.GetAbilityScore(Ability.Strength))
        return creature.GetAbilityScore(Ability.Strength);

      return saveRoll;
    }
  }
}
