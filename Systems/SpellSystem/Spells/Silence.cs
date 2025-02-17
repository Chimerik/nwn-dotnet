using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> Silence(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.Silence(oCaster, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
      return new List<NwGameObject>() { UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>() };
    }
  }
}
