using Anvil.API;
using System;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void LueurEtoilee(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        target.ApplyEffect(EffectDuration.Temporary, Effect.Beam(VfxType.BeamHoly, oCaster, BodyNode.Hand), TimeSpan.FromSeconds(1.7));

        int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);

        var attackResult = SpellUtils.GetSpellAttackRoll(target, oCaster, spell, casterClass.SpellCastingAbility);

        if (attackResult == TouchAttackResult.Miss)
          continue;

        if(attackResult == TouchAttackResult.CriticalHit)
          nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice);

        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, nbDice, oCaster, spell.GetSpellLevelForClass(casterClass));
        target.ApplyEffect(EffectDuration.Temporary, EffectSystem.LueurEtoilee, NwTimeSpan.FromRounds(spellEntry.duration));
        EffectUtils.RemoveEffectType(target, EffectType.Invisibility, EffectType.ImprovedInvisibility);

        if(target is NwCreature targetCreature)
          targetCreature.SetActionMode(ActionMode.Stealth, false);
      }
    }
  }
}
