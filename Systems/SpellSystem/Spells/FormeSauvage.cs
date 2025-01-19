using System;
using System.Linq;
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

      var previousPolymorph = caster.ActiveEffects.FirstOrDefault(e => e.EffectType == EffectType.Polymorph);

      if (previousPolymorph is not null)
      {
        caster.RemoveEffect(previousPolymorph);

        await NwTask.Delay(TimeSpan.FromSeconds(1));

        if (caster is null || !caster.IsValid)
          return;
      }

      NWScript.AssignCommand(caster, () => caster.ApplyEffect(EffectDuration.Permanent, EffectSystem.Polymorph(caster, EffectSystem.GetPolymorphType(spell.Id))));

      if(!caster.KnowsFeat((Feat)CustomSkill.DruideFormeDeLune))
        caster.IncrementRemainingFeatUses(spell.MasterSpell.FeatReference);
      
      DruideUtils.DecrementFormeSauvage(caster);
    }
  }
}
