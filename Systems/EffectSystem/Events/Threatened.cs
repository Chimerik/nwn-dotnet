﻿using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using Anvil.Services;

using NWN.Core;

namespace NWN.Systems
{
  public partial class EffectSystem
  {
    public static ScriptHandleResult OnEnterThreatRange(CallInfo callInfo)
    {
      var eventData = new AreaOfEffectEvents.OnEnter();

      if (eventData.Entering is not NwCreature entering || eventData.Effect.Creator is not NwCreature threatOrigin || !entering.IsReactionTypeHostile(threatOrigin))
        return ScriptHandleResult.Handled;

      NWScript.AssignCommand(threatOrigin, () => entering.ApplyEffect(EffectDuration.Permanent, threatenedEffect));

      if(threatOrigin.KnowsFeat(NwFeat.FromFeatId(CustomSkill.HastMaster))  
        && threatOrigin.MovementType == MovementType.Stationary && entering.MovementType != MovementType.Stationary)
      {
        var reaction = threatOrigin.ActiveEffects.FirstOrDefault(e => e.Tag == EffectSystem.ReactionEffectTag);

        if (reaction is not null)
        {
          switch (threatOrigin.GetItemInSlot(InventorySlot.RightHand)?.BaseItem.ItemType)
          {
            case BaseItemType.Halberd:
            case BaseItemType.ShortSpear:
            case BaseItemType.Quarterstaff:
            case BaseItemType.Whip:

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
      var eventData = new AreaOfEffectEvents.OnExit();

      if (eventData.Exiting is not NwCreature exiting)
        return ScriptHandleResult.Handled;

      EffectUtils.RemoveTaggedEffect(eventData.Exiting, eventData.Effect.Creator, ThreatenedEffectTag);

      return ScriptHandleResult.Handled;
    }
  }
}
