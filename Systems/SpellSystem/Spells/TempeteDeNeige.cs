using System.Collections.Generic;
using Anvil.API;
using NWN.Core;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> TempeteDeNeige(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

        NWScript.AssignCommand(oCaster, () => targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.TempeteDeNeige, SpellUtils.GetSpellDuration(oCaster, spellEntry)));

        var aoe = UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>();
        aoe.GetObjectVariable<LocalVariableInt>("_SPELL_CASTING_ABILITY").Value = (int)castingClass.SpellCastingAbility;
        concentrationList.Add(aoe);
      }

      return concentrationList;
    }
  }
}
