using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string FureurDesFlotsEffectTag = "_FUREUR_DES_FLOTS_EFFECT";
    private static ScriptCallbackHandle onEnterFureurDesFlotsCallback;
    private static ScriptCallbackHandle onHeartbeatFureurDesFlotsCallback;
    public static Effect FureurDesFlots(NwCreature caster, int druideLevel)
    {
      Effect eff = Effect.LinkEffects(Effect.VisualEffect(CustomVfx.FureurDesFlots),
        Effect.VisualEffect(VfxType.DurAuraCyan, fScale: druideLevel < 18 ? 0.9f : 1.8f), Effect.Icon((EffectIcon)190),
        Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, onEnterFureurDesFlotsCallback, onHeartbeatFureurDesFlotsCallback));

      if (druideLevel > 9)
        eff = Effect.LinkEffects(eff, ResistanceFroid, ResistanceTonnerre, ResistanceElec);

      eff.Tag = FureurDesFlotsEffectTag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = caster;
      return eff;
    }
    private static ScriptHandleResult onEnterFureurDesFlots(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering 
        || eventData.Effect.Creator is not NwCreature protector || !protector.IsReactionTypeHostile(entering))
        return ScriptHandleResult.Handled;

      int spellDC = SpellUtils.GetCasterSpellDC(protector, Ability.Wisdom);

      if (CreatureUtils.GetSavingThrowResult(entering, Ability.Constitution, protector, spellDC) == SavingThrowResult.Failure)
        entering.ApplyEffect(EffectDuration.Temporary, Effect.MovementSpeedDecrease(50), NwTimeSpan.FromRounds(1));

      return ScriptHandleResult.Handled;
    }
    private static ScriptHandleResult onHeartbeatFureurDesFlots(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnHeartbeat eventData) && eventData.Effect.Creator is NwCreature caster)
      {
        var target = caster.GetNearestCreatures(CreatureTypeFilter.Alive(true), CreatureTypeFilter.Reputation(ReputationType.Enemy), CreatureTypeFilter.Perception(PerceptionType.Seen)).FirstOrDefault();
      
        if(target is not null)
        {
          int spellDC = SpellUtils.GetCasterSpellDC(caster, Ability.Wisdom);

          if (CreatureUtils.GetSavingThrowResult(target, Ability.Constitution, caster, spellDC) == SavingThrowResult.Failure)
          {
            int damage = NwRandom.Roll(Utils.random, 6, caster.GetAbilityModifier(Ability.Wisdom) > 1 ? caster.GetAbilityModifier(Ability.Wisdom) : 1);
            NWScript.AssignCommand(caster, () => target.ApplyEffect(EffectDuration.Instant, Effect.Damage(damage, DamageType.Sonic)));
          }
        }
      }

      return ScriptHandleResult.Handled;
    }
  }
}
