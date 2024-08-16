using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffinitePoison(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffinitePoison))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoDracoAffinitePoison);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.PoisonAffinityEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.PoisonAffinity));

      return true;
    }
  }
}
