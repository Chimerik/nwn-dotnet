using System.Linq;
using Anvil.API;
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

    public static Effect AuraDeProtection(NwCreature caster, int paladinLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraHoly, fScale: paladinLevel < 18 ? 0.9f : 1.8f),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterAuraDeProtectionCallback, onExitHandle: onExitAuraDeProtectionCallback));

      eff.Tag = AuraDeProtectionEffectTag;
      eff.SubType = EffectSubType.Unyielding;
      eff.Creator = caster;

      return eff;
    }
    private static ScriptHandleResult onEnterProtectionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || protector.HP < 1
        || entering.IsReactionTypeHostile(protector) || entering.ActiveEffects.Any(e => e.Tag == AuraDeProtectionEffectTag))
        return ScriptHandleResult.Handled;

      Effect eff = Effect.SavingThrowIncrease(SavingThrow.All, entering.GetAbilityModifier(Ability.Charisma) > 1 ? entering.GetAbilityModifier(Ability.Charisma) : 1);

      if (protector.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
      {
        eff = Effect.LinkEffects(eff, Effect.Immunity(ImmunityType.Fear));
        EffectUtils.RemoveEffectType(entering, EffectType.Frightened);
        EffectUtils.RemoveTaggedEffect(entering, FrightenedEffectTag);
      }

      if (protector.KnowsFeat((Feat)CustomSkill.PaladinAuraDeDevotion))
      {
        eff = Effect.LinkEffects(eff, GetCharmImmunityEffect(ProtectionEffectTag));
        EffectUtils.RemoveEffectType(entering, EffectType.Charmed);
        EffectUtils.RemoveTaggedEffect(entering, CharmEffectTag);
      }        

      if (protector.KnowsFeat((Feat)CustomSkill.PaladinAuraDeGarde))
      {
        Effect divine = Effect.DamageImmunityIncrease(DamageType.Divine, 50);
        divine.ShowIcon = false;

        Effect necrotic = Effect.DamageImmunityIncrease(CustomDamageType.Necrotic, 50);
        necrotic.ShowIcon = false;

        Effect psychic = Effect.DamageImmunityIncrease(CustomDamageType.Psychic, 50);
        psychic.ShowIcon = false;

        eff = Effect.LinkEffects(eff, divine, necrotic, psychic, Effect.Icon((EffectIcon)207), Effect.Icon((EffectIcon)215), Effect.Icon((EffectIcon)216));
      }
        

      eff.Tag = ProtectionEffectTag;
      eff.SubType = EffectSubType.Supernatural;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, eff));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitProtectionAura(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, ProtectionEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
