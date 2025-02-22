using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string SanctuaireNaturelAuraEffectTag = "_SANCTUAIRE_NATUREL_AURA_EFFECT";
    public const string SanctuaireNaturelEffectTag = "_SANCTUAIRE_NATUREL_EFFECT";
    private static ScriptCallbackHandle onEnterSanctuaireNaturelCallback;
    private static ScriptCallbackHandle onExitSanctuaireNaturelCallback;
    public static Effect SantuaireNaturelAura(NwCreature caster)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(VfxType.ImpAuraFear), Effect.Icon(CustomEffectIcon.SanctuaireNaturel),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterHandle: onEnterSanctuaireNaturelCallback, onExitHandle: onExitSanctuaireNaturelCallback));
      eff.Tag = SanctuaireNaturelAuraEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      return eff;
    }
    public static Effect SanctuaireNaturel(PlayerSystem.Player druid)
    {
      DamageType damageType = DamageType.Fire;

      if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerrePolaire))
        damageType = DamageType.Cold;
      else if(druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTempere))
        damageType = DamageType.Electrical;
      else if (druid.learnableSkills.ContainsKey(CustomSkill.DruideCercleTerreTropicale))
        damageType = CustomDamageType.Poison;

      Effect eff = Effect.LinkEffects(Effect.ACIncrease(2),
        Effect.DamageImmunityIncrease(damageType, 50));
        
      eff.Tag = SanctuaireNaturelEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      return eff;
    }
    private static ScriptHandleResult onEnterSanctuaireNaturel(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature protector || protector.IsReactionTypeHostile(entering)
        || !PlayerSystem.Players.TryGetValue(protector, out PlayerSystem.Player player))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(protector, () => entering.ApplyEffect(EffectDuration.Permanent, SanctuaireNaturel(player)));
      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onExitSanctuaireNaturel(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) || eventData.Exiting is not NwCreature exiting
        || eventData.Effect.Creator is not NwCreature protector)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(exiting, protector, SanctuaireNaturelEffectTag);
      return ScriptHandleResult.Handled;
    }
  }
}
