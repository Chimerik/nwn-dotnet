using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnCombatWarMasterRecoverManoeuvre(OnCombatStatusChange onStatus)
    {
      if (onStatus.CombatStatus == CombatStatus.EnterCombat && PlayerSystem.Players.TryGetValue(onStatus.Player.LoginCreature, out PlayerSystem.Player player))
      {
        foreach (var manoeuvre in player.learnableSkills.Values.Where(t => t.category == SkillSystem.Category.Manoeuvre && t.currentLevel > 0))
          if (onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)manoeuvre.id) < 1)
            onStatus.Player.LoginCreature.IncrementRemainingFeatUses((Feat)manoeuvre.id);
      }
    }
  }
}
