﻿using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void PassageParLesArbres(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
    }
  }
}
