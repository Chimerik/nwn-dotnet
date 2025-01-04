using System.Linq;
using Anvil.API;
using NWN.Core.NWNX;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBenedictionDuMalin(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BenedictionDuMalin))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BenedictionDuMalin);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BenedictionDuMalinAuraEffectTag))
      {
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BenedictionDuMalinAura(player.oid.LoginCreature));
        UtilPlugin.GetLastCreatedObject(NWNXObjectType.AreaOfEffect).ToNwObject<NwAreaOfEffect>().SetRadius(4);
      }

      player.oid.LoginCreature.OnCreatureDamage -= OccultisteUtils.OnDamageBenedictionDuMalin;
      player.oid.LoginCreature.OnCreatureDamage += OccultisteUtils.OnDamageBenedictionDuMalin;

      return true;
    }
  }
}
