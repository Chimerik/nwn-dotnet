using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RepliqueInvoquee(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster)
        return new List<NwGameObject>();

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.GetRepliqueInvoqueeAuraEffect(caster), NwTimeSpan.FromRounds(spellEntry.duration)));

      ClercUtils.ConsumeConduitDivin(caster);

      return new List<NwGameObject>() { oCaster };
    }
  }
}
