﻿
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Clignotement(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      caster.LoginPlayer?.SendServerMessage("Sort non implémenté", ColorConstants.Red);
    }
  }
}