using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAbjurationWard(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AbjurationWard))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AbjurationWard);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AbjurationWardEffectTag && e.Creator == player.oid.LoginCreature))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(2)));

      return true;
    }
  }
}
