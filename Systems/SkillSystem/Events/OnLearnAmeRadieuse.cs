using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAmeRadieuse(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AmeRadieuse))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AmeRadieuse);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmeRadieuseEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AmeRadieuse));

      return true;
    }
  }
}
