using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnGuerrierDruidique(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.RangerGuerrierDruidique))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.RangerGuerrierDruidique);

      if (!player.windows.TryGetValue("spellSelection", out var spell2)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Ranger, 2));
      else ((SpellSelectionWindow)spell2).CreateWindow(ClassType.Druid, 2);

      return true;
    }
  }
}
