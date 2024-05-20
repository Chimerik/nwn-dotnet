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
        if(caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeCoolDownVariable).HasValue)
          caster.LoginPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor("Rugissement Provoquant")} disponible dans {StringUtils.ToWhitecolor(10 - caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.FurieBestialeCoolDownVariable).Value)} rounds", ColorConstants.Red);
        else
        {
          var companion = caster.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.AnimalCompanionVariable).Value;
          caster.GetObjectVariable<LocalVariableInt>(CreatureUtils.BelluaireRugissementProvoquantCoolDownVariable).Value = -1;

          StringUtils.DisplayStringToAllPlayersNearTarget(companion, "Rugissement Provoquant", StringUtils.gold);

          caster.SetFeatRemainingUses((Feat)CustomSkill.BelluaireRugissementProvoquant, 0);
          companion.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfHowlWarCry));
          
          foreach(var target in companion.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, false))
          {
            if(companion.IsReactionTypeHostile(target))
            {
              SpellConfig.SavingThrowFeedback feedback = new();
              int DC = 8 + NativeUtils.GetCreatureProficiencyBonus(caster) + caster.GetAbilityModifier(Ability.Wisdom);
              int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Wisdom);
              int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Wisdom, DC, advantage, feedback);
              bool saveFailed = totalSave < DC;

              if (saveFailed)
                NWScript.AssignCommand(companion, () => target.ApplyEffect(EffectDuration.Temporary, EffectSystem.provocation, NwTimeSpan.FromRounds(2)));

              TrapUtils.SendSavingThrowFeedbackMessage(target, feedback.saveRoll, feedback.proficiencyBonus, advantage, DC, totalSave, saveFailed, Ability.Wisdom);
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
