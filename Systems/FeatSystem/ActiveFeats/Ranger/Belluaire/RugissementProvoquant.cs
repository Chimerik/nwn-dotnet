using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void BelluaireRugissementProvoquant(NwCreature caster)
    {
      if (caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).HasValue)
      {
        if(caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).HasValue)
          caster.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor("Rugissement Provocant")} disponible dans {StringUtils.ToWhitecolor(caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).Value - 1)} rounds", ColorConstants.Red);
        else
        {
          var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).Value = 10;

          StringUtils.DisplayStringToAllPlayersNearTarget(companion, "Rugissement Provocant", StringUtils.gold);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);
          companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCry));
          
          foreach(var target in companion.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
          {
            if(companion.IsReactionTypeHostile(target))
            {
              int DC = SpellConfig.BaseSpellDC + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);

              if (CreatureUtils.GetSavingThrow(companion, target, Ability.Wisdom, DC) == SavingThrowResult.Failure)
                NWScript.AssignCommand(companion, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.provocation, NwTimeSpan.FromRounds(2)));
            }
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
