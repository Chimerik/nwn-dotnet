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

      EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);

      player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 6));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);
      
      return true;
    }
  }
}
