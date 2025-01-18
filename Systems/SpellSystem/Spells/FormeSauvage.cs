using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static async void FormeSauvage(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;
        
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(caster, caster, EffectSystem.PolymorphEffectTag);

      await NwTask.WaitUntil(() => caster is null || !caster.IsValid || caster.GetObjectVariable<PersistentVariableInt>("_SHAPECHANGE_CURRENT_HP").HasNothing);

      if (caster is null || !caster.IsValid)
        return;

      await NwTask.Delay(TimeSpan.FromSeconds(0.5));

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(spell.Id))));

      caster.IncrementRemainingFeatUses(spell.MasterSpell.FeatReference);
      DruideUtils.DecrementFormeSauvage(caster);
    }
  }
}
