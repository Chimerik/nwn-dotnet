using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Heroisme(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      List<NwGameObject> targets = new();

      if (oCaster is NwCreature caster && oTarget is NwCreature target)
      {
        SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

        targets.Add(oTarget);

        oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHolyAid));
        NWScript.AssignCommand(caster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.Heroisme(CreatureUtils.GetAbilityModifierMin1(caster, castingClass.SpellCastingAbility)), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      }

      return targets;
    }  
  }
}
