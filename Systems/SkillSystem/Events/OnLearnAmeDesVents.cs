using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnAmeDesVents(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.EnsoAmeDesVents))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.EnsoAmeDesVents);

      if(!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.AmeDesVentsEffectTag))
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.AmeDesVents));

      return true;
    }
  }
}
