using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> SouffleDuDragon(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new() { oTarget };

      if (oTarget is not NwCreature target || oCaster is not NwCreature caster)
        return concentrationList;

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);

      EffectUtils.RemoveTaggedEffect(oTarget, EffectSystem.SouffleDuDragonEffectTag);

      EffectSystem.ApplySouffleDuDragon(caster, target, spell, spellEntry, castingClass.SpellCastingAbility);

      return concentrationList;
    }
  }
}
