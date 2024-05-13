using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRangerColdWanderer(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerColdWanderer))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerColdWanderer);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ColdWandererEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ColdWanderer));

      return true;
    }
  }
}
