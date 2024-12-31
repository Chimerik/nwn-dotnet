using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void FouRireDeTasha(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, castingClass.SpellCastingAbility);

      foreach (var target in targets)
      {
        if (target is NwCreature targetCreature)
          EffectSystem.ApplyFouRireDeTasha(targetCreature, caster, SpellUtils.GetSpellDuration(caster, spellEntry), spellEntry.savingThrowAbility, castingClass.SpellCastingAbility); 
      }
    }
  }
}
