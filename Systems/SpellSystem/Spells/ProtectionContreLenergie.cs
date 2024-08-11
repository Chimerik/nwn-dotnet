using System.Collections.Generic;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ProtectionContreLenergie(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject target)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.DamageImmunityIncrease(spellEntry.damageType[0], 50), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      return new List<NwGameObject>() { target };
    }
  }
}
