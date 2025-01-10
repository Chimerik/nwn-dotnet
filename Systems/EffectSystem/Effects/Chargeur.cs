using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Native.API;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChargeurEffectTag = "_CHARGEUR_EFFECT";
    public const string ChargerVariable = "_CHARGER_INITIAL_LOCATION";
    public static readonly CExoString chargerVariableExo = ChargerVariable.ToExoString();
    private static ScriptCallbackHandle onRemoveChargeurCallback;
    public static Effect Chargeur
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(50), Effect.RunAction(onRemovedHandle:onRemoveChargeurCallback));
        eff.Tag = ChargeurEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveChargeur(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.Commandable = true;

      return ScriptHandleResult.Handled;
    }
  }
}
