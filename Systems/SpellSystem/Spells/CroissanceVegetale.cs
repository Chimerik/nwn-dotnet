using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> CroissanceVegetale(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.CroissanceVegetaleAoE(oCaster, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));
      var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      aoe.SetRadius(50);

      return new List<NwGameObject>() { aoe };
    }
  }
}
