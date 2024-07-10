using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> LianeAvide(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, Location targetLocation)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
        caster.LoginPlayer?.SendServerMessage("Sort non implémenté pour le moment");

      return concentrationList;
    }
  }
}
