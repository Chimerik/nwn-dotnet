using System;
using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string BonusActionEffectTag = "_BONUS_ACTIONS_EFFECT";
    public const int BonusActionId = -1;
    private static ScriptCallbackHandle onRemoveBonusActionCallback;
    public static void ApplyActionBonus(NwCreature creature)
    {
      if(creature.ActiveEffects.Any(e => e.EffectType == EffectType.Icon && e.IntParams[0] == (int)CustomEffectIcon.NoBonusAction))
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, BonusActionId), NwTimeSpan.FromRounds(1));
      }
      else
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.ActionBonus), Effect.RunAction(onRemovedHandle: onRemoveBonusActionCallback));
        eff.Tag = BonusActionEffectTag;
        eff.SubType = EffectSubType.Unyielding;

        creature.ApplyEffect(EffectDuration.Permanent, eff);

        if (creature.IsPlayerControlled)
        {
          foreach (var feat in creature.Feats.Where(f => f.TalentMaxCR.ToBool() && creature.GetFeatRemainingUses(f) < 1))
          {
            switch (Feats2da.featTable[feat.Id].skillCategory)
            {
              case SkillSystem.Category.Manoeuvre:

                var manoeuvre = creature.Feats.FirstOrDefault(f => !f.TalentMaxCR.ToBool() && Feats2da.featTable[f.Id].skillCategory == SkillSystem.Category.Manoeuvre);

                if (manoeuvre != null)
                {
                  creature.SetFeatRemainingUses(feat, creature.GetFeatRemainingUses(manoeuvre));
                  creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
                  continue;
                }

                break;

              case SkillSystem.Category.Ki:

                var ki = creature.Feats.FirstOrDefault(f => !f.TalentMaxCR.ToBool() && Feats2da.featTable[f.Id].skillCategory == SkillSystem.Category.Ki);

                if (ki != null)
                {
                  creature.SetFeatRemainingUses(feat, creature.GetFeatRemainingUses(ki));
                  creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
                  continue;
                }

                break;
            }

            creature.SetFeatRemainingUses(feat, (byte)creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value);
            creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
          }
        }
      }
    }
    private static ScriptHandleResult OnRemoveBonusAction(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.ApplyEffect(EffectDuration.Temporary, Cooldown(creature, 6, BonusActionId), TimeSpan.FromSeconds(5.9));
      }

      return ScriptHandleResult.Handled;
    }
  }
}
