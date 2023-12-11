using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnMartialInitiate(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.windows.TryGetValue("martialInitiateChoice", out var value)) player.windows.Add("martialInitiateChoice", new MartialInitiateChoiceWindow(player, player.oid.LoginCreature.Level));
      else ((MartialInitiateChoiceWindow)value).CreateWindow(player.oid.LoginCreature.Level);

      return true;
    }
  }
}
