using System.Security.Cryptography;
using Anvil.API;
using static NWN.Systems.PlayerSystem.Player;
using static NWN.Systems.PlayerSystem;

namespace NWN.Systems
{
  public partial class FeatSystem
  {
    private static void TransmutationStone(Player player)
    {
      if (player.oid.LoginCreature.Area.GetObjectVariable<LocalVariableInt>(AreaUtils.AreaLevelVariable).Value > 0)
      {
        player.oid.SendServerMessage("Vous devez vous trouver un endroit calme et sécurisé pour pouvoir créer votre pierre de transmutation", ColorConstants.Red);
        return;
      }

      if (player.craftJob != null)
      {
        player.oid.SendServerMessage("Veuillez annuler votre travail artisanal en cours avant d'en commencer un nouveau.", ColorConstants.Red);
        return;
      }

      player.craftJob = new CraftJob(player, JobType.TransmutationStone);
    }
  }
}
