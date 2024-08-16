using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffiniteFroid(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoDracoAffiniteFroid);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ColdAffinityEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ColdAffinity));

      return true;
    }
  }
}
