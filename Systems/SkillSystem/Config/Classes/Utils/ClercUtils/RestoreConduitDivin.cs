﻿using Anvil.API;

namespace NWN.Systems
{
  public static partial class ClercUtils
  {
    public static async void RestoreConduitDivin(NwCreature creature)
    {
      byte? level = creature.GetClassInfo(ClassType.Cleric)?.Level;

      if (!level.HasValue)
        return;

      byte conduitUses = (byte)(level.Value < 6 ? 1 : level.Value < 18 ? 2 : 3);

      await NwTask.NextFrame();
      creature.SetFeatRemainingUses(Feat.TurnUndead, conduitUses);
    }
  }
}
