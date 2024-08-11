using System;
using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> EspritsGardiens(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      NWScript.AssignCommand(oCaster, () => oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritsGardiens(spell.Id == CustomSpell.EspritsGardiensNecrotique ? CustomDamageType.Necrotic : DamageType.Divine), SpellUtils.GetSpellDuration(oCaster, spellEntry)));
      
      return new List<NwGameObject>() { oCaster };
    }
  }
}
