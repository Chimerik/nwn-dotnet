using System.Linq;
using Anvil.API;
using NWN.Systems;

namespace NWN
{
  public static partial class CreatureUtils
  {
    public static void HandleBonusActionCooldown(NwCreature creature)
    {
      if (creature.IsPlayerControlled && creature.GetObjectVariable<LocalVariableInt>(BonusActionVariable).Value < 1)
      {
        foreach (var feat in creature.Feats.Where(f => f.TalentMaxCR.ToBool()))
        {
          if (feat.UsesPerDay > 0 && creature.GetFeatRemainingUses(feat) > 0)
          {
            creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value = creature.GetFeatRemainingUses(feat);
            DelayFeatReset(creature, feat);
          }

          ModuleSystem.Log.Info($"{feat.Name.ToString()} : {creature.GetFeatRemainingUses(feat)}");
        }
      }
    }
    private static async void DelayFeatReset(NwCreature creature, NwFeat feat)
    {
      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(feat, 0);
    }
  }
}
