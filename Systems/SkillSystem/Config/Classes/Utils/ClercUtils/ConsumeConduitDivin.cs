using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void ConsumeConduitDivin(NwCreature creature)
    { 
      await NwTask.NextFrame();
      creature.DecrementRemainingFeatUses(Feat.TurnUndead);
    }
  }
}
