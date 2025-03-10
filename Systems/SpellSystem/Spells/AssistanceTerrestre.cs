using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void AssistanceTerrestre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, Location targetLocation)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, Ability.Wisdom);
      caster.IncrementRemainingFeatUses((Feat)CustomSkill.DruideAssistanceTerrestre);
      DruideUtils.DecrementFormeSauvage(caster);

      foreach (var target in targetLocation.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        int nbDices = spellEntry.numDice + (caster.GetClassInfo(ClassType.Druid).Level > 9).ToInt() + (caster.GetClassInfo(ClassType.Druid).Level > 13).ToInt();

        if (caster.IsReactionTypeHostile(target))
        { 
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpAcidS));
          SpellUtils.DealSpellDamage(target, caster.GetClassInfo(ClassType.Druid).Level, spellEntry, nbDices, caster, 2,
            CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC, spellEntry));           
        }
        else
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHealingM));
          NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Heal(SpellUtils.GetHealAmount(caster, target, spell, spellEntry, NwClass.FromClassId(CustomClass.Druid), nbDices))));
        }
      }
    }
  }
}
