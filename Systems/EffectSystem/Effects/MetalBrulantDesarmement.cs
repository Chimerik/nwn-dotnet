using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string MetalBrulantDesarmementEffectTag = "_METAL_BRULANT_DESARMEMENT_EFFECT";
    private static ScriptCallbackHandle onRemoveMetalBrulantDesarmementCallback;
    public static Effect MetalBrulantDesarmement
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.MetalBrulant), Effect.RunAction(onRemovedHandle: onRemoveMetalBrulantDesarmementCallback));
        eff.Tag = MetalBrulantDesarmementEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }
    private static ScriptHandleResult OnRemoveMetalBrulantDesarmement(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature creature)
      {
        foreach (var item in creature.Inventory.Items)
        {
          if (item.GetObjectVariable<LocalVariableObject<NwGameObject>>("_METAL_BRULANT").Value == eventData.Effect.Creator)
          {
            item.GetObjectVariable<LocalVariableObject<NwGameObject>>("_METAL_BRULANT").Delete();
            creature.ControllingPlayer?.SendServerMessage($"{StringUtils.ToWhitecolor(item.Name)} n'est plus brûlant", ColorConstants.Orange);
          }
        }

        creature.OnItemEquip -= SpellSystem.OnEquipMetalBrulant;
      }

      return ScriptHandleResult.Handled;
    }
  }
}
