using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void FormeSauvage(NwCreature caster, int featId)
    {
      if (caster.ActiveEffects.Any(e => e.Tag == EffectSystem.PolymorphEffectTag && e.Creator == caster))
        EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.PolymorphEffectTag);
      else
      {
        NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(featId))));
        DruideUtils.DecrementFormeSauvage(caster);
      }
    }
  }
}
