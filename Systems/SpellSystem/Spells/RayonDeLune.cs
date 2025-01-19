using System.Collections.Generic;
using System.Linq;
using Anvil.API;
using Anvil.Services;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> RayonDeLune(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.RayonDeLuneAura(oCaster, castingClass.SpellCastingAbility), SpellUtils.GetSpellDuration(oCaster, spellEntry));

      var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      aoe.Tag = EffectSystem.RayonDeLuneEffectTag;
      aoe.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value = (int)castingClass.SpellCastingAbility;

      return new List<NwGameObject>() { aoe };
    }
  }
}
