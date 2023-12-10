using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;

namespace NWN.Systems
{
  public static partial class SkillSystem
  {
    public static bool OnLearnElementalist(PlayerSystem.Player player, int customSkillId)
    {
      if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.Elementaliste)))
        player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.Elementaliste));

      if (!player.windows.TryGetValue("elementalistChoice", out var value)) player.windows.Add("elementalistChoice", new ElementalistChoiceWindow(player, player.oid.LoginCreature.Level));
      else ((ElementalistChoiceWindow)value).CreateWindow(player.oid.LoginCreature.Level);

      return true;
    }
  }
}
