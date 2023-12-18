using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnExpert(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("skillBonusChoice", out var skill)) player.windows.Add("skillBonusChoice", new SkillBonusChoiceWindow(player, player.oid.LoginCreature.Level, CustomSkill.Expert));
      else ((SkillBonusChoiceWindow)skill).CreateWindow(player.oid.LoginCreature.Level, CustomSkill.Expert);

      return true;
    }
  }
}
