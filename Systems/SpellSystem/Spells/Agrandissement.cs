using Anvil.API;
using System.Collections.Generic;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Agrandissement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targets = SpellUtils.GetSpellTargets(oCaster, oTarget, spellEntry, true);

      foreach (var target in targets)
      {
        if(spell.Id == CustomSpell.Agrandissement)
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Agrandissement(target, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        else
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Rapetissement(target, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }

      return targets;
    }
  }
}
