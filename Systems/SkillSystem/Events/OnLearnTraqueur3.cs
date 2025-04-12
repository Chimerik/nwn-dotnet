using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnTraqueur3(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Antidetection(false)));

      return true;
    }
  }
}
