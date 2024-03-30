using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMobile(PlayerSystem.Player player, int customSkillId)
    {
      if(!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.Mobile))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.Mobile);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.mobileEffectTag))
        player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.mobile);

      player.oid.LoginCreature.OnCreatureAttack -= CreatureUtils.OnAttackMobile;
      player.oid.LoginCreature.OnCreatureAttack += CreatureUtils.OnAttackMobile;

      return true;
    }
  }
}
