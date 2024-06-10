using System;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using DamageType = Anvil.API.DamageType;
using EffectSubType = Anvil.API.EffectSubType;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ChatimentDivinEffectTag = "_CHATIMENT_DIVIN_EFFECT";
    private static ScriptCallbackHandle onIntervalChatimentDivinCallback;
    public static Effect GetChatimentDivinEffect(int spellLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.Icon(EffectIcon.DamageIncrease),
        Effect.RunAction(onIntervalHandle: onIntervalChatimentDivinCallback, interval: TimeSpan.FromSeconds(2)));

      for (int i = 0; i < spellLevel; i++)
        eff = Effect.LinkEffects(Effect.DamageIncrease((int)DamageBonus.Plus1d8, DamageType.Divine));

      eff.Tag = ChatimentDivinEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult OnIntervalChatimentDivin(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature target)
        return ScriptHandleResult.Handled;

      NwItem weapon = target.GetItemInSlot(InventorySlot.RightHand);

      if (weapon is null || !ItemUtils.IsMeleeWeapon(weapon.BaseItem))
        EffectUtils.RemoveTaggedEffect(target, ChatimentDivinEffectTag);

        return ScriptHandleResult.Handled;
    }
  }
}
