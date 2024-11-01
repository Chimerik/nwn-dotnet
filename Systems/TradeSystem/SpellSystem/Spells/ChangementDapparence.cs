using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ChangementDapparence(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");
      }
      
      return new List<NwGameObject>() { oCaster };
    }
  }
}
