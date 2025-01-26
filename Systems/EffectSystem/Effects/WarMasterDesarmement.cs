using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string WarMasterDesarmementEffectTag = "_WARMASTER_DESARMEMENT_EFFECT";
    private static ScriptCallbackHandle onRemoveWarMasterDesarmementCallback;
    
    public static Effect warMasterDesarmement
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.AttackDecrease), 
          Effect.RunAction(onRemovedHandle: onRemoveWarMasterDesarmementCallback));

        eff.Tag = WarMasterDesarmementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveWarMasterDesarmement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.OnItemEquip -= ItemSystem.OnEquipDesarmement;

      if (!creature.IsLoginPlayerCharacter)
        _ = creature.ActionEquipMostDamagingMelee();

      return ScriptHandleResult.Handled;
    }
  }
}
