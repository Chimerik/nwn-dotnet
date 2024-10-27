using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnVisionDiabolique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.VisionDiabolique))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.VisionDiabolique);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.VisionDiaboliqueEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.VisionDiabolique));

      return true;
    }
  }
}
