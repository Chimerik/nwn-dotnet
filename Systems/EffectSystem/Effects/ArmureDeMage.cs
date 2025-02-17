using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ArmureDeMageEffectTag = "_ARMURE_DE_MAGE_EFFECT";
    private static ScriptCallbackHandle onIntervalArmureDeMageCallback;
    public static Effect ArmureDeMage
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.ACIncrease(3), Effect.RunAction(onIntervalHandle: onIntervalArmureDeMageCallback, interval:NwTimeSpan.FromRounds(1)));
        eff.Tag = ArmureDeMageEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        eff.Spell = NwSpell.FromSpellId(CustomSpell.ArmureDeMage);
        return eff;
      }
    }
    private static ScriptHandleResult OnIntervalArmureDeMage(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is NwCreature caster)
      {
        NwItem armor = caster.GetItemInSlot(InventorySlot.Chest);

        if (armor is not null && armor.BaseACValue > 0)
          EffectUtils.RemoveTaggedEffect(caster, ArmureDeMageEffectTag);
      }
      else if (eventData.EffectTarget is NwGameObject oTarget)
        oTarget.RemoveEffect(eventData.Effect);

      return ScriptHandleResult.Handled;
    }
  }
}
