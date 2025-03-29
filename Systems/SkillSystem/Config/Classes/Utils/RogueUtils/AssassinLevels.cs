using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleAssassinLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Assassin");
          player.oid.SetTextureOverride("rogue", "assassin");

          player.LearnClassSkill(CustomSkill.AssassinAssassinate);

          break;

        case 9: player.LearnClassSkill(CustomSkill.AssassinInfiltrationExpert); break;
        case 13: player.LearnClassSkill(CustomSkill.AssassinEnvenimer); break;
      }
    }
  }
}
