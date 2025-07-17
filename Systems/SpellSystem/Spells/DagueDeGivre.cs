using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class SpellSystem
  {
    public static void DagueDeGivre(NwGameObject oCaster, NwSpell spell, SpellEntry spellEntry, NwGameObject oTarget, NwClass casterClass, NwFeat feat = null)
    {
      if (oCaster is not NwCreature caster)
        return;

      if (feat is not null && feat.Id == CustomSkill.MonkDagueDeGivre)
      {
        caster.IncrementRemainingFeatUses(feat);
        FeatUtils.DecrementKi(caster, 2);
        casterClass = NwClass.FromClassId(CustomClass.Monk);
      }

      SpellUtils.SignalEventSpellCast(oTarget, oCaster, spell.SpellType);
      bool hit = true;
      int nbDice = SpellUtils.GetSpellDamageDiceNumber(oCaster, spell);
      int spellDC = SpellUtils.GetCasterSpellDC(oCaster, spell, casterClass.SpellCastingAbility);

      switch (SpellUtils.GetSpellAttackRoll(oTarget, oCaster, spell, casterClass.SpellCastingAbility))
      {
        case TouchAttackResult.CriticalHit: nbDice = SpellUtils.GetCriticalSpellDamageDiceNumber(oCaster, spellEntry, nbDice); break;
        case TouchAttackResult.Hit: break;
        default: hit = false; break;
      }

      if(hit)
      {
        oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ComHitFrost));
        NWScript.AssignCommand(oCaster, () => oTarget.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 10), DamageType.Cold)));
      } 
      
      foreach(var target in  oTarget.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, spellEntry.aoESize, false)) 
      {
        if(CreatureUtils.GetSavingThrowResult(target, spellEntry.savingThrowAbility, caster, spellDC) == SavingThrowResult.Failure)
          SpellUtils.DealSpellDamage(target, oCaster.CasterLevel, spellEntry, SpellUtils.GetSpellDamageDiceNumber(oCaster, spell), oCaster, spell.GetSpellLevelForClass(casterClass));

        oTarget.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpFrostS));
      }

      NWScript.AssignCommand(caster, () => oTarget.Location.ApplyEffect(EffectDuration.Temporary, EffectSystem.SurfaceDeGlace(spellEntry.aoESize), NwTimeSpan.FromRounds(spellEntry.duration)));
    }
  }
}
