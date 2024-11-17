using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static void RestoreInterventionDivine(NwCreature creature)
    {
      /*byte? level = creature.GetClassInfo(ClassType.Cleric)?.Level;

      if (!level.HasValue)
        return;

      if (creature.GetObjectVariable<PersistentVariableString>("_DIVINE_INTERVENTION_COOLDOWN").HasNothing
        || DateTime.Compare(DateTime.Parse(creature.GetObjectVariable<PersistentVariableString>("_DIVINE_INTERVENTION_COOLDOWN").Value), DateTime.Now) < 0)
      {
        await NwTask.NextFrame();
        creature.SetFeatRemainingUses((Feat)CustomSkill.ClercInterventionDivine, 1);
        creature.GetObjectVariable<PersistentVariableString>("_DIVINE_INTERVENTION_COOLDOWN").Delete();
      }*/
    }
  }
}
