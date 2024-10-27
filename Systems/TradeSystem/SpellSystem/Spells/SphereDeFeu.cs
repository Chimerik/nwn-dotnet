using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> SphereDeFeu(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oTarget, spell.SpellType);
      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
      //NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Temporary, EffectSystem.SphereDeFeu(castingClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry)));

      return new List<NwGameObject>() { oTarget };
    }
  }
}
