using System.Collections.Generic;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static List<NwGameObject> FleauDinsectes(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation, NwClass castingClass)
    {
      List<NwGameObject> concentrationList = new List<NwGameObject>();

      if (oCaster is NwCreature caster)
      {
        SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
        oCaster.GetObjectVariable<LocalVariableInt>($"_SPELL_CASTING_ABILITY_{spell.Id}").Value = (int)castingClass.SpellCastingAbility;

        targetLocation.ApplyEffect(EffectDuration.Temporary, EffectSystem.FleauDinsectesAoE(caster), NwTimeSpan.FromRounds(spellEntry.duration));  
        concentrationList.Add(UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>());
      }

      return concentrationList;
    }
  }
}
