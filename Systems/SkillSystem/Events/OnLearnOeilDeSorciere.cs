using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnOeilDeSorciere(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.OeilDeSorciere))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.OeilDeSorciere);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.VisionLucideEffectTag))
      {
        var eff = EffectSystem.VisionLucide;
        eff.SubType = EffectSubType.Unyielding;

        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, eff));
      }

      return true;
    }
  }
}
