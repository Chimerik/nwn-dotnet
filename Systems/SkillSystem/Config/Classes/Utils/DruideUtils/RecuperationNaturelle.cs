using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class DruideUtils
  {
    public static void RecuperationNaturelle(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.DruideRecuperationNaturelle))
        creature.SetFeatRemainingUses((Feat)CustomSkill.DruideRecuperationNaturelle, (byte)Math.Round((double)(creature.GetClassInfo(ClassType.Druid).Level / 2), MidpointRounding.AwayFromZero));
    }
  }
}
