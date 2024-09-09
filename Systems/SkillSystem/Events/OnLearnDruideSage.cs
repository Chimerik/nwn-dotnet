using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnDruideSage(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat((Feat)CustomSkill.DruideSage))
        player.oid.LoginCreature.AddFeat((Feat)CustomSkill.DruideSage);

      if (!player.windows.TryGetValue("spellSelection", out var spell1)) player.windows.Add("spellSelection", new SpellSelectionWindow(player, ClassType.Druid, 3));
      else ((SpellSelectionWindow)spell1).CreateWindow(ClassType.Druid, 3);

      return true;
    }
  }
}
