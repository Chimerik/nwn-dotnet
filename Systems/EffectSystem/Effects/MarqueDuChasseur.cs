﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MarqueDuChasseurTag = "_MARQUE_DU_CHASSEUR_EFFECT";
    public static readonly Native.API.CExoString MarqueDuChasseurExoTag = MarqueDuChasseurTag.ToExoString();

    private static ScriptCallbackHandle onRemoveMarqueDuChasseur;
    public static Effect MarqueDuChasseur
    {
      get
      {
        Effect eff = Effect.RunAction(onRemovedHandle: onRemoveMarqueDuChasseur);
        eff.Tag = MarqueDuChasseurTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveMarqueDuChasseur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnDeath -= SpellSystem.OnDeathMarqueDuChasseur;

      return ScriptHandleResult.Handled;
    }

    public const string FreeMarqueDuChasseurTag = "_FREE_MARQUE_DU_CHASSEUR_EFFECT";

    public static Effect FreeMarqueDuChasseur
    {
      get
      {
        Effect eff = Effect.Icon(EffectIcon.SkillIncrease);
        eff.Tag = FreeMarqueDuChasseurTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
  }
}
