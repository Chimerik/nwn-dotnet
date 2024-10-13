using System;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRecuperationNaturelle(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideRecuperationNaturelle))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideRecuperationNaturelle);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.DruideRecuperationNaturelle, (byte)Math.Round((double)(player.oid.LoginCreature.GetClassInfo(ClassType.Druid).Level / 2), MidpointRounding.AwayFromZero));
      return true;
    }
  }
}
