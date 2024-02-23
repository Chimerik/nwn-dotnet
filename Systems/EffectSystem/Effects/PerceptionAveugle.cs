
using System.Linq;

using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string PerceptionAveugleAuraEffectTag = "_PERCEPTION_AVEUGLE_AURA_EFFECT";
    private static ScriptCallbackHandle onEnterPerceptionAveugleCallback;
    
    public static Effect perceptionAveugleAura
    {
      get
      {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)184, onEnterHandle: onEnterPerceptionAveugleCallback);
        eff.Tag = PerceptionAveugleAuraEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
      }
    }
    private static ScriptHandleResult onEnterPerceptionAveugle(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1 || entering == protector
        || !entering.IsReactionTypeHostile(protector) || protector.ActiveEffects.Any(e => e.EffectType == EffectType.Deaf))
        return ScriptHandleResult.Handled;

      bool inviDispel = false;
      if (entering.StealthModeActive)
      {
        entering.SetActionMode(ActionMode.Stealth, false);
        inviDispel = true;
      }

      foreach(var eff in entering.ActiveEffects)
      {
        switch(eff.EffectType) 
        {
          case EffectType.Invisibility:
          case EffectType.ImprovedInvisibility: 
            entering.RemoveEffect(eff);
            inviDispel = true;
            break;
        }
      }

      if(inviDispel)
        StringUtils.DisplayStringToAllPlayersNearTarget(protector, "Perception Aveugle", StringUtils.gold, true);

      return ScriptHandleResult.Handled;
    }
  }
}
