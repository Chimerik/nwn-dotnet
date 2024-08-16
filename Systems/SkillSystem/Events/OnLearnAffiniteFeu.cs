using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffiniteFeu(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoDracoAffiniteFeu);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.FireAffinityEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.FireAffinity));

      return true;
    }
  }
}
