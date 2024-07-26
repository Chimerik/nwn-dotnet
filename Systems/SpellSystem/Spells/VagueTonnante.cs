using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void VagueTonnante(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwClass casterClass)
    {
      if (oCaster is not NwCreature caster)
        return;

      SpellUtils.SignalEventSpellCast(oCaster, oCaster, spell.SpellType);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);
      bool evocateur = caster.KnowsFeat((Feat)CustomSkill.EvocateurFaconneurDeSorts);

      caster.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlOdd));

      foreach (NwCreature target in oCaster.Location.GetObjectsInShapeByType<NwCreature>(Shape.Cube, spellEntry.aoESize, false))
      {
        if (evocateur && oCaster is NwCreature casterCreature && !casterCreature.IsReactionTypeHostile(target))
          continue;

        bool saveFailed = CreatureUtils.GetSavingThrow(caster, target, spellEntry.savingThrowAbility, spellDC);
        SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass), saveFailed);

        if (saveFailed)
          NWScript.AssignCommand(oCaster, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.knockdown, NwTimeSpan.FromRounds(spellEntry.duration)));

        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHeadNature));
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfScreenBump));
      }
    }
  }
}
