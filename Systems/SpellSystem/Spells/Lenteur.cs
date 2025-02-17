using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Lenteur(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      List<NwGameObject> concentrationTargets = new();

      if (oCaster is not NwCreature caster)
        return concentrationTargets;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry);

      foreach (var target in targets)
      {
        if(target is NwCreature targetCreature &&
          CreatureUtils.GetSavingThrow(caster, targetCreature, spellEntry.savingThrowAbility, spellDC, spellEntry) != SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSlow));
          EffectSystem.ApplyLenteur(targetCreature, oCaster, spell, castingClass.SpellCastingAbility, SpellUtils.GetSpellDuration(oCaster, spellEntry));
          
          concentrationTargets.Add(target);
        }
      }

      return concentrationTargets;
    }
  }
}
