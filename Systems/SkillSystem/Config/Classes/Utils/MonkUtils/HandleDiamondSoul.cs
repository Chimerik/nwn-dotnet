﻿using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class MonkUtils
  {
    public static int HandleDiamondSoul(NwCreature creature, int saveRoll)
    {
      byte? level = creature.GetClassInfo((ClassType)CustomClass.Monk)?.Level;

      if (!level.HasValue || level.Value < 14 || creature.GetFeatRemainingUses((Feat)CustomSkill.MonkPatience) < 1
        || !creature.ActiveEffects.Any(e => e.Tag == EffectSystem.DiamondSoulEffectTag))
        return saveRoll;

      FeatUtils.DecrementKi(creature);

      int reroll = NwRandom.Roll(Utils.random, 20);

      StringUtils.DisplayStringToAllPlayersNearTarget(creature, "Âme de diamant", StringUtils.gold, true);
      LogUtils.LogMessage($"Echec JDS => Âme de diamant rerolled => {reroll}", LogUtils.LogType.Combat);

      return reroll;
    }
  }
}
