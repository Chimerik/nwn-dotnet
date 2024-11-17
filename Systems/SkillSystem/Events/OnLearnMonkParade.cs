using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMonkParade(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.MonkParade))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.MonkParade);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.MonkParadeEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.MonkParade));

      return true;
    }
  }
}
