using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

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
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraHoly, fScale: paladinLevel < 18 ? 0.8f : 1.9f),
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
        || protector.IsReactionTypeHostile(entering) || entering.ActiveEffects.Any(e => e.Tag == ProtectionEffectTag))
        return ScriptHandleResult.Handled;

      Effect eff = Effect.Icon(CustomEffectIcon.AuraDeProtection);
      if (protector.KnowsFeat((Feat)CustomSkill.PaladinAuraDeDevotion))
      {
        eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.AuraDeDevotion), GetCharmImmunityEffect(ProtectionEffectTag));
        EffectUtils.RemoveEffectType(entering, EffectType.Charmed);
        EffectUtils.RemoveTaggedEffect(entering, CharmEffectTag);
      }
      else if (protector.KnowsFeat((Feat)CustomSkill.PaladinAuraDeGarde))
      {
        eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.AuraDeGarde), ResistanceRadiant, ResistanceNecrotique, ResistancePsychique, Effect.RunAction());
      }
      else if (protector.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
        eff = Effect.Icon(CustomEffectIcon.AuraDeCourage);

      if (protector.KnowsFeat((Feat)CustomSkill.AuraDeCourage))
      {
        eff = Effect.LinkEffects(eff, Effect.Immunity(ImmunityType.Fear));
        EffectUtils.RemoveEffectType(entering, EffectType.Frightened);
        EffectUtils.RemoveTaggedEffect(entering, FrightenedEffectTag);
      }

      eff.Tag = ProtectionEffectTag;
      eff.Creator = protector;
      eff.CasterLevel = CreatureUtils.GetAbilityModifierMin1(protector, Ability.Charisma);
      eff.SubType = EffectSubType.Supernatural;

      entering.ApplyEffect(EffectDuration.Permanent, eff);

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
