using System;
using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> ToileDaraignee(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass casterClass)
    {
      List<NwGameObject> concentrationTarget = new();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
        TimeSpan duration = SpellUtils.GetSpellDuration(oCaster, spellEntry);

        targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.ToileDaraignee(caster), duration);
        var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
        aoe.GetObjectVariable<LocalVariableInt>("_DC_ABILITY").Value = (int)casterClass.SpellCastingAbility;
        concentrationTarget.Add(aoe);
      }

      return concentrationTarget;
    }
  }
}
