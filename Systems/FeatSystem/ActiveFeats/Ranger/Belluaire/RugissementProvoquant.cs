using Anvil.API;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BelluaireRugissementProvoquant(NwCreature caster)
    {
      var companion = caster.GetAssociate(AssociateType.AnimalCompanion);

      if (companion is not null)
      {
        StringUtils.DisplayStringToAllPlayersNearTarget(companion, "Rugissement Provocant", StringUtils.gold);

        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);
        companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCry));
        caster.ApplyEffect(EffectDuration.Temporary, EffectSystem.Cooldown(caster, 60, CustomSkill.BelluaireRugissementProvoquant));
          
        foreach(var target in companion.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
        {
          if(companion.IsReactionTypeHostile(target))
          {
            int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);

            if (CreatureUtils.GetSavingThrowResult(target, Ability.Wisdom, companion, DC) == SavingThrowResult.Failure)
              EffectSystem.ApplyProvocation(caster, target, NwTimeSpan.FromRounds(2));
          }
        }
      }
      else
      {
        caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);
        caster.LoginPlayer?.SendServerMessage("Votre compagnon animal n'est pas invoqué", ColorConstants.Red);
      }
    }
  }
}
