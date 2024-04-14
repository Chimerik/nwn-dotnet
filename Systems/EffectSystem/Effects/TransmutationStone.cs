using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string TransmutationStoneEffectTag = "_TRANSMUTATION_STONE_EFFECT_";
    private static ScriptCallbackHandle onRemoveTemporaryConSaveCallback;
    public static Effect GetTransmutationStoneEffect(NwCreature target, NwItem stone, int effectId)
    {
      Effect eff;

      switch (effectId)
      {
        default:
          eff = Effect.LinkEffects(Effect.MovementSpeedIncrease(15), Effect.Icon(EffectIcon.MovementSpeedIncrease));
          target.ApplyEffect(EffectDuration.Instant, Effect.VisualEffect(VfxType.ImpHaste));
          break;

        case 1:
          target.AddFeat((Feat)CustomSkill.TemporaryConstitutionSaveProficiency);
          eff = Effect.LinkEffects(Effect.RunAction(onRemovedHandle: onRemoveTemporaryConSaveCallback), Effect.Icon(EffectIcon.FortSaveIncreased));
          break;

        case 2: eff = Effect.LinkEffects(Effect.Ultravision(), Effect.VisualEffect(VfxType.DurUltravision)); break;

        case 3: eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Acid, 50), Effect.Icon(EffectIcon.DamageImmunityAcid)); break;
        case 4: eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Cold, 50), Effect.Icon(EffectIcon.DamageImmunityCold)); break;
        case 5: eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Fire, 50), Effect.Icon(EffectIcon.DamageImmunityFire)); break;
        case 6: eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Electrical, 50), Effect.Icon(EffectIcon.DamageImmunityElectrical)); break;
        case 7: eff = Effect.LinkEffects(Effect.DamageImmunityIncrease(DamageType.Sonic, 50), Effect.Icon(EffectIcon.DamageImmunitySonic)); break;
      }

      target.OnUnacquireItem += OnUnacquireTransmutationStone;
      
      eff.Tag = $"{TransmutationStoneEffectTag}{stone.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value}";
      eff.SubType = EffectSubType.Unyielding;
      return eff;
    }
    private static ScriptHandleResult OnRemoveTemporaryConSave(CallInfo callInfo)
    {
      EffectRunScriptEvent eventData = new EffectRunScriptEvent();

      if (eventData.EffectTarget is not NwCreature creature)
        return ScriptHandleResult.Handled;

      creature.RemoveFeat((Feat)CustomSkill.TemporaryConstitutionSaveProficiency);

      return ScriptHandleResult.Handled;
    }
    public static void OnUnacquireTransmutationStone(ModuleEvents.OnUnacquireItem unacquire)
    {
      NwItem stone = unacquire.Item;
      NwCreature creature = unacquire.LostBy;

      if (creature is null || stone is null || stone.Tag != "PierredeTransmutation")
        return;

      EffectUtils.RemoveTaggedEffect(creature, $"{TransmutationStoneEffectTag}{stone.GetObjectVariable<LocalVariableInt>("_CHARACTER_ID").Value}");
      creature.OnUnacquireItem -= OnUnacquireTransmutationStone;
    }
  }
}
