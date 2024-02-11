using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using NWN.Systems;
using static NWN.Systems.SpellConfig;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static int GetCreatureSkillAdvantage(NwCreature creature, int skill)
    {
      int advantage = 0;

      switch (skill)
      {
        case CustomSkill.PerceptionProficiency:

          if (creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.TotemAspectAigle)))
            advantage += 1;

          break;
      }

      return advantage;
    }
  }
}
