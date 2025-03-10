using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void SoinsDeGroupe(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass castingClass, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if (Utils.In(target.Race.RacialType, RacialType.Undead, RacialType.Construct) || !caster.Faction.GetMembers().Contains(target))
          continue;

        NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Instant, 
          Effect.Heal(SpellUtils.GetHealAmount(caster, target, spell, spellEntry, castingClass, spellEntry.numDice) 
            + caster.GetAbilityModifier(castingClass.SpellCastingAbility))));
        
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingG));
      }
    }  
  }
}
