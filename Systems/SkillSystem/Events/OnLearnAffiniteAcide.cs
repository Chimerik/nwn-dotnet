using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffiniteAcide(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoDracoAffiniteAcide);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AcidAffinityEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AcidAffinity));

      return true;
    }
  }
}
