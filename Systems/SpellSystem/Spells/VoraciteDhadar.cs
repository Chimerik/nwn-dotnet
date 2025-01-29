using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> VoraciteDhadar(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.VoracitedHadar(oCaster), SpellUtils.GetSpellDuration(oCaster, spellEntry));

      var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      aoe.Tag = EffectSystem.VoracitedHadarEffectTag;
      aoe.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value = (int)castingClass.SpellCastingAbility;

      targetLocation.ApplyEffect(EffectDuration.Temporary, Effect.AreaOfEffect(PersistentVfxType.PerDarkness), SpellUtils.GetSpellDuration(oCaster, spellEntry));
      var darkness = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      darkness.Tag = EffectSystem.VoracitedHadarEffectTag;

      return new List<NwGameObject>() { aoe, darkness };
    }
  }
}
