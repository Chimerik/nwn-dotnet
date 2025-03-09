using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAuraDeDevotion(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.PaladinAuraDeDevotion))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.PaladinAuraDeDevotion);

      EffectUtils.RemoveTaggedEffect(player.oid.LoginCreature, EffectSystem.AuraDeProtectionEffectTag);

      player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AuraDeProtection(player.oid.LoginCreature, 7));
      UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(3);
    
      return true;
    }
  }
}
