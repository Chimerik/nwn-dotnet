using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void CharmePlanteEtAnimaux(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry)
    {
      if (oCaster is not NwCreature caster) 
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfLosHoly10));

      foreach (NwCreature target in caster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false))
      {
        if(target.Race.RacialType == RacialType.Animal && target.Master is null && caster.IsReactionTypeHostile(target) && !EffectSystem.IsCharmeImmune(caster, target)
          && !CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, DC))
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpSunstrike));

          if(caster.KnowsFeat((Feat)CustomSkill.ClercMaitreDeLaNature))
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Temporary, Effect.Dominated(), NwTimeSpan.FromRounds(spellEntry.duration)));
          else
            target.ApplyEffect(EffectDuration.Temporary, Effect.Pacified(), NwTimeSpan.FromRounds(spellEntry.duration));
        }
      }

      ClercUtils.ConsumeConduitDivin(caster);
    }
  }
}
