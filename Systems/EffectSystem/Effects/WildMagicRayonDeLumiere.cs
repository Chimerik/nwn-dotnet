using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalWildMagicRayonDeLumiereCallback;
    public const string WildMagicRayonDeLumiereEffectTag = "_EFFECT_WILD_MAGIC_RAYON_DE_LUMIERE";
    public static Effect wildMagicRayonDeLumiere
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraPulseYellowWhite), Effect.RunAction(onIntervalHandle: onIntervalWildMagicRayonDeLumiereCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = WildMagicRayonDeLumiereEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalWildMagicRayonDeLumiere(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature source)
        return ScriptHandleResult.Handled;

      foreach(var target in source.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Enemy), CreatureTypeFilter.Perception(PerceptionType.Seen)))
      {
        if (target.DistanceSquared(source) > 80)
          break;

        SpellConfig.SavingThrowFeedback feedback = new();
        int spellDC = 8 + source.GetAbilityModifier(Ability.Constitution) + NativeUtils.GetCreatureProficiencyBonus(source);

        int advantage = CreatureUtils.GetCreatureAbilityAdvantage(target, Ability.Constitution);
        int totalSave = SpellUtils.GetSavingThrowRoll(target, Ability.Constitution, spellDC, advantage, feedback);
        bool saveFailed = totalSave < spellDC;

        StringUtils.DisplayStringToAllPlayersNearTarget(source, "Magie Sauvage - Rayon de Lumière", StringUtils.gold, true);
        SpellUtils.SendSavingThrowFeedbackMessage(source, target, feedback, advantage, spellDC, totalSave, saveFailed, Ability.Constitution);

        if(saveFailed)
        {
          target.ApplyEffect(EffectDuration.Instant, Effect.Damage(NwRandom.Roll(Utils.random, 6), DamageType.Positive));
          target.ApplyEffect(EffectDuration.Temporary, Effect.Blindness(), NwTimeSpan.FromRounds(1));
        }

        target.ApplyEffect(EffectDuration.Temporary, Effect.VisualEffect(VfxType.BeamHoly), TimeSpan.FromSeconds(1.7));

        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
