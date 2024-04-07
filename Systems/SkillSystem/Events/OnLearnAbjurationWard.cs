using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAbjurationWard(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.AbjurationWard))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.AbjurationWard);

      NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.GetAbjurationWardEffect(2)));

      player.oid.LoginCreature.OnDamaged -= WizardUtils.OnDamageAbjurationWard;
      player.oid.LoginCreature.OnDamaged += WizardUtils.OnDamageAbjurationWard;

      return true;
    }
  }
}
