using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> CapeDuCroise(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is NwCreature caster)
      {
        oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.CapeDuCroiseAura(caster), SpellUtils.GetSpellDuration(oCaster, spellEntry));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(9);
      }

      return new List<NwGameObject>() { oCaster };
    }
  }
}
