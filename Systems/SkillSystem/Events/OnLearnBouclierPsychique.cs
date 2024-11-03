﻿
using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnBouclierPsychique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BouclierPsychique))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BouclierPsychique);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.BouclierPsychiqueEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.BouclierPsychique));

      return true;
    }
  }
}
