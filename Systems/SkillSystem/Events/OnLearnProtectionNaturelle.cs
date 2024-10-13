using System;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnProtectionNaturelle(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideProtectionNaturelle))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideProtectionNaturelle);

      NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.ProtectionNaturelle(player)));

      return true;
    }
  }
}
