using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRangerFireWanderer(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerFireWanderer))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerFireWanderer);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FireWandererEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FireWanderer));

      return true;
    }
  }
}
