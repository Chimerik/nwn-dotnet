using System;
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static async void FormeSauvage(NwCreature caster, int featId)
    {
      if (!caster.KnowsFeat((Feat)CustomSkill.DruideFormeDeLune) || caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value < 1)
      {
        await caster.ActionCastFakeSpellAt(Spell.PolymorphSelf, caster);
        await NwTask.Delay(TimeSpan.FromSeconds(3));
      }
      else
        caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BonusActionVariable).Value -= 1;

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
