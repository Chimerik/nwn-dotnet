using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public static partial class Rogue
  {
    public static void HandleThiefLevelUp(Player player, int level)
    {
      switch (level)
      {
        case 3: 
          
          new StrRef(16).SetPlayerOverride(player.oid, "Voleur");
          player.oid.SetTextureOverride("rogue", "thief");

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.MainLeste)))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.MainLeste));

          break;

        case 9:

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.DiscretionSupreme)))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.DiscretionSupreme));

          if (!player.oid.LoginCreature.KnowsFeat(NwFeat.FromFeatId(CustomSkill.ThiefInvisibility)))
            player.oid.LoginCreature.AddFeat(NwFeat.FromFeatId(CustomSkill.ThiefInvisibility));

          break;

        case 13:

          

          break;
      }
    }
  }
}
