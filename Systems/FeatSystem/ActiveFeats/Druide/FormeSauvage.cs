using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FormeSauvage(NwCreature caster, int featId, byte formeSauvageCharges = 1)
    {
      if (!CreatureUtils.HandleBonusActionUse(caster))
        return;

      EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.PolymorphEffectTag);

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(featId))));
      DruideUtils.DecrementFormeSauvage(caster, formeSauvageCharges);
    }
  }
}
