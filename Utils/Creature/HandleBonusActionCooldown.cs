using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class CreatureUtils
  {
    public static async void HandleBonusActionCooldown(NwCreature creature)
    {
      await NwTask.NextFrame();

      if (creature.IsPlayerControlled && !creature.ActiveEffects.Any(e => e.Tag == EffectSystem.BonusActionEffectTag))
      {
        foreach (var feat in creature.Feats.Where(f => f.TalentMaxCR.ToBool() && f.UsesPerDay > 0 && creature.GetFeatRemainingUses(f) > 0))
        {
          creature.GetObjectVariable<LocalVariableInt>($"_FEAT_REMAINING_USE_{feat.Id}").Value = creature.GetFeatRemainingUses(feat);
          DelayFeatReset(creature, feat);

          //ModuleSystem.Log.Info($"{feat.Name.ToString()} : {creature.GetFeatRemainingUses(feat)}");
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
