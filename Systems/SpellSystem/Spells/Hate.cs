using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Hate(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget)
    {
      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      List<NwGameObject> targetList = new();

      if (oTarget is not NwCreature target || oCaster is not NwCreature caster || target.IsReactionTypeHostile(caster))
        return targetList;

      targetList.Add(target);
      NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Hate, NwTimeSpan.FromRounds(spellEntry.duration)));

      return targetList;
    }
  }
}
