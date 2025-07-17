using Anvil.API;
using Ability = Anvil.API.Ability;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureAbilityAdvantage(NwCreature creature, Ability ability, int skill = -1)
    {
      bool advantage = ComputeCreatureAbilityAdvantage(creature, ability, skill);
      bool disadvantage = ComputeCreatureAbilityDisadvantage(creature, ability, skill);

      if (advantage)
      {
        if (disadvantage)
          return 0;

        return 1;
      }

      if (disadvantage)
        return -1;

      return 0;
    }
  }
}
