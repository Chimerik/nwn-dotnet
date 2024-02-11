using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    private static ScriptCallbackHandle onIntervalWildMagicEspritIntangibleCallback;
    public const string WildMagicEspritIntangibleEffectTag = "_EFFECT_WILD_MAGIC_ESPRIT_INTANGIBLE";
    public static Effect wildMagicEspritIntangible
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.DurAuraPurple), Effect.RunAction(onIntervalHandle: onIntervalWildMagicEspritIntangibleCallback, interval: NwTimeSpan.FromRounds(1)));
        eff.Tag = WildMagicEspritIntangibleEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalWildMagicEspritIntangible(CallInfo callInfo)
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

        StringUtils.DisplayStringToAllPlayersNearTarget(source, "Magie Sauvage - Esprit Intangible", StringUtils.gold, true);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionMind));

        foreach (var victims in source.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 2, false))
        {
          int advantage = CreatureUtils.GetCreatureAbilityAdvantage(victims, Ability.Dexterity);
          int totalSave = SpellUtils.GetSavingThrowRoll(victims, Ability.Dexterity, spellDC, advantage, feedback);
          bool saveFailed = totalSave < spellDC;

          SpellUtils.SendSavingThrowFeedbackMessage(source, victims, feedback, advantage, spellDC, totalSave, saveFailed, Ability.Dexterity);

          if (saveFailed)
            victims.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpMagblue), 
              Effect.Damage(NwRandom.Roll(Utils.random, 6), DamageType.Magical)));
        }

        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
