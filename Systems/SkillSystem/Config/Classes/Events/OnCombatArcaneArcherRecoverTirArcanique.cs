using System.Linq;
using Anvil.API;
using Anvil.API.Events;

namespace NWN.Systems
{
  public static partial class FighterUtils
  {
    public static void OnCombatArcaneArcherRecoverTirArcanique(OnCombatStatusChange onStatus)
    {
      if(onStatus.CombatStatus == CombatStatus.EnterCombat && PlayerSystem.Players.TryGetValue(onStatus.Player.LoginCreature, out PlayerSystem.Player player))
      {
        foreach(var tirArcanique in player.learnableSkills.Values.Where(t => t.category == SkillSystem.Category.TirArcanique && t.currentLevel > 0))
          if(onStatus.Player.LoginCreature.GetFeatRemainingUses((Feat)tirArcanique.id) < 1)
            onStatus.Player.LoginCreature.IncrementRemainingFeatUses((Feat)tirArcanique.id);
      }
    }
  }
}
