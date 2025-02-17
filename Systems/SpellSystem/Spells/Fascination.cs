using Anvil.API;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void Fascination(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {     
      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);

      if (oCaster is not NwCreature caster)
        return;

      int DC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      foreach (var targetObject in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(targetObject is NwCreature target 
          && !target.IsInCombat 
          && !caster.Faction.GetMembers().Contains(target)
          && CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC, spellEntry) == SavingThrowResult.Failure)
        {
          target.ApplyEffect(EffectDuration.Temporary, EffectSystem.Fascination, SpellUtils.GetSpellDuration(oCaster, spellEntry));
        }
      }        
    }
  }
}
