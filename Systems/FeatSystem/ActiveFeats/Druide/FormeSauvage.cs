using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void FormeSauvage(NwCreature caster, int featId, byte formeSauvageCharges = 1)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.PolymorphEffectTag);

      await NwTask.WaitUntil(() => caster is null || !caster.IsValid || caster.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").HasNothing);

      if (caster is null || !caster.IsValid)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(featId))));
      DruideUtils.DecrementFormeSauvage(caster, formeSauvageCharges);
    }
  }
}
