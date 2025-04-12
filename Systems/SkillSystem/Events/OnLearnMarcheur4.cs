using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMarcheur4(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)customSkillId))
        player.oid.LoginCreature.AddFeat((Feat)customSkillId);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.Marcheur4EffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.Marcheur4));

      return true;
    }
  }
}
