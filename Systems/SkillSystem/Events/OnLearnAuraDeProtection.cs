using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAuraDeProtection(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AuraDeProtection))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AuraDeProtection);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AuraDeProtectionEffectTag && e.Creator == player.oid.LoginCreature))
      {
        int paladinLevel = player.oid.LoginCreature.GetClassInfo(ClassType.Paladin).Level;
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, paladinLevel));
        UtilPlugin.GetLastCreatedObject(11).ToNwObject<NwAreaOfEffect>().SetRadius(paladinLevel < 18 ? 3 : 9);
      }
      return true;
    }
  }
}
