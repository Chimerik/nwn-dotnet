using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRangerAcidWanderer(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerAcidWanderer))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerAcidWanderer);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AcidWandererEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AcidWanderer));

      return true;
    }
  }
}
