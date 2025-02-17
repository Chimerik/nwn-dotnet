using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> TempeteDeNeige(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.TempeteDeNeige(oCaster, spell), SpellUtils.GetSpellDuration(oCaster, spellEntry));

      var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
      aoe.Tag = EffectSystem.TempeteDeNeigeEffectTag;
      aoe.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value = (int)castingClass.SpellCastingAbility;   

      return new List<NwGameObject>() { aoe };
    }
  }
}
