using Anvil.API;

namespace NWN.Systems
{
  public static partial class FeatUtils
  {
    public static async void DecrementFeatUses(NwCreature creature, int featId)
    {
      await NwTask.NextFrame();
      creature.DecrementRemainingFeatUses(NwFeat.FromFeatId(featId));
    }
  }
}
