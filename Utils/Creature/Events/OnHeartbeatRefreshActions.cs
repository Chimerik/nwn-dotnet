using System.Linq;
using Anvil.API;
using Anvil.API.Events;
using NWN.Native.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void OnHeartbeatRefreshActions(ModuleEvents.OnHeartbeat onHB)
    {
      foreach (var creature in NwObject.FindObjectsOfType<NwCreature>())
      {
        creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value = 1;
        creature.GetObjectVariable<LocalVariableInt>(HastMasterCooldownVariable).Delete();

        if(creature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.BersekerRepresailles)))
          creature.GetObjectVariable<LocalVariableInt>(BersekerRepresaillesVariable).Value = 1;

        if (creature.IsPlayerControlled)
        {
          foreach (var feat in creature.Feats.Where(f => f.TalentMaxCR.ToBool() && creature.GetFeatRemainingUses(f) < 1))
          {
            creature.SetFeatRemainingUses(feat, (byte)creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value);
            creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Delete();
          }
        }

        if (creature.ActiveEffects.Any(e => e.Tag == EffectSystem.noReactionsEffectTag))
          continue;

        creature.GetObjectVariable<LocalVariableInt>(ReactionVariable).Value = 1;
      }
    }
  }
}
