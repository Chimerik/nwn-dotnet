using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ProtectionContreLaMortEffectTag = "_PROTECTION_CONTRE_LA_MORT_EFFECT";
    private static ScriptCallbackHandle onRemoveProtectionContreLaMortCallback;

    public static Effect ProtectionContreLaMort
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.ProtectionContreLaMort), Effect.RunAction(onRemovedHandle: onRemoveProtectionContreLaMortCallback));
        eff.Tag = ProtectionContreLaMortEffectTag;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ProtectionContreLaMort);
        return eff;
      }
    }

    private static ScriptHandleResult OnRemoveProtectionContreLaMort(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        creature.Immortal = false;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
