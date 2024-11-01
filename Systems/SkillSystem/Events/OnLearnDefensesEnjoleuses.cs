using System.Linq;
using Anvil.API;
using NWN.Core;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDefensesEnjoleuses(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DefensesEnjoleuses))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DefensesEnjoleuses);

      if (!player.oid.LoginCreature.ActiveEffects.Any(e => e.Tag == EffectSystem.DefensesEnjoleusesEffectTag))
      {
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.DefensesEnjoleuses));
        NWScript.AssignCommand(player.oid.LoginCreature, () => player.oid.LoginCreature.ApplyEffect(EffectDuration.Permanent, EffectSystem.CharmeImmunite));
      
      }
      return true;
    }
  }
}
