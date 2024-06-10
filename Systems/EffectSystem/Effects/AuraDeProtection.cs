﻿using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string AuraDeProtectionEffectTag = "_AURA_DE_PROTECTION_EFFECT";
    public const string ProtectionEffectTag = "_PROTECTION_EFFECT";
    private static ScriptCallbackHandle onEnterAuraDeProtectionCallback;
    private static ScriptCallbackHandle onExitAuraDeProtectionCallback;

    public static Effect GetAuraDeProtectionEffect(int paladinLevel)
    {
        Effect eff = Effect.AreaOfEffect((PersistentVfxType)(paladinLevel < 18 ? 185 : 189), onEnterHandle: onEnterAuraDeProtectionCallback, onExitHandle: onExitAuraDeProtectionCallback);
        eff.Tag = AuraDeProtectionEffectTag;
        eff.SubType = EffectSubType.Unyielding;
        return eff;
    }
    public static Effect GetProtectionEffect(int charismaModifier)
    {
      Effect eff = Effect.SavingThrowIncrease(SavingThrow.All, charismaModifier > 1 ? charismaModifier : 1);
      eff.Tag = ProtectionEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterProtectionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, GetProtectionEffect(entering.GetAbilityModifier(Ability.Charisma))));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitProtectionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector || exiting.IsReactionTypeHostile(protector))
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProtectionEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
