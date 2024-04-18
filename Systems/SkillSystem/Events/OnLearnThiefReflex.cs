using System.Linq;
using Anvil.API;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnThiefReflex(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.ThiefReflex))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.ThiefReflex);

      player.oid.OnCombatStatusChange -= RogueUtils.OnCombatThiefReflex;
      player.oid.OnCombatStatusChange += RogueUtils.OnCombatThiefReflex;

      return true;
    }
  }
}
