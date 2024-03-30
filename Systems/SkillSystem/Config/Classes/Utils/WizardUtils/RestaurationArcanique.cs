using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class WizardUtils
  {
    public static void RestaurationArcanique(NwCreature creature)
    {
      if(creature.KnowsFeat((Feat)CustomSkill.WizardRestaurationArcanique))
        creature.SetFeatRemainingUses((Feat)CustomSkill.WizardRestaurationArcanique, (byte)Math.Round((double)(creature.GetClassInfo(ClassType.Wizard).Level / 2), MidpointRounding.AwayFromZero));
    }
  }
}
