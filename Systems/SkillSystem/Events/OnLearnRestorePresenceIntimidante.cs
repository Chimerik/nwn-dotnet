
using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnRestorePresenceIntimidante(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.BersekerRestorePresenceIntimidante))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.BersekerRestorePresenceIntimidante);

      player.oid.LoginCreature.SetFeatRemainingUses((Feat)CustomSkill.BersekerRestorePresenceIntimidante, 0);

      return true;
    }
  }
}
