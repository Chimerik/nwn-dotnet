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

      StringUtils.DisplayStringToAllPlayersNearTarget(source, "Magie Sauvage - Esprit Intangible", StringUtils.gold, true);

      foreach (NwCreature target in source.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 9, true))
      {
        if (target.HP < 1 || !source.IsEnemy(target) || !source.IsCreatureSeen(target))
          continue;

        int spellDC = SpellConfig.BaseSpellDC + source.GetAbilityModifier(Ability.Constitution) + NativeUtils.GetCreatureProficiencyBonus(source);
        target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.FnfGasExplosionMind));

        foreach (var victims in source.Location.GetObjectsInShapeByType<NwCreature>(Shape.Sphere, 2, false))
        {
          if (CreatureUtils.GetSavingThrow(source, victims, Ability.Dexterity, spellDC) == SavingThrowResult.Failure)
            victims.ApplyEffect(EffectDuration.Instant, Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpMagblue), 
              Effect.Damage(NwRandom.Roll(Utils.random, 6), DamageType.Magical)));
        }

        return ScriptHandleResult.Handled;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
