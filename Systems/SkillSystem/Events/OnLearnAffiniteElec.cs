using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAffiniteElec(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoDracoAffiniteElec))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoDracoAffiniteElec);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.ElectricityAffinityEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ElectricityAffinity));

      return true;
    }
  }
}
