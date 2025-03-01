using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ApparitionAnimale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentrationList = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

        EffectSystem.CreateApparitionAnimale(caster, targetLocation, casterClass, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }

      return concentrationList;
    }
  }
}
