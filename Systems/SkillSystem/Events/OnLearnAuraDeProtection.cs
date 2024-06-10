using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAuraDeProtection(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeProtection))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AuraDeProtection);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeProtectionEffectTag && e.Creator == player.oid.LoginCreature))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAuraDeProtectionEffect(player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level)));

      return true;
    }
  }
}
