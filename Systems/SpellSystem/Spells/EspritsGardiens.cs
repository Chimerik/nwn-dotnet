using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> EspritsGardiens(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      oCaster.ApplyEffect(EffectDuration.Temporary, EffectSystem.EspritsGardiens(oCaster, spell.Id == CustomSpell.EspritsGardiensNecrotique ? CustomDamageType.Necrotic : DamageType.Divine), SpellUtils.GetSpellDuration(oCaster, spellEntry));

      var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      aoe.SetRadius(4.5f);

      return new List<NwGameObject>() { aoe };
    }
  }
}
