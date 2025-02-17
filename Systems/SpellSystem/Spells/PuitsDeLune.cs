using System.Collections.Generic;
using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> PuitsDeLune(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass)
    {
      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        EffectSystem.PuitsDeLune(caster, spell, castingClass.SpellCastingAbility, SpellUtils.GetSpellDuration(oCaster, spellEntry));
      }
      
      return new List<NwGameObject>() { oCaster };
    }
  }
}
