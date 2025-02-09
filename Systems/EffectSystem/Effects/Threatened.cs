using System.Linq;
using Anvil.API;
using Anvil.Services;
using Anvil.API.Events;
using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public const string ThreatenedAoETag = "_THREAT_RANGE";
    public const string ThreatenedEffectTag = "_THREATENED_EFFECT";

    public static Effect threatAoE(NwCreature creature)
    {
      Effect eff = Effect.AreaOfEffect(PersistentVfxType.PerCustomAoe, scriptHandleFactory.CreateUniqueHandler(OnEnterThreatRange), null, scriptHandleFactory.CreateUniqueHandler(OnExitThreatRange));
      eff.Tag = ThreatenedAoETag;
      eff.SubType = EffectSubType.Supernatural;
      eff.Creator = creature;
      return eff;
    }
    public static Effect threatenedEffect
    {
      get
      {
        Effect eff = Effect.LinkEffects(Effect.Icon(CustomEffectIcon.Threatened));
        eff.Tag = ThreatenedEffectTag;
        eff.SubType = EffectSubType.Supernatural;
        return eff;
      }
    }

    public static ScriptHandleResult OnEnterThreatRange(CallInfo callInfo)
    {
      if (!callInfo.TryGetEvent(out AreaOfEffectEvents.OnEnter eventData) || eventData.Entering is not NwCreature entering
        || eventData.Effect.Creator is not NwCreature threatOrigin || threatOrigin.HP < 1
        || !entering.IsReactionTypeHostile(threatOrigin))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(threatOrigin, () => entering.ApplyEffect(EffectDuration.Permanent, threatenedEffect));

      if (threatOrigin.KnowsFeat(NwFeat.FromFeatId(CustomSkill.HastMaster))
        && threatOrigin.MovementType == MovementType.Stationary && entering.MovementType != MovementType.Stationary)
      {
        var reaction = threatOrigin.ActiveEffects.FirstOrDefault(e => e.Tag == ReactionEffectTag);

        if (reaction is not null)
        {
          switch (threatOrigin.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType)
          {
            case BaseItemType.Halberd:
            case BaseItemType.ShortSpear:
            case BaseItemType.Quarterstaff:

              switch (threatOrigin.CurrentAction)
              {
                case Action.AttackObject:
                  threatOrigin.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.HastMasterOpportunityVariable).Value = entering;
                  StringUtils.DisplayStringToAllPlayersNearTarget(threatOrigin, "Maître de Hast", StringUtils.gold, true);
                  threatOrigin.RemoveEffect(reaction);
                  break;

                case Action.Wait:
                case Action.Invalid:
                  threatOrigin.GetObjectVariable<LocalVariableObject<NwCreature>>(CreatureUtils.HastMasterOpportunityVariable).Value = entering;
                  StringUtils.DisplayStringToAllPlayersNearTarget(threatOrigin, "Maître de Hast", StringUtils.gold, true);
                  threatOrigin.RemoveEffect(reaction);
                  _ = threatOrigin.ActionAttackTarget(entering);
                  break;
              }

              break;
          }
        }
      }

      return ScriptHandleResult.Handled;
    }
    public static ScriptHandleResult OnExitThreatRange(CallInfo callInfo)
    {
      if (callInfo.TryGetEvent(out AreaOfEffectEvents.OnExit eventData) && eventData.Exiting is NwCreature exiting)
        EffectUtils.RemoveTaggedEffect(eventData.Exiting, eventData.Effect.Creator, ThreatenedEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
